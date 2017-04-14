using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Foundation;
using mehspot.iOS.Views.Cell;
using Mehspot.Core.DTO.Search;
using Mehspot.Core.Services;
using MehSpot.Models.ViewModels;
using UIKit;

namespace mehspot.iOS.Controllers.Badges.DataSources.Search
{
    public delegate void SendMessageButtonTouched (UIButton obj, ISearchResultDTO item);
    public delegate void ViewProfileButtonTouched (UIButton obj, ISearchResultDTO item);

    public class SearchResultTableSource : UITableViewSource
    {
        private volatile bool loading;
        private const int pageSize = 20;
        private List<NSIndexPath> expandedPaths = new List<NSIndexPath> ();
        BadgeService badgeService;
        ISearchFilterDTO filter;


        public SearchResultTableSource (BadgeService badgeService, ISearchFilterDTO filter)
        {
            this.badgeService = badgeService;
            this.filter = filter;
        }

        public List<ISearchResultDTO> Items { get; } = new List<ISearchResultDTO> ();


        public event SendMessageButtonTouched SendMessageButtonTouched;
        public event ViewProfileButtonTouched ViewProfileButtonTouched;

        public event Action LoadingStarted;
        public event Action LoadingEnded;

        public async Task LoadDataAsync (UITableView tableView, bool refresh = false)
        {
            var skip = refresh ? 0 : (this.Items?.Count ?? 0);

            var result = await badgeService.Search<BabysitterSearchResultDTO> (this.filter, skip, pageSize);
            if (result.IsSuccess) {
                if (refresh) {
                    this.Items.Clear ();
                }
                this.Items.AddRange (result.Data);
                tableView.ReloadData ();
            }
        }

        public void Clear (UITableView tableView)
        {
            Items.Clear ();
            tableView.ReloadData ();
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

        public override nint RowsInSection (UITableView tableview, nint section)
        {
            return Items?.Count ?? 0;
        }

        public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
        {
            if (expandedPaths.Contains (indexPath)) {
                return 125;
            } else {
                return 84;
            }
        }

        private async void LoadMoreAsync (UITableView tableView)
        {
            if (!loading) {
                this.loading = true;
                LoadingStarted?.Invoke ();
                await this.LoadDataAsync (tableView);
                LoadingEnded?.Invoke ();
                loading = false;
            }
        }

        public override void Scrolled (UIScrollView scrollView)
        {
            var currentOffset = scrollView.ContentOffset.Y;
            var maximumOffset = scrollView.ContentSize.Height - scrollView.Frame.Size.Height;
            var deltaOffset = maximumOffset - currentOffset;

            if (currentOffset > 0 && deltaOffset <= 0) {
                var uITableView = (UITableView)scrollView;
                LoadMoreAsync (uITableView);
            }
        }

        public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
        {
            var item = Items [indexPath.Row];

            var cell = tableView.DequeueReusableCell (SearchResultsCell.Key, indexPath);
            ConfigureCell (cell as SearchResultsCell, item);
            return cell;
        }

        private void ConfigureCell (SearchResultsCell cell, ISearchResultDTO item)
        {
            cell.Configure (item);

            cell.SendMessageButtonAction = (obj) => SendMessageButtonTouched (obj, item);
            cell.ViewProfileButtonAction = (obj) => ViewProfileButtonTouched (obj, item);
        }
    }
}
