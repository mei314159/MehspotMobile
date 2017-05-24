using Foundation;
using System;
using UIKit;
using Mehspot.Core;
using System.Threading.Tasks;
using SDWebImage;
using Mehspot.Models.ViewModels;
using Mehspot.Core.DTO.Badges;
using Mehspot.iOS.Controllers.Badges.DataSources.Search;
using Mehspot.iOS.Controllers.Badges.BadgeProfileDataSource;
using Mehspot.Core.Contracts.Wrappers;
using Mehspot.iOS.Wrappers;
using Mehspot.Core.Services;
using Mehspot.Core.DTO;

namespace Mehspot.iOS
{
    public partial class ViewProfileViewController : UIViewController
    {
        private volatile bool loading;
        public ISearchResultDTO SearchResultDTO;
        public BadgeSummaryDTO BadgeSummary;

        ViewBadgeProfileTableSource profileDataSource;
        RecommendationsTableSource recommendationsDataSource;
        IViewHelper viewHelper;

		BadgeService badgeService;

        private string MessageUserId;
        private string MessageUserName;

        public ViewProfileViewController (IntPtr handle) : base (handle)
		{
        }

        public override async void ViewDidLoad ()
        {
            this.NavBar.TopItem.Title =
                    (MehspotResources.ResourceManager.GetString (BadgeSummary.BadgeName) ?? BadgeSummary.BadgeName) + " Profile";
            this.viewHelper = new ViewHelper (this.View);
			this.badgeService = new BadgeService(MehspotAppContext.Instance.DataStorage);
			TableView.TableHeaderView.Hidden = true;
            TableView.TableFooterView = new UIView ();
            this.ProfilePicture.UserInteractionEnabled = true;
            var tapGestureRecognizer = new UITapGestureRecognizer (ProfilePictureDoupleTapped);
            tapGestureRecognizer.NumberOfTapsRequired = 2;
            this.ProfilePicture.AddGestureRecognizer (tapGestureRecognizer);

			profileDataSource = new ViewBadgeProfileTableSource(BadgeSummary.BadgeId, BadgeSummary.BadgeName, badgeService);
            await RefreshView ();
        }

        public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
        {
            if (segue.Identifier == "GoToMessagingSegue") {
                var controller = (MessagingViewController)segue.DestinationViewController;
                controller.ToUserId = MessageUserId;
                controller.ToUserName = MessageUserName;
                controller.ParentController = this;
            } else if (segue.Identifier == "GoToWriteRecommendationSegue") {
                var controller = (WriteReviewController)segue.DestinationViewController;
                controller.BadgeId = SearchResultDTO.Details.BadgeId;
                controller.UserId = SearchResultDTO.Details.UserId;
                controller.OnSave += RecommendationAdded;
            }


            base.PrepareForSegue (segue, sender);
        }

        async partial void SegmentControlChanged (UISegmentedControl sender)
        {
            switch (sender.SelectedSegment) {
            case 0: {
                    SetProfileDataSource ();
                    break;
                }
            case 1: {
                    await SetRecommendationsDataSourceAsync ();
                    break;
                }
            case 2: {
                    GoToMessaging (profileDataSource.Profile.Details.UserId, profileDataSource.Profile.Details.UserName);
                    break;
                }
            }
        }

        private async void ProfilePictureDoupleTapped ()
        {
            var dto = new BadgeUserDescriptionDTO {
                BadgeName = this.BadgeSummary.BadgeName,
                Delete = this.SearchResultDTO.Details.Favourite,
                EmployeeId = this.SearchResultDTO.Details.UserId,
                Type = BadgeDescriptionTypeEnum.Favourite
            };

            this.FavoriteIcon.Hidden = this.SearchResultDTO.Details.Favourite;
            var result = await profileDataSource.ToggleBadgeUserDescriptionAsync (dto);
            if (result.IsSuccess) {
                this.SearchResultDTO.Details.Favourite = !this.SearchResultDTO.Details.Favourite;
            } else {
                this.FavoriteIcon.Hidden = !this.SearchResultDTO.Details.Favourite;
            }

        }

