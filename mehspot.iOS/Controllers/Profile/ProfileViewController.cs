using Foundation;
using System;
using UIKit;
using Mehspot.Core.Messaging;
using mehspot.iOS.Core;
using SDWebImage;
using Mehspot.Core.DTO;
using System.Threading.Tasks;
using CoreGraphics;
using Mehspot.Core;
using mehspot.iOS.Extensions;
using mehspot.iOS.Views.Cell;
using mehspot.iOS.Wrappers;
using Mehspot.Core.Contracts.Wrappers;

namespace mehspot.iOS
{
    public partial class ProfileViewController : UIViewController, IUICollectionViewDelegate, IUICollectionViewDataSource
    {
        BadgeSummaryDTO [] badgesList;
        private string[] colorPalette = { "fdfdfd", "fdcd5b", "0091ae", "bcc0c6", "f48f46" };
        private string previousColor;
        private Random random = new Random ();
        private IViewHelper viewHelper;
        private ProfileService profileService;
        private BadgeService badgeService;
        private ApplicationDataStorage applicationDataStorage;

        public ProfileViewController (IntPtr handle) : base (handle)
        {
            applicationDataStorage = new ApplicationDataStorage ();
            profileService = new ProfileService (applicationDataStorage);
            badgeService = new BadgeService (applicationDataStorage);
        }

        public override void ViewDidLoad ()
        {
            viewHelper = new ViewHelper (this.View);
            EditButton.Layer.BorderWidth = 1;
            EditButton.Layer.BorderColor = UIColor.LightGray.CGColor;
            BadgesContainer.RegisterNibForCell (BadgeCollectionViewCell.Nib, BadgeCollectionViewCell.Key);
            BadgesContainer.WeakDataSource = this;
            BadgesContainer.Delegate = this;
            var collectionViewLayout = ((UICollectionViewFlowLayout)BadgesContainer.CollectionViewLayout);

            var itemWidth = this.View.Frame.Width > 320 ? 123 : 106;
            var itemSpacing = this.View.Frame.Width > 320 ? 3 : 1;
            collectionViewLayout.ItemSize = new CGSize (itemWidth, 160);
            collectionViewLayout.MinimumLineSpacing = itemSpacing;
            collectionViewLayout.MinimumInteritemSpacing = itemSpacing;
        }

        public override void ViewDidAppear (bool animated)
        {
            RefreshAsync ();
        }

        void RefreshControl_ValueChanged (object sender, EventArgs e)
        {
            RefreshAsync ();
        }

        public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
        {
            var controller = (EditProfileController)segue.DestinationViewController;
            controller.profile = MehspotAppContext.Instance.DataStorage.Profile;
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
            viewHelper.ShowOverlay ("Refreshing...");
            var profileResult = await profileService.GetProfileAsync ();
            this.EditButton.Enabled = profileResult.IsSuccess;

            if (profileResult.IsSuccess) {
                MehspotAppContext.Instance.DataStorage.Profile = profileResult.Data;
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

            viewHelper.HideOverlay ();
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

        void SetBadges (BadgeSummaryDTO [] badges)
        {
            this.badgesList = badges;
            BadgesContainer.ReloadData ();
        }

        public nint GetItemsCount (UICollectionView collectionView, nint section)
        {
            return badgesList?.Length ?? 0;
        }

        public UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
        {
            var item = badgesList [indexPath.Row];

            var cell = BadgesContainer.DequeueReusableCell (BadgeCollectionViewCell.Key, indexPath) as BadgeCollectionViewCell;
            ConfigureCell (cell, item);
            return cell;
        }

        private void ConfigureCell (BadgeCollectionViewCell cell, BadgeSummaryDTO badge)
        {
            string color;
            do {
                color = colorPalette [random.Next (colorPalette.Length)];
            } while (previousColor == color);
            previousColor = color;
            cell.BackgroundColor = UIColor.Clear.FromHexString (color);
            cell.BadgeImage.Image = UIImage.FromFile ("badges/" + badge.BadgeName.ToLower () + (badge.IsRegistered ? string.Empty : "b"));
            cell.BadgeName.Text = MehspotStrings.ResourceManager.GetString (badge.BadgeName);
            cell.LikesCount.Text = badge.Likes.ToString ();
            cell.RecommendationsCount.Text = badge.Recommendations.ToString ();
            cell.ReferencesCount.Text = badge.References.ToString ();
        }
    }
}