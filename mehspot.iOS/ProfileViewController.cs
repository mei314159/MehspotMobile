using Foundation;
using System;
using UIKit;
using Mehspot.Core.Messaging;
using mehspot.iOS.Core;
using SDWebImage;
using Mehspot.Core.DTO;
using System.Threading.Tasks;
using CoreGraphics;
using System.Linq;
using Mehspot.Core;
using mehspot.iOS.Extensions;

namespace mehspot.iOS
{
    public partial class ProfileViewController : UIViewController
    {
        EditProfileDto profile;

        private ProfileService profileService;
        private BadgeService badgeService;
        private ApplicationDataStorage applicationDataStorage;
        private readonly UIRefreshControl refreshControl;

        public ProfileViewController (IntPtr handle) : base (handle)
        {
            applicationDataStorage = new ApplicationDataStorage ();
            profileService = new ProfileService (applicationDataStorage);
            badgeService = new BadgeService (applicationDataStorage);
            refreshControl = new UIRefreshControl ();
        }

        public override void ViewDidLoad ()
        {
            EditButton.Layer.BorderWidth = 1;
            EditButton.Layer.BorderColor = UIColor.LightGray.CGColor;
            this.MainScrollView.AddSubview (refreshControl);
            refreshControl.ValueChanged += RefreshControl_ValueChanged;
        }

        public override void ViewDidAppear (bool animated)
        {
            if (!refreshControl.Refreshing) {
                this.refreshControl.BeginRefreshing ();
                this.MainScrollView.SetContentOffset (new CGPoint (0, -refreshControl.Frame.Size.Height), true);
                RefreshAsync ();
            }
        }

        void RefreshControl_ValueChanged (object sender, EventArgs e)
        {
            RefreshAsync ();
        }

        public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
        {
            var controller = (EditProfileController)segue.DestinationViewController;
            controller.profile = this.profile;
            base.PrepareForSegue (segue, sender);
        }

        partial void SignoutButtonTouched (UIBarButtonItem sender)
        {
            UIAlertView alert = new UIAlertView (
                                            "Sign Out",
                                            "Are you sure you want to sign out?",
                                            null,
                                            "Cancel",
                                            new string [] { "Yes, I do" });
            alert.Clicked += (object s, UIButtonEventArgs e) => {
                if (e.ButtonIndex != alert.CancelButtonIndex) {
                    MehspotAppContext.Instance.AuthManager.SignOut ();
                    MehspotAppContext.Instance.DisconnectSignalR ();

                    var targetViewController = UIStoryboard.FromName ("Main", null).InstantiateViewController ("LoginViewController");

                    targetViewController.SwapController (UIViewAnimationOptions.TransitionFlipFromRight);
                }
            };
            alert.Show ();
        }

        async Task RefreshAsync ()
        {
            var profileResult = await profileService.GetProfileAsync ();
            this.EditButton.Enabled = profileResult.IsSuccess;
            if (profileResult.IsSuccess) {
                this.profile = profileResult.Data;
                SetFields (profileResult.Data);
                ProfileInfoContainer.Hidden = false;
                var badgesResult = await badgeService.GetBadgesSummaryAsync ();
                if (badgesResult.IsSuccess) {
                    SetBadges (badgesResult.Data);
                } else {
                    new UIAlertView ("Error", "Can not load badges data", null, "OK").Show ();
                }
            } else {
                new UIAlertView ("Error", "Can not load profile data", null, "OK").Show ();
            }
            refreshControl.EndRefreshing ();
        }

        private void SetFields (EditProfileDto profile)
        {
            this.UserNameLabel.Text = profile.UserName;
            this.FullName.Text = $"{profile.FirstName} {profile.LastName}".Trim (' ');
            if (!string.IsNullOrEmpty (profile.ProfilePicturePath)) {
                var url = NSUrl.FromString (profile.ProfilePicturePath);
                if (url != null) {
                    this.ProfilePicture.SetImage (url);
                }
            }
        }

        void SetBadges (BadgeSummaryDto [] badges)
        {
            foreach (var subview in this.BadgesContainer.Subviews) {
                subview.RemoveFromSuperview ();
                subview.Dispose ();
            }

            for (int i = 0; i < badges.Length; i++) {
                var badge = badges [i];
                var badgeItemView = BadgeItemView.Create ();
                badgeItemView.BadgeImage.Image = UIImage.FromFile ("badges/" + badge.BadgeName.ToLower () + (badge.IsRegistered ? string.Empty : "b"));
                badgeItemView.BadgeName.Text = MehspotStrings.ResourceManager.GetString (badge.BadgeName);
                badgeItemView.LikesCount.Text = badge.Likes.ToString ();
                badgeItemView.RecommendationsCount.Text = badge.Recommendations.ToString ();
                badgeItemView.ReferencesCount.Text = badge.References.ToString ();
                badgeItemView.Frame = new CGRect (0, i * badgeItemView.Frame.Height, BadgesContainer.Frame.Width, badgeItemView.Frame.Height);
                this.BadgesContainer.AddSubview (badgeItemView);
            }
            var badgesContainerHeight = this.BadgesContainer.Subviews.Length > 0 ? this.BadgesContainer.Subviews.Max (a => a.Frame.Height + a.Frame.Y) : this.BadgesContainer.Frame.Height;
            this.BadgesContainer.Frame = new CGRect (this.BadgesContainer.Frame.Location, new CGSize (this.BadgesContainer.Frame.Width, badgesContainerHeight));
            this.MainScrollView.ContentSize = new CGSize (this.MainScrollView.Frame.Width, this.BadgesContainer.Frame.Y + this.BadgesContainer.Frame.Height);
        }
    }
}