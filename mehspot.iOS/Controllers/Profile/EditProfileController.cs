using Foundation;
using CoreGraphics;
using UIKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mehspot.Core;
using Mehspot.Core.DTO;
using Mehspot.Core.Messaging;
using mehspot.iOS.Views;
using mehspot.iOS.Extensions;
using mehspot.iOS.Wrappers;
using System.Runtime.InteropServices;
using SDWebImage;
using Mehspot.Core.Contracts.Wrappers;

namespace mehspot.iOS
{
    public partial class EditProfileController : UIViewController, IUITableViewDataSource, IUITableViewDelegate
    {
        volatile bool viewWasInitialized;
        volatile bool profileImageChanged;

        private IViewHelper viewHelper;
        private readonly ProfileService profileService;
        private List<UITableViewCell> cells = new List<UITableViewCell> ();
        private KeyValuePair<int?, string> [] states;
        private KeyValuePair<int?, string> [] subdivisions;

        public ProfileDto profile;


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

        public override void ViewDidLoad ()
        {
            this.ProfileTableView.TableFooterView = new UIView ();
        }

        public override async void ViewWillAppear (bool animated)
        {
            if (!viewWasInitialized)
                await InitializeView ();
            ProfileTableView.WeakDataSource = this;
            ProfileTableView.Delegate = this;
            ProfileTableView.ReloadData ();
            RegisterForKeyboardNotifications ();
            ProfileTableView.AddGestureRecognizer (new UITapGestureRecognizer (HideKeyboard));
        }

        public void HideKeyboard ()
        {
            this.View.FindFirstResponder ()?.ResignFirstResponder ();
        }

        partial void ChangePhotoButtonTouched (UIButton sender)
        {
            var photoSourceActionSheet = new UIActionSheet ("Take a photo from");
            photoSourceActionSheet.AddButton ("Camera");
            photoSourceActionSheet.AddButton ("Photo Library");
            photoSourceActionSheet.AddButton ("Cancel");
            photoSourceActionSheet.CancelButtonIndex = 2;
            photoSourceActionSheet.Clicked += PhotoSouceActionSheet_Clicked; ;
            photoSourceActionSheet.ShowInView (View);
        }

        private void PhotoSouceActionSheet_Clicked (object sender, UIButtonEventArgs e)
        {
            var imagePicker = new UIImagePickerController ();
            imagePicker.MediaTypes = new string [] { MobileCoreServices.UTType.Image };
            if (e.ButtonIndex == 0) {
                imagePicker.SourceType = UIImagePickerControllerSourceType.Camera;
            } else if (e.ButtonIndex == 1) {
                imagePicker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
            } else {
                return;
            }

            imagePicker.FinishedPickingMedia += Handle_FinishedPickingMedia;
            imagePicker.Canceled += Handle_Canceled;

            NavigationController.PresentModalViewController (imagePicker, true);
        }

        void Handle_FinishedPickingMedia (object sender, UIImagePickerMediaPickedEventArgs e)
        {
            if (e.Info [UIImagePickerController.MediaType].ToString () != MobileCoreServices.UTType.Image)
                return;

            NSUrl referenceURL = e.Info [new NSString (UIImagePickerController.ReferenceUrl)] as NSUrl;
            if (referenceURL != null)
                Console.WriteLine ("Url:" + referenceURL);

            UIImage originalImage = e.Info [UIImagePickerController.OriginalImage] as UIImage;
            if (originalImage != null) {
                ProfilePicture.Image = UIImage.FromImage (originalImage.CGImage, 4, originalImage.Orientation);
                this.profileImageChanged = true;
            }

            ((UIImagePickerController)sender).DismissModalViewController (true);
        }

        void Handle_Canceled (object sender, EventArgs e)
        {
            ((UIImagePickerController)sender).DismissModalViewController (true);
        }


        async partial void SaveButtonTouched (UIBarButtonItem sender)
        {
            sender.Enabled = false;
            HideKeyboard ();
            viewHelper.ShowOverlay ("Saving...");

            if (profileImageChanged) {
                var data = this.ProfilePicture.Image.AsJPEG ();
                byte [] dataBytes = new byte [data.Length];
                Marshal.Copy (data.Bytes, dataBytes, 0, Convert.ToInt32 (data.Length));
                await this.profileService.UploadProfileImageAsync (dataBytes);
            }

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
                this.NavigationController?.PopViewController (true);
            }

            sender.Enabled = true;
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

        private async Task InitializeView ()
        {

            if (!string.IsNullOrEmpty (profile.ProfilePicturePath)) {
                var url = NSUrl.FromString (profile.ProfilePicturePath);
                if (url != null) {
                    this.ProfilePicture.SetImage (url);
                }
            }

            var genders = new [] {
                new KeyValuePair<string, string>(null, "N/A"),
                new KeyValuePair<string, string>("M", "Male"),
                new KeyValuePair<string, string>("F", "Female")
            };

            viewHelper.ShowOverlay ("Loading...");
            states = await GetStates ();
            subdivisions = await GetSubdivisions (profile.Zip);

            cells.Add (TextEditCell.Create (profile, a => a.UserName, "User Name"));
            cells.Add (TextEditCell.Create (profile, a => a.Email, "Email", true));
            var phoneNumberCell = TextEditCell.Create (profile, a => a.PhoneNumber, "Phone Number");
            phoneNumberCell.Mask = "(###)###-####";
            cells.Add (phoneNumberCell);
            cells.Add (PickerCell.Create (profile, a => a.DateOfBirth, (model, property) => { model.DateOfBirth = property; }, v => v?.ToString ("MMMM dd, yyyy"), "Date Of Birth"));
            cells.Add (PickerCell.Create (profile, a => a.Gender, (model, property) => { model.Gender = property; }, v => genders.First (a => a.Key == v).Value, "Gender", genders));
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
            cells.Add (zipCell);
            cells.Add (subdivisionCell);

            cells.Add (BooleanEditCell.Create (profile, a => a.MehspotNotificationsEnabled, "Email notifications enabled"));
            cells.Add (BooleanEditCell.Create (profile, a => a.AllGroupsNotificationsEnabled, "Group notifications enabled"));

            viewHelper.HideOverlay ();
            viewWasInitialized = true;
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
            if (!string.IsNullOrWhiteSpace (zipCode)) {
                var subdivisionsResult = await profileService.GetSubdivisionsAsync (zipCode);
                if (subdivisionsResult.IsSuccess) {
                    subdivisions = subdivisionsResult.Data.Select (a => new KeyValuePair<int?, string> (a.Id, a.DisplayName)).ToArray ();
                    return subdivisions;
                }
            }

            return null;
        }

        private async Task<KeyValuePair<int?, string> []> GetStates ()
        {
            var statesResult = await profileService.GetStatesAsync ();
            if (statesResult.IsSuccess) {
                return statesResult.Data.Select (a => new KeyValuePair<int?, string> (a.Id, a.Name)).ToArray ();
            }

            return null;
        }
    }
}