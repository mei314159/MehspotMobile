using Foundation;
using System;
using UIKit;
using Mehspot.Core.Messaging;
using mehspot.iOS.Core;
using Mehspot.Core.Contracts.Wrappers;
using mehspot.iOS.Wrappers;
using SDWebImage;
using Mehspot.Core.DTO;
using System.Threading.Tasks;
using CoreGraphics;
using System.Linq;
using Mehspot.Core;

namespace mehspot.iOS
{
    public partial class ProfileViewController : UIViewController
    {
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
            this.MainScrollView.AddSubview (refreshControl);
            refreshControl.ValueChanged += RefreshControl_ValueChanged;
        }

        public override void ViewDidAppear (bool animated)
        {
            if (!refreshControl.Refreshing) {
                this.refreshControl.BeginRefreshing ();
                RefreshAsync ();
            }
        }

        void RefreshControl_ValueChanged (object sender, EventArgs e)
        {
            RefreshAsync ();
        }

        async Task RefreshAsync ()
        {
            var profileResult = await profileService.GetProfileAsync ();
            if (profileResult.IsSuccess) {
                SetFields (profileResult.Data);
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

        private void SetFields (ProfileDto profile)
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
                badgeItemView.BadgeImage.Image = UIImage.FromFile ("badges/" + badge.BadgeName.ToLower());
                badgeItemView.BadgeName.Text = MehspotStrings.ResourceManager.GetString (badge.BadgeName);
                badgeItemView.LikesCount.Text = badge.Likes.ToString();
                badgeItemView.RecommendationsCount.Text = badge.Recommendations.ToString();
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