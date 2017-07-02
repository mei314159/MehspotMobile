using Foundation;
using System;
using UIKit;
using Mehspot.Core.Contracts.Wrappers;
using Mehspot.Core.DTO;
using Mehspot.Core;
using Mehspot.iOS.Wrappers;
using Mehspot.iOS.Extensions;
using System.Runtime.InteropServices;
using CoreGraphics;
using SDWebImage;
using Facebook.LoginKit;
using Mehspot.Core.Services;
using Mehspot.Core.Models;
using Mehspot.iOS.Core.Builders;

namespace Mehspot.iOS
{
	public partial class EditProfileViewController : UITableViewController, IProfileViewController
	{
		volatile bool loading;
		volatile bool profileImageChanged;

		private ProfileService profileService;
		private SubdivisionService subdivisionService;
		private ProfileModel<UITableViewCell> model;
		private string profilePicturePath;

		public ProfileDto profile;

		#region IProfileViewController
		public IViewHelper ViewHelper { get; set; }

		public string UserName
		{
			get
			{
				return UserNameLabel.Text;
			}

			set
			{
				UserNameLabel.Text = value;
			}
		}

		string IProfileViewController.FullName
		{
			get
			{
				return FullName.Text;
			}

			set
			{
				FullName.Text = value;
			}
		}

		public string ProfilePicturePath
		{
			get
			{
				return profilePicturePath;
			}

			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					var url = NSUrl.FromString(value);
					if (url != null)
					{
						this.ProfilePicture.SetImage(url);
						profilePicturePath = value;
					}
				}
			}
		}

		public bool SaveButtonEnabled
		{
			get
			{
				return SaveButton.Enabled;
			}
			set
			{
				SaveButton.Enabled = value;
			}
		}

		#endregion

		public EditProfileViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			profileService = new ProfileService(MehspotAppContext.Instance.DataStorage);
			subdivisionService = new SubdivisionService(MehspotAppContext.Instance.DataStorage);
			ViewHelper = new ViewHelper(this.View);

			ChangePhotoButton.Layer.BorderWidth = 1;
			ChangePhotoButton.Layer.BorderColor = UIColor.LightGray.CGColor;
			TableView.AddGestureRecognizer(new UITapGestureRecognizer(this.HideKeyboard));
			TableView.TableHeaderView.Hidden = TableView.TableFooterView.Hidden = true;
			TableView.RowHeight = UITableView.AutomaticDimension;
			TableView.EstimatedRowHeight = 44;
			this.RefreshControl.ValueChanged += RefreshControl_ValueChanged;

			model = new ProfileModel<UITableViewCell>(profileService, subdivisionService, this, new IosCellBuilder());
			model.LoadingStart += Model_LoadingStart;
			model.LoadingEnd += Model_LoadingEnd;
			model.SignedOut += Model_SignedOut;
		}

		public override async void ViewDidAppear(bool animated)
		{
			if (!model.dataLoaded)
			{
				TableView.TableHeaderView.Hidden = TableView.TableFooterView.Hidden = false;
				await model.RefreshView();
			}
		}

		void Model_LoadingStart()
		{
			this.SaveButton.Enabled = this.ChangePhotoButton.Enabled = TableView.UserInteractionEnabled = false;
			this.RefreshControl.BeginRefreshing();
			TableView.SetContentOffset(new CGPoint(0, -this.RefreshControl.Frame.Size.Height), true);
		}

		void Model_LoadingEnd()
		{
			TableView.SetContentOffset(CGPoint.Empty, true);
			RefreshControl.EndRefreshing();
			this.SaveButton.Enabled = this.ChangePhotoButton.Enabled = TableView.UserInteractionEnabled = true;
		}

		#region UITableView
		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var item = model.Cells[indexPath.Row];
			return item;
		}

		public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			var item = model.Cells[indexPath.Row];
			return item.Frame.Height;
		}

		public override nint RowsInSection(UITableView tableView, nint section)
		{
			return model.Cells.Count;
		}
		#endregion

		private async void RefreshControl_ValueChanged(object sender, EventArgs e)
		{
			await model.RefreshView();
		}

		partial void ChangePhotoButtonTouched(UIButton sender)
		{
			var photoSourceActionSheet = new UIActionSheet("Take a photo from");
			photoSourceActionSheet.AddButton("Camera");
			photoSourceActionSheet.AddButton("Photo Library");
			photoSourceActionSheet.AddButton("Cancel");
			photoSourceActionSheet.CancelButtonIndex = 2;
			photoSourceActionSheet.Clicked += PhotoSouceActionSheet_Clicked;
			photoSourceActionSheet.ShowInView(View);
		}

		private void PhotoSouceActionSheet_Clicked(object sender, UIButtonEventArgs e)
		{
			var imagePicker = new UIImagePickerController();
			imagePicker.MediaTypes = new string[] { MobileCoreServices.UTType.Image };
			if (e.ButtonIndex == 0)
			{
				imagePicker.SourceType = UIImagePickerControllerSourceType.Camera;
			}
			else if (e.ButtonIndex == 1)
			{
				imagePicker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
			}
			else
			{
				return;
			}

			imagePicker.FinishedPickingMedia += Handle_FinishedPickingMedia;
			imagePicker.Canceled += Handle_Canceled;

			NavigationController.PresentModalViewController(imagePicker, true);
		}

		void Handle_FinishedPickingMedia(object sender, UIImagePickerMediaPickedEventArgs e)
		{
			if (e.Info[UIImagePickerController.MediaType].ToString() != MobileCoreServices.UTType.Image)
				return;

			NSUrl referenceURL = e.Info[new NSString(UIImagePickerController.ReferenceUrl)] as NSUrl;
			if (referenceURL != null)
				Console.WriteLine("Url:" + referenceURL);

			UIImage originalImage = e.Info[UIImagePickerController.OriginalImage] as UIImage;
			if (originalImage != null)
			{
				ProfilePicture.Image = UIImage.FromImage(originalImage.CGImage, 4, originalImage.Orientation);

				this.profileImageChanged = true;
			}

			((UIImagePickerController)sender).DismissModalViewController(true);
		}

		void Handle_Canceled(object sender, EventArgs e)
		{
			((UIImagePickerController)sender).DismissModalViewController(true);
		}

		async partial void SaveButtonTouched(UIBarButtonItem sender)
		{
			byte[] dataBytes = null;
			if (profileImageChanged)
			{
				var data = this.ProfilePicture.Image.AsJPEG();
				dataBytes = new byte[data.Length];
				Marshal.Copy(data.Bytes, dataBytes, 0, Convert.ToInt32(data.Length));
			}

			await model.SaveProfileAsync(dataBytes);
		}

		partial void SignoutButtonTouched(UIButton sender)
		{
			model.Signout();
		}

		void Model_SignedOut()
		{
			new LoginManager().LogOut();
			var targetViewController = UIStoryboard.FromName("Main", null).InstantiateViewController("LoginViewController");
			this.View.Window.SwapController(targetViewController);
		}

		public void ReloadData()
		{
			TableView.ReloadData();
		}

		public void HideKeyboard()
		{
			ViewExtensions.HideKeyboard(this);
		}
	}
}
