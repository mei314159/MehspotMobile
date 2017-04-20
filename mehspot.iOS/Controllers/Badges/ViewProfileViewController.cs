using Foundation;
using System;
using UIKit;
using Mehspot.Core;
using System.Threading.Tasks;
using SDWebImage;
using MehSpot.Models.ViewModels;
using Mehspot.Core.Services;
using Mehspot.Core.DTO.Badges;
using mehspot.iOS.Controllers.Badges.DataSources.Search;
using mehspot.iOS.Controllers.Badges.BadgeProfileDataSource;

namespace mehspot.iOS
{
    public partial class ViewProfileViewController : UIViewController
    {
        private volatile bool loading;
        public ISearchResultDTO SearchResultDTO;
        public SearchModel SearchModel;
        ViewBadgeProfileTableSource source;

        public ViewProfileViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad ()
        {
            SendMessageButton.Layer.BorderWidth = 1;
            SendMessageButton.Layer.BorderColor = SendMessageButton.TitleColor (UIControlState.Normal).CGColor;
            this.NavBar.TopItem.Title =
                (MehspotResources.ResourceManager.GetString (SearchModel.BadgeName) ?? SearchModel.BadgeName) + " Profile";

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
                controller.ToUserId = source.Profile.Details.UserId;
                controller.ToUserName = source.Profile.Details.UserName;
                controller.ParentController = this;
            }

            base.PrepareForSegue (segue, sender);
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
            var result = await source.ToggleBadgeUserDescriptionAsync (dto);
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
            source = await SearchModel.GetViewProfileTableSource (this.SearchResultDTO.Details.UserId);
            if (source != null) {
                TableView.Source = source;

                this.NavBar.TopItem.Title = $"{this.SearchModel.BadgeName} {source.Profile.Details.UserName}";
                this.SubdivisionLabel.Text = source.Profile.Details.SubdivisionName.Trim ();
                if (!string.IsNullOrEmpty (source.Profile.Details.ProfilePicturePath)) {
                    var url = NSUrl.FromString (source.Profile.Details.ProfilePicturePath);
                    if (url != null) {
                        this.ProfilePicture.SetImage (url);
                    }
                }

                this.DistanceLabel.Text = Math.Round (SearchResultDTO.Details.DistanceFrom ?? 0, 2) + " miles";
                this.LikesLabel.Text = $"{SearchResultDTO.Details.Likes} Likes / {SearchResultDTO.Details.Recommendations} Recommendations";
                this.FavoriteIcon.Hidden = !SearchResultDTO.Details.Favourite;
                this.FirstNameLabel.Text = SearchResultDTO.Details.FirstName;
                this.HourlyRateLabel.Text = source.Profile.AdditionalInfo.InfoLabel1;
                this.AgeRangeLabel.Text = source.Profile.AdditionalInfo.InfoLabel2;
                TableView.ReloadData ();
                SendMessageButton.Enabled = true;
            } else {
                new UIAlertView ("Error", "Can not load profile data", null, "OK").Show ();
                return;
            }

            ActivityIndicator.StopAnimating ();
            ActivityIndicator.RemoveFromSuperview ();
            TableView.UserInteractionEnabled = true;
            loading = false;
        }

        void InitializeTable ()
        {

        }
    }
}