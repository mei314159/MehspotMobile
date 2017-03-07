using Foundation;
using System;
using UIKit;
using System.Collections.Generic;
using mehspot.iOS.Views;
using Mehspot.Core.DTO;
using System.Linq;
using Mehspot.Core.Messaging;
using Mehspot.Core;
using System.Threading.Tasks;
using CoreGraphics;
using mehspot.iOS.Extensions;
using mehspot.iOS.Wrappers;

namespace mehspot.iOS
{
    public partial class EditProfileController : UIViewController, IUITableViewDataSource, IUITableViewDelegate
    {
        private ViewHelper viewHelper;
        private readonly ProfileService profileService;
        private List<UITableViewCell> cells = new List<UITableViewCell> ();
        private KeyValuePair<int?, string> [] subdivisions;

        public EditProfileDto profile;


        public EditProfileController (IntPtr handle) : base (handle)
        {
            profileService = new ProfileService (MehspotAppContext.Instance.DataStorage);
            viewHelper = new ViewHelper (this.View);
        }

        public UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
        {
            var item = cells [indexPath.Row];
            return item;
        }

        public nint RowsInSection (UITableView tableView, nint section)
        {
            return cells.Count;
        }

        public override async void ViewDidLoad ()
        {
            this.ProfileTableView.TableFooterView = new UIView ();
            await InitializeCells ();
            ProfileTableView.WeakDataSource = this;
            ProfileTableView.Delegate = this;
            ProfileTableView.ReloadData ();
            RegisterForKeyboardNotifications ();
            ProfileTableView.AddGestureRecognizer (new UITapGestureRecognizer (HideKeyboard));
        }

        public void HideKeyboard ()
        {
            this.View.FindFirstResponder()?.ResignFirstResponder ();
        }

        async partial void SaveButtonTouched (UIBarButtonItem sender)
        {
            HideKeyboard ();
            viewHelper.ShowOverlay ("Saving...");
            var result = await this.profileService.UpdateAsync (profile);
            viewHelper.HideOverlay ();
            if (!result.IsSuccess) {
                var error = string.Join (Environment.NewLine, result.ModelState.ModelState.SelectMany (a => a.Value));
                UIAlertView alert = new UIAlertView (
                                    result.ErrorMessage,
                                    error,
                                    null,
                                    "OK");
                alert.Show ();
            } else {
                this.NavigationController.PopViewController (true);
            }
        }

        protected virtual void RegisterForKeyboardNotifications ()
        {
            NSNotificationCenter.DefaultCenter.AddObserver (UIKeyboard.WillHideNotification, OnKeyboardNotification);
            NSNotificationCenter.DefaultCenter.AddObserver (UIKeyboard.WillShowNotification, OnKeyboardNotification);
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
                var yTreshold = ProfileTableView.Frame.Y + ProfileTableView.Frame.Height;
                var deltaY = yTreshold - relativeLocation.Y;
                ProfileTableView.Frame = new CGRect (ProfileTableView.Frame.Location, new CGSize (ProfileTableView.Frame.Width, ProfileTableView.Frame.Height - deltaY));
            } else {
                var deltaY = (this.View.Frame.Height) - (ProfileTableView.Frame.Y + ProfileTableView.Frame.Height);
                ProfileTableView.Frame = new CGRect (ProfileTableView.Frame.Location, new CGSize (ProfileTableView.Frame.Width, ProfileTableView.Frame.Height + deltaY));
            }
        }

        private async Task InitializeCells ()
        {
            var genders = new [] {
                new KeyValuePair<string, string>(null, "N/A"),
                new KeyValuePair<string, string>("M", "Male"),
                new KeyValuePair<string, string>("F", "Female")
            };

            var statesResult = await profileService.GetStatesAsync ();
            var states = statesResult.Data.Select (a => new KeyValuePair<int?, string> (a.Id, a.Name)).ToArray ();

            subdivisions = await GetSubdivisions (profile.Zip);

            cells.Add (TextEditCell.Create (profile, a => a.UserName, "User Name"));
            cells.Add (TextEditCell.Create (profile, a => a.Email, "Email", true));
            var phoneNumberCell = TextEditCell.Create (profile, a => a.PhoneNumber, "Phone Number");
            phoneNumberCell.Mask = "(###)###-####";
            cells.Add (phoneNumberCell);// TODO: Mask field
            cells.Add (PickerCell.Create (profile, a => a.DateOfBirth, (model, property) => { model.DateOfBirth = property; }, v => v?.ToString ("MMMM dd, yyyy"), "Date Of Birth"));
            cells.Add (PickerCell.Create (profile, a => a.Gender, (model, property) => { model.Gender = property; }, v => genders.First (a => a.Key == v).Value, "Gender", genders));
            // TODO: Email field
            cells.Add (TextEditCell.Create (profile, a => a.FirstName, "First Name"));
            cells.Add (TextEditCell.Create (profile, a => a.LastName, "Last Name"));
            cells.Add (TextEditCell.Create (profile, a => a.AddressLine1, "Address Line 1"));
            cells.Add (TextEditCell.Create (profile, a => a.AddressLine2, "Address Line 2"));
            cells.Add (PickerCell.Create (profile, a => a.State, (model, property) => { model.State = property; }, v => states.FirstOrDefault (a => a.Key == v).Value, "State", states));
            cells.Add (TextEditCell.Create (profile, a => a.City, "City"));
            var zipCell = TextEditCell.Create (profile, a => a.Zip, "Zip");
            zipCell.Mask = "#####";
            var subdivisionEnabled = !string.IsNullOrWhiteSpace (profile.Zip) && zipCell.IsValid;
            var subdivisionCell = PickerCell.Create (profile, a => a.SubdivisionId, (model, property) => { model.SubdivisionId = (int?)property; }, v => subdivisions.FirstOrDefault (a => a.Key == v).Value, "Subdivision", subdivisions, !subdivisionEnabled);
            zipCell.ValueChanged += (arg1, arg2) => ZipCell_ValueChanged (arg1, arg2, subdivisionCell);
            cells.Add (zipCell); //zip mask
            cells.Add (subdivisionCell); //Subdivision Selector

            cells.Add (BooleanEditCell.Create (profile, a => a.MehspotNotificationsEnabled, "Email notifications enabled")); //True-False selector
            cells.Add (BooleanEditCell.Create (profile, a => a.AllGroupsNotificationsEnabled, "Group notifications enabled")); //True-False selector
        }

        async void ZipCell_ValueChanged (TextEditCell sender, string value, PickerCell subdivisionCell)
        {
            subdivisionCell.IsReadOnly = true;
            if (sender.IsValid) {
                subdivisionCell.RowValues = (await GetSubdivisions (profile.Zip)).Select (a => new KeyValuePair<object, string> (a.Key, a.Value)).ToArray ();
            }

            subdivisionCell.IsReadOnly = !sender.IsValid;
        }

        private async Task<KeyValuePair<int?, string> []> GetSubdivisions (string zipCode)
        {
            var subdivisionsResult = await profileService.GetSubdivisionsAsync (zipCode);
            if (subdivisionsResult.IsSuccess) {
                subdivisions = subdivisionsResult.Data.Select (a => new KeyValuePair<int?, string> (a.Id, a.DisplayName)).ToArray ();
                return subdivisions;
            }

            return null;
        }
    }
}