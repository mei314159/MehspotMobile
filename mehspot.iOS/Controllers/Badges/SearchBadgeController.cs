using Foundation;
using System;
using UIKit;
using mehspot.iOS.Extensions;
using Mehspot.Core.Contracts.Wrappers;
using mehspot.iOS.Wrappers;
using Mehspot.Core;
using Mehspot.Core.Services;
using mehspot.iOS.Controllers.Badges.DataSources.Search;

namespace mehspot.iOS
{
    public partial class SearchBadgeController : UITableViewController
    {
        volatile bool viewWasInitialized;
        private IViewHelper viewHelper;
        private SearchModel SearchModel;
        public int BadgeId;
        public string BadgeName;


        public SearchBadgeController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad ()
        {
            viewHelper = new ViewHelper (this.View);
            this.TableView.AddGestureRecognizer (new UITapGestureRecognizer (HideKeyboard));
            this.TableView.TableFooterView.Hidden = true;
        }

        private void SetTitle ()
        {
            var badgeName = MehspotResources.ResourceManager.GetString (this.BadgeName) ?? this.BadgeName;
            var title = "Search for " + badgeName;
            this.NavBar.Title = title;
        }

        public override async void ViewWillAppear (bool animated)
        {
            SetTitle ();
            if (viewWasInitialized)
                return;

            viewHelper.ShowOverlay ("Loading...");

            this.SearchModel = await SearchModel.GetInstanceAsync (new BadgeService (MehspotAppContext.Instance.DataStorage), this.BadgeName, this.BadgeId);
            this.TableView.Source = this.SearchModel.SearchFilterTableSource;
            this.TableView.ReloadData ();

            viewHelper.HideOverlay ();
            viewWasInitialized = true;
            this.TableView.TableFooterView.Hidden = false;
        }

        public void HideKeyboard ()
        {
            this.View.FindFirstResponder ()?.ResignFirstResponder ();
        }

        public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
        {
            if (segue.Identifier == "SearchResultsSegue") {
                var destinationViewController = ((SearchResultsViewController)segue.DestinationViewController);
                destinationViewController.SearchModel = this.SearchModel;
                this.NavBar.Title = "Filter";
            }

            base.PrepareForSegue (segue, sender);
        }

        partial void SearchButtonTouched (UIButton sender)
        {
            this.PerformSegue ("SearchResultsSegue", this);
        }
    }
}