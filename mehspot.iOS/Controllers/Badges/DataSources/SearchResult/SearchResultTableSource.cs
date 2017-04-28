using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Foundation;
using mehspot.iOS.Views.Cell;
using Mehspot.Core.DTO;
using Mehspot.Core.DTO.Search;
using Mehspot.Core.Services;
using MehSpot.Models.ViewModels;
using UIKit;

namespace mehspot.iOS.Controllers.Badges.DataSources.Search
{
    public delegate void SendMessageButtonTouched (UIButton obj, ISearchResultDTO item);
    public delegate void ViewProfileButtonTouched (UIButton obj, ISearchResultDTO item);
    public delegate void OnRegisterButtonTouched (int requiredBadgeId);
    public delegate void OnLoadingError (Result result);
    public class SearchResultTableSource : UITableViewSource
    {
        private const int pageSize = 20;
        private const int limitedResultsCount = 5;
        private volatile bool loading;
        private readonly BadgeService badgeService;
        private readonly ISearchFilterDTO filter;
        private readonly BadgeSummaryDTO searchBadge;
        private List<NSIndexPath> expandedPaths = new List<NSIndexPath> ();

        public readonly Type ResultType;

        public SearchResultTableSource (BadgeService badgeService, ISearchFilterDTO filter, Type resultType, BadgeSummaryDTO searchBadge)
        {
            this.searchBadge = searchBadge;
            this.badgeService = badgeService;
            this.filter = filter;
            this.ResultType = resultType;
        }

        public List<ISearchResultDTO> Items { get; } = new List<ISearchResultDTO> ();


        public event SendMessageButtonTouched SendMessageButtonTouched;
        public event ViewProfileButtonTouched ViewProfileButtonTouched;
        public event OnRegisterButtonTouched RegisterButtonTouched;
        public event OnLoadingError OnLoadingError;

        public event Action LoadingMoreStarted;
        public event Action LoadingMoreEnded;

        public async Task LoadDataAsync (UITableView tableView, bool refresh = false)
        {
            var skip = refresh ? 0 : (this.Items?.Count ?? 0);
            var result = await badgeService.Search (this.filter, skip, pageSize, this.ResultType);
            if (result.IsSuccess) {
                if (refresh) {
                    this.Items.Clear ();
                }

                this.Items.AddRange (result.Data);
                tableView.ReloadData ();
            } else {
                OnLoadingError?.Invoke (result);
            }
        }

        public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
        {
            if (!this.searchBadge.RequiredBadgeIsRegistered)
                return;
            
            if (expandedPaths.Contains (indexPath)) {
                expandedPaths.Remove (indexPath);
            } else {
                expandedPaths.Add (indexPath);
            }
            tableView.ReloadRows (new [] { indexPath }, UITableViewRowAnimation.Fade);
        }

        public override nint RowsInSection (UITableView tableview, nint section)
        {
            var rowsCount = Items?.Count ?? 0;

            return !this.searchBadge.RequiredBadgeIsRegistered && rowsCount > limitedResultsCount ? limitedResultsCount + 1 : rowsCount;
        }

        public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
        {
            if (!this.searchBadge.RequiredBadgeIsRegistered && indexPath.Row == limitedResultsCount) {
                return SearchLimitCell.Height;
            }

            if (expandedPaths.Contains (indexPath)) {
                return SearchResultsCell.ExpandedHeight;
            }

            return SearchResultsCell.CollapsedHeight;
        }

        private async void LoadMoreAsync (UITableView tableView)
        {
            if (!loading && this.searchBadge.RequiredBadgeIsRegistered) {
                this.loading = true;
                LoadingMoreStarted?.Invoke ();
                await this.LoadDataAsync (tableView);
                LoadingMoreEnded?.Invoke ();
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
            var rowNumber = indexPath.Row;
            UITableViewCell cell = null;
            if (this.searchBadge.RequiredBadgeIsRegistered || rowNumber < limitedResultsCount) {
                var item = Items [indexPath.Row];
                cell = tableView.DequeueReusableCell (SearchResultsCell.Key, indexPath);
                ConfigureCell (cell as SearchResultsCell, item);
            } else if (!this.searchBadge.RequiredBadgeIsRegistered) {
                var searchLimitCell = SearchLimitCell.Create (searchBadge.RequiredBadgeName);
                searchLimitCell.OnRegisterButtonTouched += SearchLimitCell_OnRegisterButtonTouched;
                cell = searchLimitCell;
            }

            return cell;
        }

        private void ConfigureCell (SearchResultsCell cell, ISearchResultDTO item)
        {
            cell.Configure (item);

            cell.SendMessageButtonAction = (obj) => SendMessageButtonTouched (obj, item);
            cell.ViewProfileButtonAction = (obj) => ViewProfileButtonTouched (obj, item);
        }

        void SearchLimitCell_OnRegisterButtonTouched ()
        {
            if (searchBadge.RequiredBadgeId.HasValue) {
                this.RegisterButtonTouched?.Invoke (searchBadge.RequiredBadgeId.Value);
            }
        }
    }
}
