using Foundation;
using System;
using UIKit;
using System.Threading.Tasks;
using CoreGraphics;
using System.Collections.Generic;
using mehspot.iOS.Extensions;
using Mehspot.Core.Contracts.Wrappers;
using mehspot.iOS.Wrappers;
using Mehspot.Core.DTO.Search;
using mehspot.iOS.Views;
using System.Linq;
using Mehspot.Core.Messaging;
using Mehspot.Core;
using mehspot.iOS.Views.Cell;

namespace mehspot.iOS
{
    public partial class SearchBabysitterController : UITableViewController
    {
        volatile bool viewWasInitialized;
        private BadgeService badgeService;
        private IViewHelper viewHelper;
        private SearchBabysitterDTO filter;
        private List<UITableViewCell> cells;
        private KeyValuePair<int?, string> [] ageRanges;

        public SearchBabysitterController (IntPtr handle) : base (handle)
        {
            viewHelper = new ViewHelper (this.View);
        }

        public override void ViewDidLoad ()
        {
            badgeService = new BadgeService (MehspotAppContext.Instance.DataStorage);
            filter = new SearchBabysitterDTO ();
            cells = new List<UITableViewCell> ();
            this.TableView.WeakDataSource = this;
            this.TableView.Delegate = this;
        }

        public override string TitleForHeader (UITableView tableView, nint section)
        {
            if (section == 0) {
                return "Filter";
            }

            return string.Empty;
        }

        public override async void ViewWillAppear (bool animated)
        {
            if (!viewWasInitialized)
                await InitializeView ();
            this.TableView.ReloadData ();
            this.TableView.AddGestureRecognizer (new UITapGestureRecognizer (HideKeyboard));
        }

        public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
        {
            var item = cells [indexPath.Row];
            return item;
        }

        public override nint RowsInSection (UITableView tableView, nint section)
        {
            return cells.Count;
        }

        public void HideKeyboard ()
        {
            this.View.FindFirstResponder ()?.ResignFirstResponder ();
        }


        public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
        {
            if (segue.Identifier == "SearchResultsSegue") {
                var destinationViewController = ((SearchResultsViewController)segue.DestinationViewController);
                destinationViewController.Filter = filter;
                destinationViewController.BadgeName = "Babysitter";
            }

            base.PrepareForSegue (segue, sender);
        }

        private async Task InitializeView ()
        {
            viewHelper.ShowOverlay ("Loading...");
            ageRanges = await GetAgeRangesAsync ();

            cells.Add (SliderCell.Create (filter, a => a.Details.MaxDistance, "Max Distance"));
            cells.Add (SliderCell.Create (filter, a => a.HourlyRate, "Max Hourly Rate ($)"));
            var zipCell = TextEditCell.Create (filter, a => a.Details.Zip, "Zip");
            zipCell.Mask = "#####";
            cells.Add (zipCell);

            cells.Add (BooleanEditCell.Create (filter, a => a.HasCar, "Has Car"));
            cells.Add (BooleanEditCell.Create (filter, a => a.Details.HasPicture, "Has Profile Picture"));
            cells.Add (BooleanEditCell.Create (filter, a => a.Details.HasReferences, "Has References"));
            cells.Add (BooleanEditCell.Create (filter, a => a.HasCertifications, "Has Certifications"));
            cells.Add (PickerCell.Create (filter, a => a.AgeRange, (model, property) => { model.AgeRange = property; }, v => ageRanges.FirstOrDefault (a => a.Key == v).Value, "Age Range", ageRanges));
            var searchButtonCell = ButtonCell.Create (SearchButtonTouched, "Search");
            searchButtonCell.CellButton.BackgroundColor = UIColor.FromRGB (0, 254, 0);
            searchButtonCell.CellButton.TintColor = UIColor.White;
            searchButtonCell.CellButton.Layer.CornerRadius = 5;
            cells.Add (searchButtonCell);
            viewHelper.HideOverlay ();
            viewWasInitialized = true;
        }


        private async Task<KeyValuePair<int?, string> []> GetAgeRangesAsync ()
        {
            var result = await badgeService.GetAgeRangesAsync (BadgeService.BadgeNames.Babysitter);
            if (result.IsSuccess) {
                return result.Data.Select (a => new KeyValuePair<int?, string> (a.Id, a.Name)).ToArray ();
            }

            return null;
        }

        private void SearchButtonTouched (UIButton sender)
        {
            this.PerformSegue ("SearchResultsSegue", this);
        }
    }
}