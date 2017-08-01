using Foundation;
using System;
using UIKit;
using Mehspot.Core;
using SDWebImage;
using Mehspot.Models.ViewModels;
using Mehspot.Core.DTO.Badges;
using Mehspot.Core.Contracts.Wrappers;
using Mehspot.iOS.Wrappers;
using Mehspot.Core.Services;
using Mehspot.Core.DTO;
using Mehspot.Core.Models;
using Mehspot.iOS.Core.Builders;

namespace Mehspot.iOS
{
	public partial class ViewProfileViewController : UIViewController, IUITableViewDataSource, IUITableViewDelegate,
	IViewBadgeProfileController
	{
		private ViewBadgeProfileModel<UITableViewCell> model;
		private string MessageUserId;
		private string MessageUserName;

		public IViewHelper ViewHelper { get; private set; }
		public int BadgeId { get; set; }
		public string BadgeName { get; set; }
		public string UserId { get; set; }

		public UIViewController ParentController { get; set; }

		#region IViewBadgeProfileController
		public string WindowTitle
		{
			get
			{
				return this.NavBar.TopItem.Title;
			}

			set
			{
				this.NavBar.TopItem.Title = value;
			}
		}

		public string Subdivision
		{
			get
			{
				return SubdivisionLabel.Text;
			}

			set
			{
				SubdivisionLabel.Text = value;
			}
		}

		public string Distance
		{
			get
			{
				return DistanceLabel.Text;
			}

			set
			{
				DistanceLabel.Text = value;
			}
		}

		public string Likes
		{
			get
			{
				return LikesLabel.Text;
			}

			set
			{
				LikesLabel.Text = value;
			}
		}

		public string InfoLabel1
		{
			get
			{
				return HourlyRateLabel.Text;
			}

			set
			{
				HourlyRateLabel.Text = value;
			}
		}

		public string InfoLabel2
		{
			get
			{
				return AgeRangeLabel.Text;
			}

			set
			{
				AgeRangeLabel.Text = value;
			}
		}

		public string FirstName
		{
			get
			{
				return FirstNameLabel.Text;
			}

			set
			{
				FirstNameLabel.Text = value;
			}
		}

		public bool HideFavoriteIcon
		{
			get
			{
				return FavoriteIcon.Hidden;
			}

			set
			{
				FavoriteIcon.Hidden = value;
			}
		}

		public bool EnableSendMessageButton
		{
			get
			{
				return SendMessageButton.Enabled;
			}

			set
			{
				SendMessageButton.Enabled = value;
			}
		}

		public void SetProfilePictureUrl(string value)
		{

			if (!string.IsNullOrEmpty(value))
			{
				var url = NSUrl.FromString(value);
				if (url != null)
				{
					this.ProfilePicture.SetImage(url);
				}
			}
		}
		#endregion

		public ViewProfileViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			TableView.TableHeaderView.Hidden = true;
			TableView.TableFooterView = new UIView();
			TableView.DataSource = this;
			TableView.Delegate = this;
			this.ProfilePicture.UserInteractionEnabled = true;
			this.ProfilePicture.AddGestureRecognizer(new UITapGestureRecognizer(ProfilePictureDoupleTapped) { NumberOfTapsRequired = 2 });

			this.ViewHelper = new ViewHelper(this.View);
			model = new ViewBadgeProfileModel<UITableViewCell>(this, new BadgeService(MehspotAppContext.Instance.DataStorage), new IosCellBuilder());
			model.OnRefreshing += Model_OnRefreshing;
			model.OnRefreshed += Model_OnRefreshed;
			model.OnWriteReviewButtonTouched += RecommendationsDataSource_OnWriteReviewButtonTouched;
			model.OnGoToMessaging += GoToMessaging;
		}

		public override async void ViewWillAppear(bool animated)
		{
			SendMessageButton.SelectedSegment = 0;
			await model.RefreshView();
		}

		public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
		{
			if (segue.Identifier == "GoToMessagingSegue" && this.model.Profile != null)
			{
				var controller = (MessagingViewController)segue.DestinationViewController;
				controller.ToUserId = MessageUserId;
				controller.ToUserName = MessageUserName;
				controller.ProfilePicturePath = this.model.Profile.Details.ProfilePicturePath;
				controller.ParentController = this;
			}
			else if (segue.Identifier == "GoToWriteRecommendationSegue")
			{
				var controller = (WriteReviewController)segue.DestinationViewController;
				controller.BadgeId = this.BadgeId;
				controller.UserId = this.UserId;
				controller.OnSave += RecommendationAdded;
			}

			base.PrepareForSegue(segue, sender);
		}

		public void ReloadCells()
		{
			TableView.ReloadData();
			TableView.LayoutIfNeeded();
		}

		async partial void SegmentControlChanged(UISegmentedControl sender)
		{
			switch (sender.SelectedSegment)
			{
				case 0:
					{
						model.LoadProfile();
						break;
					}
				case 1:
					{
						await model.LoadRecommendations();
						break;
					}
				case 2:
					{
						GoToMessaging(this.UserId, this.FirstName);
						break;
					}
			}
		}

		partial void CloseButtonTouched(UIBarButtonItem sender)
		{
			this.DismissViewController(true, null);
		}

		void Model_OnRefreshing()
		{
			TableView.Hidden = true;
			TableView.UserInteractionEnabled = false;
			ActivityIndicator.Hidden = false;
			ActivityIndicator.StartAnimating();
		}

		void Model_OnRefreshed()
		{
			ActivityIndicator.StopAnimating();
			TableView.Hidden = false;
			ActivityIndicator.Hidden = true;
			TableView.UserInteractionEnabled = true;
			TableView.TableHeaderView.Hidden = false;
		}

		void RecommendationAdded(BadgeUserRecommendationDTO recommendation)
		{
			model.AddRecommendation(recommendation);
			model.HideCreateButton();
			TableView.ReloadData();
			TableView.LayoutIfNeeded();
		}

		void RecommendationsDataSource_OnWriteReviewButtonTouched()
		{
			PerformSegue("GoToWriteRecommendationSegue", this);
		}

		void GoToMessaging(string userId, string userName)
		{
			MessageUserId = userId;
			MessageUserName = userName;
			string segueName = this.ParentController is UserProfileViewController ? "UnwindToMessagingSegue" : "GoToMessagingSegue";
			base.PerformSegue(segueName, this);
		}

		private async void ProfilePictureDoupleTapped()
		{
			await model.ToggleFavoriteAsync();
		}

		#region IUITableViewDataSource, IUITableViewDelegate
		public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var item = model.Cells.Count > indexPath.Row ? this.model.Cells[indexPath.Row] : null;
			return item;
		}

		public nint RowsInSection(UITableView tableview, nint section)
		{
			return this.model.Cells.Count;
		}

		[Export("tableView:heightForRowAtIndexPath:")]
		public nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			if (model.ShowRecommendations)
			{
				if (model.Cells.Count > 0)
				{
					var cell = model.Cells[indexPath.Row];
					return cell.Frame.Height;
				}

				return 70;
			}

			return this.model.Cells[indexPath.Row]?.Frame.Height ?? 0;
		}

		[Export("tableView:didSelectRowAtIndexPath:")]
		public void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			model.RowSelected(indexPath.Row);
		}
		#endregion

	}
}