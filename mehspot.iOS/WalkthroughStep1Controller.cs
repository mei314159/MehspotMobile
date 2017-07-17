using Foundation;
using System;
using UIKit;
using SDWebImage;
using System.IO;

namespace mehspot.iOS
{
	public delegate void WalkthroughStep1Delegate(Stream image);

	public partial class WalkthroughStep1Controller : UIViewController
	{
		private volatile bool profileImageChanged;
		string profilePicturePath;
		public string ProfilePicturePath
		{
			get
			{
				return profilePicturePath;
			}

			set
			{
				if (IsViewLoaded && !string.IsNullOrEmpty(value))
				{
					var url = NSUrl.FromString(value);
					if (url != null)
					{
						this.ProfilePicture.SetImage(url);
						this.ContinueButton.Hidden = false;
					}
				}

				profilePicturePath = value;
			}
		}

		public event WalkthroughStep1Delegate OnContinue;

		public WalkthroughStep1Controller(IntPtr handle) : base(handle)
		{

		}

		public override bool PrefersStatusBarHidden()
		{
			return true;
		}

		partial void ContinueButtonTouched(UIButton sender)
		{
			if (OnContinue != null)
			{
				Stream dataBytes = null;
				if (profileImageChanged)
				{
					dataBytes = this.ProfilePicture.Image.AsJPEG().AsStream();
				}

				OnContinue.Invoke(dataBytes);
			}
		}

		partial void PictureButtonTouched(UIButton sender)
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

			PresentModalViewController(imagePicker, true);
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
				UIView.Animate(0.2, () =>
				{
					PictureButton.Layer.BorderWidth = 1;
					PictureButton.Layer.BorderColor = PictureButton.BackgroundColor.CGColor;
					PictureButton.SetTitleColor(PictureButton.BackgroundColor, UIControlState.Normal);
					PictureButton.BackgroundColor = UIColor.White;
					PictureButtonBottomConstraint.Constant = PictureButtonBottomConstraint.Constant + 20;
					ContinueButton.Hidden = false;
				}, () =>
				{
					this.profileImageChanged = true;
				});
			}

			((UIImagePickerController)sender).DismissModalViewController(true);
		}

		void Handle_Canceled(object sender, EventArgs e)
		{
			((UIImagePickerController)sender).DismissModalViewController(true);
		}
	}
}