using Foundation;
using System;
using UIKit;
using System.Threading.Tasks;
using CoreGraphics;
using MehSpot.Models.ViewModels;
using mehspot.iOS.Views.Cell;
using mehspot.iOS.Controllers.Badges.DataSources.Search;
using Mehspot.Core;
using Mehspot.Core.Services;

namespace mehspot.iOS
{
    public partial class SearchResultsViewController : UITableViewController
    {
        private volatile bool loading;
        private volatile bool viewWasInitialized;

        private ISearchResultDTO SelectedItem;
        public SearchModel SearchModel;


        public SearchResultsViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad ()
        {
            this.TableView.RegisterNibForCellReuse (SearchResultsCell.Nib, SearchResultsCell.Key);
            SearchModel.SearchResultTableSource.SendMessageButtonTouched += SendMessageButtonTouched;
            SearchModel.SearchResultTableSource.ViewProfileButtonTouched += ViewProfileButtonTouched;
            SearchModel.SearchResultTableSource.LoadingStarted += LoadingStarted;
            SearchModel.SearchResultTableSource.LoadingEnded += LoadingEnded;
            this.TableView.Source = SearchModel.SearchResultTableSource;

            this.RefreshControl.ValueChanged += RefreshControl_ValueChanged;
            this.TableView.TableFooterView.Hidden = true;
            SetTitle ();
        }

        public override async void ViewDidAppear (bool animated)
        {
            if (!viewWasInitialized) {
                await RefreshResultsAsync ();
                viewWasInitialized = true;
            }
        }

        private void SetTitle ()
        {
            var title = MehspotResources.ResourceManager.GetString (this.SearchModel.BadgeName + "_SearchResultsTitle") ??
                                            ((MehspotResources.ResourceManager.GetString (this.SearchModel.BadgeName) ?? this.SearchModel.BadgeName) + "s");
            this.NavBar.Title = title;
        }

        [Action ("UnwindToSearchResultsViewController:")]
        public void UnwindToSearchResultsViewController (UIStoryboardSegue segue)
        {
        }

        private void LoadingStarted ()
        {
            this.ActivityIndicator.StartAnimating ();
            this.TableView.TableFooterView.Hidden = false;
        }

        private void LoadingEnded ()
        {
            this.ActivityIndicator.StopAnimating ();
            this.TableView.TableFooterView.Hidden = true;
        }

        public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
        {
            if (segue.Identifier == "GoToMessagingSegue") {
                var controller = (MessagingViewController)segue.DestinationViewController;
                controller.ToUserName = this.SelectedItem.Details.FirstName;
                controller.ToUserId = this.SelectedItem.Details.UserId;
            } else if (segue.Identifier == "ViewProfileSegue") {
                var controller = (ViewProfileViewController)segue.DestinationViewController;
                controller.SearchModel = SearchModel;
                controller.SearchResultDTO = this.SelectedItem;
            }

            base.PrepareForSegue (segue, sender);
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
            await SearchModel.SearchResultTableSource.LoadDataAsync (this.TableView, true);
            this.RefreshControl.EndRefreshing ();
            loading = false;
        }
    }
}