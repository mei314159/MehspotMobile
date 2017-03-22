using Foundation;
using System;
using UIKit;
using Mehspot.Core.Messaging;
using Mehspot.Core;
using System.Threading.Tasks;
using SDWebImage;
using System.Linq;
using MehSpot.Models.ViewModels;
using mehspot.iOS.Controllers.Badges.BadgeProfileDataSource;

namespace mehspot.iOS
{
    public partial class ViewProfileViewController : UIViewController
    {
        volatile bool loading;
        private BadgeService badgeService;
        BadgeProfileDTO<BabysitterProfileDTO> profile;

        public ViewProfileViewController (IntPtr handle) : base (handle)
        {
        }

        public string UserId { get; set; }

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
            SendMessageButton.Layer.BorderColor = UIColor.LightGray.CGColor;
            this.NavBar.TopItem.Title = BadgeService.BadgeNames.Babysitter + " Profile";
            TableView.InsertSections (new NSIndexSet (), UITableViewRowAnimation.None);
            TableView.InsertSections (new NSIndexSet (), UITableViewRowAnimation.None);
            TableView.TableHeaderView.Hidden = true;
            TableView.TableFooterView = new UIView ();
            badgeService = new BadgeService (MehspotAppContext.Instance.DataStorage);
        }

        public override async void ViewDidAppear (bool animated)
        {
            await RefreshView ();
            TableView.TableHeaderView.Hidden = false;
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

            var result = await badgeService.GetBadgeProfileAsync (BadgeService.BadgeNames.Babysitter, this.UserId);

            if (result.IsSuccess) {
                profile = result.Data;
                InitializeTable ();
                SendMessageButton.Enabled = true;
            } else {
                new UIAlertView ("Error", "Can not load profile data", null, "OK").Show ();
                return;
            }

            TableView.UserInteractionEnabled = true;
            loading = false;
        }

        void InitializeTable ()
        {
            this.UserNameLabel.Text = profile.Details.UserName;
            this.FirstName.Text = profile.Details.FirstName;

            if (!string.IsNullOrEmpty (profile.Details.ProfilePicturePath)) {
                var url = NSUrl.FromString (profile.Details.ProfilePicturePath);
                if (url != null) {
                    this.ProfilePicture.SetImage (url);
                }
            }

            TableView.Source = new BabysitterDataSource (profile);
            TableView.ReloadData ();
        }
    }
}