using Foundation;
using System;
using UIKit;
using Mehspot.Core.Contracts.Wrappers;
using System.Collections.Generic;
using Mehspot.Core.DTO;
using Mehspot.Core;
using mehspot.iOS.Wrappers;
using mehspot.iOS.Extensions;
using System.Runtime.InteropServices;
using CoreGraphics;
using System.Threading.Tasks;
using SDWebImage;
using mehspot.iOS.Views;
using System.Linq;
using Mehspot.Core.Services;
using Mehspot.DTO;

namespace mehspot.iOS
{
    public partial class EditProfileViewController : UITableViewController, IUITableViewDataSource, IUITableViewDelegate
    {
        volatile bool loading;
        volatile bool dataLoaded;
        volatile bool profileImageChanged;

        private IViewHelper viewHelper;
        private ProfileService profileService;
        private List<UITableViewCell> cells = new List<UITableViewCell> ();
        private KeyValuePair<string, string> [] genders = {
                new KeyValuePair<string, string>(null, "N/A"),
                new KeyValuePair<string, string>("M", "Male"),
                new KeyValuePair<string, string>("F", "Female")
        };

        public ProfileDto profile;


        public EditProfileViewController (IntPtr handle) : base (handle)
        {
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

        public override void ViewDidLoad ()
        {
            profileService = new ProfileService (MehspotAppContext.Instance.DataStorage);
            viewHelper = new ViewHelper (this.View);
            ChangePhotoButton.Layer.BorderWidth = 1;
            ChangePhotoButton.Layer.BorderColor = UIColor.LightGray.CGColor;
            TableView.Delegate = this;
            TableView.WeakDataSource = this;
            TableView.AddGestureRecognizer (new UITapGestureRecognizer (HideKeyboard));
            TableView.TableHeaderView.Hidden = TableView.TableFooterView.Hidden = true;
            RefreshControl.ValueChanged += RefreshControl_ValueChanged;
        }

        public override async void ViewDidAppear (bool animated)
        {
            if (!dataLoaded) {
                await RefreshView ();
                TableView.TableHeaderView.Hidden = TableView.TableFooterView.Hidden = false;
            }
        }

        private async void RefreshControl_ValueChanged (object sender, EventArgs e)
        {
            await RefreshView ();
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
            photoSourceActionSheet.Clicked += PhotoSouceActionSheet_Clicked;
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
            }

            this.SaveButton.Enabled = true;
        }

        partial void SignoutButtonTouched (UIButton sender)
        {
            UIAlertView alert = new UIAlertView (
                                            "Sign Out",
                                            "Are you sure you want to sign out?",
                                            null,
                                            "Cancel",
                                            new string [] { "Yes, I do" });
            alert.Clicked += (object s, UIButtonEventArgs e) => {
                if (e.ButtonIndex != alert.CancelButtonIndex) {
                    MehspotAppContext.Instance.AuthManager.SignOut ();
                    MehspotAppContext.Instance.DisconnectSignalR ();

                    var targetViewController = UIStoryboard.FromName ("Main", null).InstantiateViewController ("LoginViewController");

                    targetViewController.SwapController (UIViewAnimationOptions.TransitionFlipFromRight);
                }
            };
            alert.Show ();
        }

        private async Task RefreshView ()
        {
            if (loading)
                return;
            loading = true;
            TableView.UserInteractionEnabled = false;
            this.SaveButton.Enabled = this.ChangePhotoButton.Enabled = false;
            RefreshControl.BeginRefreshing ();
            TableView.SetContentOffset (new CGPoint (0, -this.TableView.RefreshControl.Frame.Size.Height), true);

            var profileResult = await profileService.GetProfileAsync ();
            RefreshControl.EndRefreshing ();

            if (profileResult.IsSuccess) {
                profile = profileResult.Data;
                var states = await GetStates ();
                var subdivisions = await GetSubdivisions (profile.Zip);
                InitializeTable (profile, states, subdivisions);
            }else {
                RefreshControl.EndRefreshing ();
                new UIAlertView ("Error", "Can not load profile data", null, "OK").Show ();
            }

            TableView.UserInteractionEnabled = true;
            dataLoaded = this.SaveButton.Enabled = this.ChangePhotoButton.Enabled = profileResult.IsSuccess;
            loading = false;
        }

