using Foundation;
using System;
using UIKit;
using Mehspot.Core;
using System.Threading.Tasks;
using SDWebImage;
using MehSpot.Models.ViewModels;
using Mehspot.Core.DTO.Badges;
using mehspot.iOS.Controllers.Badges.DataSources.Search;
using mehspot.iOS.Controllers.Badges.BadgeProfileDataSource;
using mehspot.iOS.Views.Cell;

namespace mehspot.iOS
{
    public partial class ViewProfileViewController : UIViewController
    {
        private volatile bool loading;
        public ISearchResultDTO SearchResultDTO;
        public SearchModel SearchModel;
        ViewBadgeProfileTableSource profileDataSource;

        public ViewProfileViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad ()
        {
            this.NavBar.TopItem.Title =
                (MehspotResources.ResourceManager.GetString (SearchModel.BadgeName) ?? SearchModel.BadgeName) + " Profile";

            TableView.RegisterNibForCellReuse (RecommendationCell.Nib, RecommendationCell.Key);
            TableView.TableHeaderView.Hidden = true;
            TableView.TableFooterView = new UIView ();
            this.ProfilePicture.UserInteractionEnabled = true;
            var tapGestureRecognizer = new UITapGestureRecognizer (ProfilePictureDoupleTapped);
            tapGestureRecognizer.NumberOfTapsRequired = 2;
            this.ProfilePicture.AddGestureRecognizer (tapGestureRecognizer);
        }

        public override async void ViewDidAppear (bool animated)
        {
            await RefreshView ();
            TableView.TableHeaderView.Hidden = false;
        }

        public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
        {
            if (segue.Identifier == "GoToMessagingSegue") {
                var controller = (MessagingViewController)segue.DestinationViewController;
                controller.ToUserId = profileDataSource.Profile.Details.UserId;
                controller.ToUserName = profileDataSource.Profile.Details.UserName;
                controller.ParentController = this;
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
                    PerformSegue ("GoToMessagingSegue", this);
                    break;
                }
            }
        }

        private async void ProfilePictureDoupleTapped ()
        {
            var dto = new BadgeUserDescriptionDTO {
                BadgeName = this.SearchModel.BadgeName,
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
            profileDataSource = await SearchModel.GetViewProfileTableSource (this.SearchResultDTO.Details.UserId);
            if (profileDataSource != null) {
                this.NavBar.TopItem.Title = $"{this.SearchModel.BadgeName} {profileDataSource.Profile.Details.UserName}";
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
            loading = false;
        }

        void SetProfileDataSource ()
        {
            TableView.Source = profileDataSource;
            TableView.ReloadData ();
        }

        async Task SetRecommendationsDataSourceAsync ()
        {
            var recommendationsDataSource = await this.SearchModel.GetRecommendationsTableSource (this.SearchResultDTO.Details.UserId);
            TableView.Source = recommendationsDataSource;
            TableView.ReloadData ();
        }

        void InitializeTable ()
        {

        }
    }
}