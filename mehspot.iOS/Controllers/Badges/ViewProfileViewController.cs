using Foundation;
using System;
using UIKit;
using Mehspot.Core.Messaging;
using Mehspot.Core;
using System.Threading.Tasks;
using SDWebImage;
using MehSpot.Models.ViewModels;
using mehspot.iOS.Controllers.Badges.BadgeProfileDataSource;
using MehSpot.Web.ViewModels;

namespace mehspot.iOS
{
    public partial class ViewProfileViewController : UIViewController
    {
        private volatile bool loading;
        private BadgeService badgeService;
        private BadgeProfileDTO<BabysitterProfileDTO> profile;
        public BabysitterSearchResultDTO SearchResultDTO;

        public ViewProfileViewController (IntPtr handle) : base (handle)
        {
        }

        public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
        {
            if (segue.Identifier == "GoToMessagingSegue") {
                var controller = (MessagingViewController)segue.DestinationViewController;
                controller.ToUserId = profile.Details.UserId;
                controller.ToUserName = profile.Details.UserName;
                controller.ParentController = this;
            }

            base.PrepareForSegue (segue, sender);
        }

        public override void ViewDidLoad ()
        {
            SendMessageButton.Layer.BorderWidth = 1;
            SendMessageButton.Layer.BorderColor = SendMessageButton.TitleColor(UIControlState.Normal).CGColor;
            this.NavBar.TopItem.Title = BadgeService.BadgeNames.Babysitter + " Profile";
            TableView.InsertSections (new NSIndexSet (), UITableViewRowAnimation.None);
            TableView.InsertSections (new NSIndexSet (), UITableViewRowAnimation.None);
            TableView.TableHeaderView.Hidden = true;
            TableView.TableFooterView = new UIView ();
            badgeService = new BadgeService (MehspotAppContext.Instance.DataStorage);
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

        async void ProfilePictureDoupleTapped ()
        {
            var dto = new BadgeUserDescriptionDTO {
                BadgeName = BadgeService.BadgeNames.Babysitter,
                Delete = this.SearchResultDTO.Details.Favourite,
                EmployeeId = this.SearchResultDTO.Details.UserId,
                Type = BadgeDescriptionTypeEnum.Favourite
            };

            this.FavoriteIcon.Hidden = this.SearchResultDTO.Details.Favourite;
            var result = await badgeService.ToggleBadgeUserDescriptionAsync (dto);
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

            var result = await badgeService.GetBadgeProfileAsync (BadgeService.BadgeNames.Babysitter, this.SearchResultDTO.Details.UserId);

            if (result.IsSuccess) {
                profile = result.Data;
                InitializeTable ();
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
            this.NavBar.TopItem.Title =  $"{BadgeService.BadgeNames.Babysitter} {profile.Details.UserName}";
            this.FirstNameLabel.Text = profile.Values.FirstName;
            this.SubdivisionLabel.Text = profile.Details.SubdivisionName.Trim ();
            this.HourlyRateLabel.Text = $"${profile.Values.HourlyRate}/hr";
            this.AgeRangeLabel.Text = profile.Values.AgeRange;
            this.DistanceLabel.Text = Math.Round (SearchResultDTO.Details.DistanceFrom ?? 0, 2) + " miles";
            this.LikesLabel.Text = $"{SearchResultDTO.Details.Likes} Likes / {SearchResultDTO.Details.Recommendations} Recommendations";
            this.FavoriteIcon.Hidden = !SearchResultDTO.Details.Favourite;
            if (!string.IsNullOrEmpty (profile.Details.ProfilePicturePath)) {
                var url = NSUrl.FromString (profile.Details.ProfilePicturePath);
                if (url != null) {
                    this.ProfilePicture.SetImage (url);
                }
            }

            TableView.Source = new BabysitterDataSource (profile, badgeService);
            TableView.ReloadData ();
        }
    }
}