        void InitializeTable (ProfileDto profile, KeyValuePair<int?, string> [] states, SubdivisionDTO [] subdivisions)
        {
            this.UserNameLabel.Text = profile.UserName;
            this.FullName.Text = $"{profile.FirstName} {profile.LastName}".Trim (' ');

            if (!string.IsNullOrEmpty (profile.ProfilePicturePath)) {
                var url = NSUrl.FromString (profile.ProfilePicturePath);
                if (url != null) {
                    this.ProfilePicture.SetImage (url);
                }
            }

            cells.Clear ();
            cells.Add (TextEditCell.Create (profile, a => a.UserName, "User Name"));
            cells.Add (TextEditCell.Create (profile, a => a.Email, "Email", null, true));
            var phoneNumberCell = TextEditCell.Create (profile, a => a.PhoneNumber, "Phone Number");
            phoneNumberCell.Mask = "(###)###-####";
            cells.Add (phoneNumberCell);
            cells.Add (PickerCell.CreateDatePicker (profile.DateOfBirth, (property) => { profile.DateOfBirth = property; }, "Date Of Birth"));
            cells.Add (PickerCell.Create (profile.Gender, (property) => { profile.Gender = property; }, "Gender", genders));
            cells.Add (TextEditCell.Create (profile, a => a.FirstName, "First Name"));
            cells.Add (TextEditCell.Create (profile, a => a.LastName, "Last Name"));
            cells.Add (TextEditCell.Create (profile, a => a.AddressLine1, "Address Line 1"));
            cells.Add (TextEditCell.Create (profile, a => a.AddressLine2, "Address Line 2"));
            cells.Add (PickerCell.Create (profile.State, (property) => { profile.State = property; }, "State", states));
            cells.Add (TextEditCell.Create (profile, a => a.City, "City"));
            var zipCell = TextEditCell.Create (profile, a => a.Zip, "Zip");
            zipCell.Mask = "#####";
            var subdivisionEnabled = !string.IsNullOrWhiteSpace (profile.Zip) && zipCell.IsValid;
            var subdivisionCell = SubdivisionPickerCell.Create (profile.SubdivisionId, (property) => { profile.SubdivisionId = property?.Id; }, "Subdivision", subdivisions, !subdivisionEnabled);
            zipCell.ValueChanged += (arg1, arg2) => ZipCell_ValueChanged (arg1, arg2, subdivisionCell);
            cells.Add (zipCell);
            cells.Add (subdivisionCell);

            cells.Add (BooleanEditCell.Create (profile.MehspotNotificationsEnabled, v => profile.MehspotNotificationsEnabled = v, "Email notifications enabled"));
            cells.Add (BooleanEditCell.Create (profile.AllGroupsNotificationsEnabled, v => profile.AllGroupsNotificationsEnabled = v, "Group notifications enabled"));

            TableView.ReloadData ();
        }

        async void ZipCell_ValueChanged (TextEditCell sender, string value, SubdivisionPickerCell subdivisionCell)
        {
            subdivisionCell.IsReadOnly = true;
            if (sender.IsValid) {
                subdivisionCell.Subdivisions = await GetSubdivisions (profile.Zip);
            }

            subdivisionCell.IsReadOnly = !sender.IsValid;
        }

        private async Task<SubdivisionDTO[]> GetSubdivisions (string zipCode)
        {
            if (!string.IsNullOrWhiteSpace (zipCode)) {
                var subdivisionsResult = await profileService.GetSubdivisionsAsync (zipCode);
                if (subdivisionsResult.IsSuccess) {
                    return subdivisionsResult.Data;
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