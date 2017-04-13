using Foundation;
using System;
using UIKit;
using Mehspot.Core;
using System.Threading.Tasks;
using CoreGraphics;
using MehSpot.Models.ViewModels;
using mehspot.iOS.Views.Cell;
using System.Collections.Generic;
using Mehspot.Core.Services;
using mehspot.iOS.Controllers.Badges.DataSources.Search;

namespace mehspot.iOS
{
    public partial class SearchResultsViewController : UITableViewController
    {
        private volatile bool loading;
        private volatile bool viewWasInitialized;
        private List<NSIndexPath> expandedPaths = new List<NSIndexPath> ();
        private List<ISearchResultDTO> items = new List<ISearchResultDTO> ();
        private BadgeService badgeService;
        private ISearchResultDTO SelectedItem;

        private const int pageSize = 20;

        public SearchModel SearchModel;
        public string BadgeName;
        public int BadgeId;


        public SearchResultsViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad ()
        {
            badgeService = new BadgeService (MehspotAppContext.Instance.DataStorage);
            this.TableView.RegisterNibForCellReuse (SearchResultsCell.Nib, SearchResultsCell.Key);
            this.TableView.WeakDataSource = this;
            this.TableView.Delegate = this;
            this.RefreshControl.ValueChanged += RefreshControl_ValueChanged;
            this.TableView.TableFooterView.Hidden = true;
        }

        public override async void ViewDidAppear (bool animated)
        {
            if (!viewWasInitialized) {
                await RefreshResultsAsync ();
                viewWasInitialized = true;
            }
        }

        [Action ("UnwindToSearchResultsViewController:")]
        public void UnwindToSearchResultsViewController (UIStoryboardSegue segue)
        {
        }

        [Export ("scrollViewDidScroll:")]
        public void Scrolled (UIScrollView scrollView)
        {
            var currentOffset = scrollView.ContentOffset.Y;
            var maximumOffset = scrollView.ContentSize.Height - scrollView.Frame.Size.Height;
            var deltaOffset = maximumOffset - currentOffset;

            if (deltaOffset <= 0) {
                LoadMoreAsync ();
            }
        }

        private async void LoadMoreAsync ()
        {
            if (!loading) {
                this.loading = true;
                this.ActivityIndicator.StartAnimating ();
                this.TableView.TableFooterView.Hidden = false;

                await LoadDataAsync ();
                this.ActivityIndicator.StopAnimating ();
                this.TableView.TableFooterView.Hidden = true;
                loading = false;
            }
        }

        public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
        {
            if (segue.Identifier == "GoToMessagingSegue") {
                var controller = (MessagingViewController)segue.DestinationViewController;
                controller.ToUserName = this.SelectedItem.Details.FirstName;
                controller.ToUserId = this.SelectedItem.Details.UserId;
            } else if (segue.Identifier == "ViewProfileSegue") {
                var controller = (ViewProfileViewController)segue.DestinationViewController;
                controller.SearchResultDTO = this.SelectedItem;
                controller.BadgeId = this.BadgeId;
                controller.BadgeName = this.BadgeName;
            }

            base.PrepareForSegue (segue, sender);
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
                return 125;
            } else {
                return 84;
            }
        }

        public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
        {
            var item = items [indexPath.Row];

            var cell = TableView.DequeueReusableCell (SearchResultsCell.Key, indexPath);
            ConfigureCell (cell as SearchResultsCell, item);
            return cell;
        }

        private void ConfigureCell (SearchResultsCell cell, ISearchResultDTO item)
        {
            cell.Configure (item);

            cell.SendMessageButtonAction = (obj) => SendMessageButtonTouched (obj, item);
            cell.ViewProfileButtonAction = (obj) => ViewProfileButtonTouched (obj, item);
        }

        void SendMessageButtonTouched (UIButton obj, ISearchResultDTO dto)
        {
            this.SelectedItem = dto;
            PerformSegue ("GoToMessagingSegue", this);
        }

        void ViewProfileButtonTouched (UIButton obj, ISearchResultDTO dto)
        {
            this.SelectedItem = dto;
            PerformSegue ("ViewProfileSegue", this);
        }

        public override nint RowsInSection (UITableView tableView, nint section)
        {
            return items?.Count ?? 0;
        }

        private async void RefreshControl_ValueChanged (object sender, EventArgs e)
        {
            await RefreshResultsAsync ();
        }

        private async Task RefreshResultsAsync ()
        {
            if (loading)
                return;
            loading = true;
            this.RefreshControl.BeginRefreshing ();

            this.TableView.SetContentOffset (new CGPoint (0, -this.TableView.RefreshControl.Frame.Size.Height), true);
            await LoadDataAsync (true);
            this.RefreshControl.EndRefreshing ();
            loading = false;
        }

        private async Task LoadDataAsync (bool refresh = false)
        {
            var skip = refresh ? 0 : (items?.Count ?? 0);
            var result = await badgeService.Search<BabysitterSearchResultDTO> (this.SearchModel.Filter, this.BadgeId, skip, pageSize);
            if (result.IsSuccess) {
                if (refresh) {
                    this.items.Clear ();
                }
                this.items.AddRange (result.Data);
                TableView.ReloadData ();
            }
        }
    }
}