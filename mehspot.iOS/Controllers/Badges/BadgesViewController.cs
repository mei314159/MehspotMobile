using Foundation;
using System;
using UIKit;
using Mehspot.Core.DTO;
using Mehspot.Core.Messaging;
using Mehspot.Core;
using System.Threading.Tasks;
using mehspot.iOS.Views;
using CoreGraphics;
using System.Collections.Generic;

namespace mehspot.iOS
{
    public partial class BadgesViewController : UITableViewController, IUITableViewDataSource, IUITableViewDelegate
    {
        volatile bool loading;
        volatile bool dataLoaded;
        private List<NSIndexPath> expandedPaths = new List<NSIndexPath> ();
        private BadgeSummaryDTO [] badgesList;
        private BadgeService badgeService;

        public BadgesViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad ()
        {
            badgeService = new BadgeService (MehspotAppContext.Instance.DataStorage);
            TableView.RegisterNibForCellReuse (BadgeItemCell.Nib, BadgeItemCell.Key);
            TableView.WeakDataSource = this;
            TableView.Delegate = this;
            RefreshControl.ValueChanged += RefreshControl_ValueChanged;
        }

        public override async void ViewDidAppear (bool animated)
        {
            if (!dataLoaded)
                await RefreshAsync ();
        }

        async void RefreshControl_ValueChanged (object sender, EventArgs e)
        {
            await RefreshAsync ();
        }

        async Task RefreshAsync ()
        {
            if (loading)
                return;
            loading = true;
            RefreshControl.BeginRefreshing ();
            TableView.SetContentOffset (new CGPoint (0, -this.TableView.RefreshControl.Frame.Size.Height), true);
            var badgesResult = await badgeService.GetBadgesSummaryAsync ();
            if (badgesResult.IsSuccess) {
                SetBadges (badgesResult.Data);
            } else {
                new UIAlertView ("Error", "Can not load badges", null, "OK").Show ();
            }

            RefreshControl.EndRefreshing ();
            dataLoaded = badgesResult.IsSuccess;
            loading = false;
        }

        void SetBadges (BadgeSummaryDTO [] badges)
        {
            this.badgesList = badges;
            TableView.ReloadData ();
        }

        public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
        {
            var item = badgesList [indexPath.Row];

            var cell = tableView.DequeueReusableCell (BadgeItemCell.Key, indexPath) as BadgeItemCell;
            ConfigureCell (cell, item);
            return cell;
        }

        public override nint RowsInSection (UITableView tableView, nint section)
        {
            return badgesList?.Length ?? 0;
        }

        private void ConfigureCell (BadgeItemCell cell, BadgeSummaryDTO badge)
        {
            cell.BadgePicture.Image = UIImage.FromFile ("badges/" + badge.BadgeName.ToLower () + (badge.IsRegistered ? string.Empty : "b"));
            cell.BadgeName.Text = MehspotStrings.ResourceManager.GetString (badge.BadgeName);
            cell.BadgeSummary = badge;
            cell.SearchButton.Layer.BorderWidth = cell.BadgeRegisterButton.Layer.BorderWidth = 1;
            cell.SearchButton.Layer.BorderColor = cell.SearchButton.TitleColor (UIControlState.Normal).CGColor;
            cell.BadgeRegisterButton.Layer.BorderColor = cell.BadgeRegisterButton.TitleColor(UIControlState.Normal).CGColor;
            cell.BadgeRegisterButton.SetTitle (badge.IsRegistered ? "Update" : "Register", UIControlState.Normal);
            cell.BadgeDescription.Text = MehspotStrings.ResourceManager.GetString (badge.BadgeName + "_Description");
            cell.LikesCount.Text = badge.Likes.ToString ();
            cell.RecommendationsCount.Text = badge.Recommendations.ToString ();
            cell.ReferencesCount.Text = badge.References.ToString ();
            cell.SearchButtonTouch = SearchButton_TouchUpInside;
        }

        public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
        {
            if (expandedPaths.Contains (indexPath)) {
                expandedPaths.Remove (indexPath);
            } else {
                expandedPaths.Add (indexPath);
            }
            tableView.ReloadRows (new [] { indexPath }, UITableViewRowAnimation.Fade);
        }

        public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
        {
            if (expandedPaths.Contains (indexPath)) {
                return 152;
            } else {
                return 70;
            }
        }

        void SearchButton_TouchUpInside (UIButton button)
        {
            PerformSegue ("GoToSearchFilterSegue", this);
        }
    }
}