using Foundation;
using System;
using UIKit;
using Mehspot.Core.DTO.Search;
using Mehspot.Core.Messaging;
using Mehspot.Core;
using System.Threading.Tasks;
using CoreGraphics;
using MehSpot.Models.ViewModels;

namespace mehspot.iOS
{
    public partial class SearchResultsViewController : UITableViewController
    {
        private volatile bool loading;
        private volatile bool viewWasInitialized;
        private BabysitterDetailsDTO [] items;
        private UIRefreshControl refreshControl;
        private BadgeService badgeService;

        private const int pageSize = 20;

        public ISearchFilterDTO Filter;
        public string BadgeName;

        public SearchResultsViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad ()
        {
            badgeService = new BadgeService (MehspotAppContext.Instance.DataStorage);
            this.TableView.WeakDataSource = this;
            this.TableView.Delegate = this;
            refreshControl = new UIRefreshControl ();
            refreshControl.ValueChanged += RefreshControl_ValueChanged;
            this.TableView.AddSubview (refreshControl);
        }
        public override async void ViewDidAppear (bool animated)
        {
            if (!viewWasInitialized) {
                await LoadMessageBoardAsync ();
                viewWasInitialized = true;
            }
        }

        public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
        {
            var item = items [indexPath.Row];

            var cell = TableView.DequeueReusableCell (MessageBoardCell.Key, indexPath);
            //ConfigureCell (cell as MessageBoardCell, item);
            return cell;
        }

        //private void ConfigureCell (MessageBoardCell cell, SearchResultDTO item)
        //{
        //    if (!string.IsNullOrEmpty (item.WithUser.ProfilePicturePath)) {

        //        var url = NSUrl.FromString (item.WithUser.ProfilePicturePath);
        //        if (url != null) {
        //            cell.ProfilePicture.SetImage (url);
        //        }
        //    }
        //    cell.UserName.Text = item.WithUser.UserName;
        //    cell.Message.Text = item.LastMessage;
        //    cell.CountLabel.Hidden = item.UnreadMessagesCount == 0;
        //    cell.CountLabel.Text = item.UnreadMessagesCount.ToString ();
        //}


        public override nint RowsInSection (UITableView tableView, nint section)
        {
            return items?.Length ?? 0;
        }

        private async void RefreshControl_ValueChanged (object sender, EventArgs e)
        {
            await LoadMessageBoardAsync ();
        }

        private async Task LoadMessageBoardAsync ()
        {
            if (loading)
                return;
            loading = true;
            refreshControl.BeginRefreshing ();
            this.TableView.SetContentOffset (new CGPoint (0, -refreshControl.Frame.Size.Height), true);
            var result = await badgeService.Search<BabysitterDetailsDTO> (this.Filter, this.BadgeName, items?.Length ?? 0, pageSize);
            if (result.IsSuccess) {
                this.items = result.Data;
                TableView.ReloadData ();
            }

            refreshControl.EndRefreshing ();
            loading = false;
        }
    }
}