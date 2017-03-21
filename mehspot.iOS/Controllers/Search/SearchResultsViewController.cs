using Foundation;
using System;
using UIKit;
using Mehspot.Core.DTO.Search;
using Mehspot.Core.Messaging;
using Mehspot.Core;
using System.Threading.Tasks;
using CoreGraphics;
using MehSpot.Models.ViewModels;
using mehspot.iOS.Views.Cell;
using SDWebImage;
using System.Collections.Generic;

namespace mehspot.iOS
{
    public partial class SearchResultsViewController : UITableViewController
    {
        private volatile bool loading;
        private volatile bool viewWasInitialized;
        private List<NSIndexPath> expandedPaths = new List<NSIndexPath> ();
        private List<BabysitterDetailsDTO> items = new List<BabysitterDetailsDTO>();
        private BadgeService badgeService;
        private string SelectedUserId;
        private string SelectedUserName;

        private const int pageSize = 20;

        public ISearchFilterDTO Filter;

        public KeyValuePair<int?, string> [] AgeRanges { get; internal set; }

        public string BadgeName;

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
            var controller = (MessagingViewController)segue.DestinationViewController;
            controller.ToUserName = this.SelectedUserName;
            controller.ToUserId = this.SelectedUserId;
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
                return 85;
            }
        }

        public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
        {
            var item = items [indexPath.Row];

            var cell = TableView.DequeueReusableCell (SearchResultsCell.Key, indexPath);
            ConfigureCell (cell as SearchResultsCell, item);
            return cell;
        }

        private void ConfigureCell (SearchResultsCell cell, BabysitterDetailsDTO item)
        {
            if (!string.IsNullOrEmpty (item.Details.ProfilePicturePath)) {

                var url = NSUrl.FromString (item.Details.ProfilePicturePath);
                if (url != null) {
                    cell.ProfilePicture.SetImage (url);
                }
            }

            cell.UserNameLabel.Text = item.Details.FirstName;
            cell.DistanceLabel.Text = Math.Round (item.Details.DistanceFrom ?? 0, 2) + " miles";
            cell.SubdivisionLabel.Text = !string.IsNullOrWhiteSpace (item.Details.Subdivision) ? $"{item.Details.Subdivision} ({item.Details.ZipCode})" : item.Details.ZipCode;
            cell.HourlyRateLabel.Text = $"${item.HourlyRate}/hr";
            cell.AgeRangeLabel.Text = item.AgeRange.HasValue ? AgeRanges [item.AgeRange.Value].Value : string.Empty;
            cell.FavoriteIcon.Hidden = !item.Details.Favourite;
            cell.LikesLabel.Text = $"{item.Details.Likes} Likes / {item.Details.Recommendations} Recommendations";

            cell.SendMessageButton.Layer.BorderWidth = cell.ViewProfileButton.Layer.BorderWidth = 1;
            cell.SendMessageButton.Layer.BorderColor = cell.SendMessageButton.TitleColor (UIControlState.Normal).CGColor;
            cell.ViewProfileButton.Layer.BorderColor = cell.ViewProfileButton.TitleColor (UIControlState.Normal).CGColor;
            cell.SendMessageButtonAction = (obj) => SendMessageButtonTouched (obj, item);
        }

        void SendMessageButtonTouched (UIButton obj, BabysitterDetailsDTO dto)
        {
            this.SelectedUserId = dto.Details.UserId;
            this.SelectedUserName = dto.Details.FirstName;
            PerformSegue ("GoToMessagingSegue", this);
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
            var result = await badgeService.Search<BabysitterDetailsDTO> (this.Filter, this.BadgeName, skip, pageSize);
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