        partial void CloseButtonTouched (UIBarButtonItem sender)
        {
            this.DismissViewController (true, null);
        }

        private async Task RefreshView ()
        {
            if (loading)
                return;
            loading = true;
            TableView.UserInteractionEnabled = false;
            ActivityIndicator.StartAnimating ();

			var success = await profileDataSource.LoadAsync(SearchResultDTO.Details.UserId);
            if (success) {
                this.NavBar.TopItem.Title = $"{this.BadgeSummary.BadgeName} {profileDataSource.Profile.Details.UserName}";
                this.SubdivisionLabel.Text = profileDataSource.Profile.Details.SubdivisionName?.Trim ();
                if (!string.IsNullOrEmpty (profileDataSource.Profile.Details.ProfilePicturePath)) {
                    var url = NSUrl.FromString (profileDataSource.Profile.Details.ProfilePicturePath);
                    if (url != null) {
                        this.ProfilePicture.SetImage (url);
                    }
                }

                this.DistanceLabel.Text = Math.Round (SearchResultDTO.Details.DistanceFrom ?? 0, 2) + " miles";
                this.LikesLabel.Text = $"{SearchResultDTO.Details.Likes} Likes / {SearchResultDTO.Details.Recommendations} Recommendations";
                this.FavoriteIcon.Hidden = !SearchResultDTO.Details.Favourite;
                this.FirstNameLabel.Text = SearchResultDTO.Details.FirstName;
                this.HourlyRateLabel.Text = profileDataSource.Profile.AdditionalInfo.InfoLabel1;
                this.AgeRangeLabel.Text = profileDataSource.Profile.AdditionalInfo.InfoLabel2;
                SetProfileDataSource ();
                SendMessageButton.Enabled = true;
            } else {
                new UIAlertView ("Error", "Can not load profile data", (IUIAlertViewDelegate)null, "OK").Show ();
                return;
            }

            ActivityIndicator.StopAnimating ();
            ActivityIndicator.RemoveFromSuperview ();
            TableView.UserInteractionEnabled = true;
            TableView.TableHeaderView.Hidden = false;
            loading = false;
        }

        void SetProfileDataSource ()
        {
            TableView.Source = profileDataSource;
            TableView.ReloadData ();
        }

        async Task SetRecommendationsDataSourceAsync ()
        {
            if (recommendationsDataSource == null) {
                viewHelper.ShowOverlay ("Wait...");
				var result = new RecommendationsTableSource(this.badgeService, this.BadgeSummary.BadgeId, BadgeSummary.BadgeName);
				await result.LoadAsync(this.SearchResultDTO.Details.UserId, MehspotAppContext.Instance.AuthManager.AuthInfo.UserId);
                if (recommendationsDataSource == null) {
                    recommendationsDataSource = result;
                    recommendationsDataSource.OnWriteReviewButtonTouched += RecommendationsDataSource_OnWriteReviewButtonTouched;
                    recommendationsDataSource.OnGoToMessaging += GoToMessaging;
                }

                viewHelper.HideOverlay ();
            }

            TableView.Hidden = true;
            TableView.Source = recommendationsDataSource;
            TableView.ReloadData ();
            TableView.Hidden = false;
        }

        void RecommendationAdded (BadgeUserRecommendationDTO recommendation)
        {
            recommendationsDataSource.AddRecommendation (recommendation);
            recommendationsDataSource.HideCreateButton ();
            TableView.ReloadData ();
        }

        void RecommendationsDataSource_OnWriteReviewButtonTouched ()
        {
            PerformSegue ("GoToWriteRecommendationSegue", this);
        }

        void GoToMessaging (string userId, string userName)
        {
            MessageUserId = userId;
            MessageUserName = userName;
            PerformSegue ("GoToMessagingSegue", this);
        }
    }
}