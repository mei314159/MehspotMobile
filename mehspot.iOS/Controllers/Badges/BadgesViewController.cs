using Foundation;
using System;
using UIKit;
using Mehspot.Core.DTO;
using Mehspot.Core;
using mehspot.iOS.Views;
using CoreGraphics;
using Mehspot.Core.Services;
using mehspot.iOS.Extensions;
using Mehspot.Core.Contracts.ViewControllers;
using mehspot.iOS.Wrappers;
using Mehspot.Core.Models;
using Mehspot.Core.Contracts.Wrappers;

namespace mehspot.iOS
{
    public partial class BadgesViewController : UITableViewController, IUITableViewDataSource, IUITableViewDelegate, IBadgesViewController
    {
        private BadgesModel model;

        public BadgesViewController (IntPtr handle) : base (handle)
        {
        }

		public IViewHelper ViewHelper { get; private set; }


		public override void ViewDidLoad ()
        {
			this.ViewHelper = new ViewHelper(this.View);
			model = new BadgesModel(new BadgeService(MehspotAppContext.Instance.DataStorage), this);
			model.LoadingStart += Model_LoadingStart;
			model.LoadingEnd += Model_LoadingEnd;

            TableView.RegisterNibForCellReuse (BadgeItemCell.Nib, BadgeItemCell.Key);
            TableView.WeakDataSource = this;
            TableView.Delegate = this;
            this.RefreshControl.ValueChanged += RefreshControl_ValueChanged;
        }

		void Model_LoadingStart()
		{
			this.RefreshControl.BeginRefreshing();
            this.TableView.SetContentOffset(new CGPoint(0, -this.RefreshControl.Frame.Size.Height), true);
		}

		void Model_LoadingEnd()
		{
			this.TableView.SetContentOffset(CGPoint.Empty, true);
			this.RefreshControl.EndRefreshing();
		}

        public override async void ViewDidAppear (bool animated)
        {
            await model.RefreshAsync(); ;
        }

        public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
        {
            var selectedBadge = model.SelectedBadge;
            if (segue.Identifier == "GoToSearchFilterSegue") {
                var controller = ((SearchBadgeController)segue.DestinationViewController);
                controller.BadgeSummary = selectedBadge;
            } else if (segue.Identifier == "GoToEditBadgeSegue") {
                var controller = ((EditBadgeProfileController)segue.DestinationViewController);
                controller.BadgeId = selectedBadge.BadgeId;
                controller.BadgeName = selectedBadge.BadgeName;
                controller.BadgeIsRegistered = selectedBadge.IsRegistered;
            }

            base.PrepareForSegue (segue, sender);
        }

        async void RefreshControl_ValueChanged (object sender, EventArgs e)
        {
            await model.RefreshAsync();
        }

        public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
        {
            var item = model.Items [indexPath.Row];

            var cell = tableView.DequeueReusableCell (BadgeItemCell.Key, indexPath) as BadgeItemCell;
            ConfigureCell (cell, item);
            return cell;
        }

        public override nint RowsInSection (UITableView tableView, nint section)
        {
            return model.Items?.Length ?? 0;
        }

        private void ConfigureCell (BadgeItemCell cell, BadgeSummaryDTO badge)
        {
            cell.Configure (badge);
            cell.SearchButtonTouch = SearchButton_TouchUpInside;
            cell.BadgeRegisterButtonTouch = BadgeRegisterButton_TouchUpInside;
        }

        public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
        {
            model.SelectRow(indexPath.Row);
            tableView.ReloadRows (new [] { indexPath }, UITableViewRowAnimation.Fade);
        }

        public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
        {
            if (model.IsRowExpanded(indexPath.Row)) {
                return 152;
            } else {
                return 70;
            }
        }

        void SearchButton_TouchUpInside (UIButton button)
        {
            var cell = (BadgeItemCell)button.FindSuperviewOfType (this.View, typeof (BadgeItemCell));
            var indexPath = this.TableView.IndexPathForCell (cell);
            this.model.SelectRow(indexPath.Row);
            PerformSegue ("GoToSearchFilterSegue", this);
        }

        void BadgeRegisterButton_TouchUpInside (UIButton button)
        {
            var cell = (BadgeItemCell)button.FindSuperviewOfType (this.View, typeof (BadgeItemCell));
            var indexPath = this.TableView.IndexPathForCell (cell);
            this.model.SelectRow(indexPath.Row);
            PerformSegue ("GoToEditBadgeSegue", this);
        }

        public void DisplayBadges()
        {
            TableView.ReloadData();
        }
    }
}