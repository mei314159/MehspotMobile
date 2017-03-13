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
            RegisterForKeyboardNotifications ();
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

        protected virtual void RegisterForKeyboardNotifications ()
        {
            NSNotificationCenter.DefaultCenter.AddObserver (UIKeyboard.WillHideNotification, OnKeyboardNotification);
            NSNotificationCenter.DefaultCenter.AddObserver (UIKeyboard.WillShowNotification, OnKeyboardNotification);
        }

        public void HideKeyboard ()
        {
            this.View.FindFirstResponder ()?.ResignFirstResponder ();
        }

        private void OnKeyboardNotification (NSNotification notification)
        {
            if (!IsViewLoaded)
                return;

            //Check if the keyboard is becoming visible
            var visible = notification.Name == UIKeyboard.WillShowNotification;

            //Start an animation, using values from the keyboard
            UIView.BeginAnimations ("AnimateForKeyboard");
            UIView.SetAnimationBeginsFromCurrentState (true);
            UIView.SetAnimationDuration (UIKeyboard.AnimationDurationFromNotification (notification));
            UIView.SetAnimationCurve ((UIViewAnimationCurve)UIKeyboard.AnimationCurveFromNotification (notification));

            //Pass the notification, calculating keyboard height, etc.
            var keyboardFrame = visible
                                    ? UIKeyboard.FrameEndFromNotification (notification)
                                    : UIKeyboard.FrameBeginFromNotification (notification);
            OnKeyboardChanged (visible, keyboardFrame);
            //Commit the animation
            UIView.CommitAnimations ();
        }

        private void OnKeyboardChanged (bool visible, CGRect keyboardFrame)
        {
            if (View.Superview == null) {
                return;
            }

            if (visible) {
                var relativeLocation = View.Superview.ConvertPointToView (keyboardFrame.Location, View);
                var yTreshold = this.TableView.Frame.Y + this.TableView.Frame.Height;
                var deltaY = yTreshold - relativeLocation.Y;
                this.TableView.Frame = new CGRect (this.TableView.Frame.Location, new CGSize (this.TableView.Frame.Width, this.TableView.Frame.Height - deltaY));
            } else {
                var deltaY = (this.View.Frame.Height) - (this.TableView.Frame.Y + this.TableView.Frame.Height);
                this.TableView.Frame = new CGRect (this.TableView.Frame.Location, new CGSize (this.TableView.Frame.Width, this.TableView.Frame.Height + deltaY));
            }
        }

        private async Task InitializeView ()
        {
            viewHelper.ShowOverlay ("Loading...");
            ageRanges = await GetAgeRangesAsync ();

            cells.Add (TextEditCell.Create (filter, a => a.MaxDistance, "Max Distance"));
            cells.Add (TextEditCell.Create (filter, a => a.MaxHourlyRate, "Max Hourly Rate ($)"));
            var zipCell = TextEditCell.Create (filter, a => a.Zip, "Zip");
            zipCell.Mask = "#####";
            cells.Add (zipCell);

            cells.Add (BooleanEditCell.Create (filter, a => a.HasCar, "Has Car"));
            cells.Add (BooleanEditCell.Create (filter, a => a.HasProfilePicture, "Has Profile Picture"));
            cells.Add (BooleanEditCell.Create (filter, a => a.HasReferences, "Has References"));
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

        void SearchButtonTouched (UIButton sender)
        {
            this.PerformSegue ("SearchResultsSegue", this);
        }
    }
}