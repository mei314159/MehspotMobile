using Foundation;
using System;
using UIKit;
using CoreGraphics;
using Mehspot.iOS.Wrappers;
using Mehspot.Core.Contracts.Wrappers;
using Mehspot.Core.Contracts.ViewControllers;
using Mehspot.Core.Models;
using Mehspot.iOS.Views;
using Mehspot.Core.Services;
using Mehspot.Core;
using Mehspot.Core.DTO;
using SDWebImage;
using CoreImage;

namespace Mehspot.iOS
{
	public partial class UserProfileViewController : UIViewController, IUITableViewDataSource, IUITableViewDelegate, IUserProfileViewController
	{
		private UserProfileViewModel model;
		private BadgeService badgeService;
		private ProfileService profileService;
		private string profilePicturePath;

		public string ToUserName { get; set; }
		public string ToUserId { get; set; }
		public UIViewController ParentController { get; set; }
		public IViewHelper ViewHelper { get; private set; }

		#region IUserProfileViewController
		public string UserName
		{
			get
			{
				return this.NavigationBar.Title;
			}

			set
			{
				this.NavigationBar.Title = value;
			}
		}

		public string FullName
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
						this.UserProfileImage.SetImage(url, (image, error, cacheType, imageUrl) =>
						{
							this.UserBlurImage.Image = Blur(image);
						});

						profilePicturePath = value;
					}
				}
			}
		}

		public int RecommendationsCount
		{
			get
			{
				return Int32.Parse(RecommendationsLabel.Text);
			}

			set
			{
				RecommendationsLabel.Text = value.ToString();
			}
		}

		public int ReferencesCount
		{
			get
			{
				return Int32.Parse(LikesLabel.Text);
			}

			set
			{
				LikesLabel.Text = value.ToString();
			}
		}
		#endregion

		public UserProfileViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			this.TableView.WeakDelegate = this;
			this.TableView.WeakDataSource = this;
			this.ViewHelper = new ViewHelper(this.View);
			badgeService = new BadgeService(MehspotAppContext.Instance.DataStorage);
			profileService = new ProfileService(MehspotAppContext.Instance.DataStorage);
			ViewHelper.ShowOverlay("Wait...", true);

			model = new UserProfileViewModel(badgeService, profileService, this);
			model.UserId = this.ToUserId;

			this.TableView.RegisterNibForCellReuse(BadgeItemCellSimplified.Nib, BadgeItemCellSimplified.Key);
		}

		public override async void ViewDidAppear(bool animated)
		{
			if (!model.dataLoaded)
			{
				ViewHelper.ShowOverlay("Wait...", true);
				model.LoadAsync();
			}
		}

		partial void CloseButtonTouched(UIBarButtonItem sender)
		{
			DismissViewController(true, null);
		}

		public void ReloadData()
		{
			TableView.ReloadData();
		}

		public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
		{
			var selectedBadge = model.SelectedBadge;
			if (segue.Identifier == "GoToBadgeProfileViewSegue")
			{
				var controller = (ViewProfileViewController)segue.DestinationViewController;
				controller.BadgeId = selectedBadge.BadgeId;
				controller.BadgeName = selectedBadge.BadgeName;
				controller.UserId = model.UserId;
			}
		}

		public static UIImage Blur(UIImage image, float blurRadius = 2f)
		{
			if (image == null)
			{
				return null;
			}
			// Create a new blurred image.
			var inputImage = new CIImage(image);
			var blur = new CIGaussianBlur();
			blur.Image = inputImage;
			blur.Radius = blurRadius;

			var outputImage = blur.OutputImage;
			var context = CIContext.FromOptions(new CIContextOptions { UseSoftwareRenderer = false });
			var cgImage = context.CreateCGImage(outputImage, new CGRect(0, 0, inputImage.Extent.Width, inputImage.Extent.Height));
			var newImage = UIImage.FromImage(cgImage);

			// Clean up
			inputImage.Dispose();
			context.Dispose();
			blur.Dispose();
			outputImage.Dispose();
			cgImage.Dispose();

			return newImage;
		}

		#region IUITableViewDataSource, IUITableViewDelegate
		public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var item = model.Items[indexPath.Row];
			var cell = tableView.DequeueReusableCell(BadgeItemCellSimplified.Key, indexPath) as BadgeItemCellSimplified ?? BadgeItemCellSimplified.Create();
			ConfigureCell(cell, item);
			return cell;
		}

		public nint RowsInSection(UITableView tableView, nint section)
		{
			return model.Items?.Count ?? 0;
		}

		[Export("tableView:didSelectRowAtIndexPath:")]
		public void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			model.SelectRow(indexPath.Row);
			tableView.DeselectRow(indexPath, true);
			PerformSegue("GoToBadgeProfileViewSegue", this);
		}
		#endregion

		private void ConfigureCell(BadgeItemCellSimplified cell, BadgeSummaryBaseDTO badge)
		{
			cell.Configure(badge);
		}
	}
}
