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

namespace mehspot.iOS
{
    public partial class EditProfileController : UIViewController, IUITableViewDataSource, IUITableViewDelegate
    {
        private readonly ProfileService profileService;
        private List<UITableViewCell> cells = new List<UITableViewCell> ();
        private KeyValuePair<int?, string> [] subdivisions;

        public EditProfileDto profile;


        public EditProfileController (IntPtr handle) : base (handle)
        {
            profileService = new ProfileService (MehspotAppContext.Instance.DataStorage);
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
            ProfileTableView.ReloadData();
        }

        partial void SaveButtonTouched (UIBarButtonItem sender)
        {
        }

        private async Task InitializeCells ()
        {
            var genders = new [] {
                new KeyValuePair<string, string>(null, "N/A"),
                new KeyValuePair<string, string>("M", "Male"),
                new KeyValuePair<string, string>("F", "Female")
            };

            var statesResult = await profileService.GetStatesAsync ();
            var states = statesResult.Data.Select (a => new KeyValuePair<int?, string> (a.Id, a.Name)).ToArray();

            subdivisions = await GetSubdivisions (profile.Zip);

            cells.Add (TextEditCell.Create (profile, a => a.UserName, "User Name", true));
            cells.Add (TextEditCell.Create (profile, a => a.Email, "Email"));
            cells.Add (TextEditCell.Create (profile, a => a.PhoneNumber, "Phone Number"));// TODO: Mask field
            cells.Add (PickerCell.Create (profile, a => a.DateOfBirth, (model, property) => { model.DateOfBirth = property; }, v => v?.ToString ("MMMM dd, yyyy"), "Date Of Birth"));
            cells.Add (PickerCell.Create (profile, a => a.Gender, (model, property) => { model.Gender = property; }, v => genders.First (a => a.Key == v).Value, "Gender", genders));
            // TODO: Email field
            cells.Add (TextEditCell.Create (profile, a => a.FirstName, "First Name"));
            cells.Add (TextEditCell.Create (profile, a => a.LastName, "Last Name"));
            cells.Add (TextEditCell.Create (profile, a => a.AddressLine1, "Address Line 1"));
            cells.Add (TextEditCell.Create (profile, a => a.AddressLine2, "Address Line 2"));
            cells.Add (PickerCell.Create (profile, a => a.State, (model, property) => { model.State = property; }, v => states.FirstOrDefault (a => a.Key == v).Value, "State", states));
            cells.Add (TextEditCell.Create (profile, a => a.City, "City"));
            var zipCell = MaskedTextEditCell.Create (profile, a => a.Zip, "Zip");
            zipCell.Mask = "#####";
            var subdivisionEnabled = !string.IsNullOrWhiteSpace(profile.Zip) && zipCell.IsValid;
            var subdivisionCell = PickerCell.Create (profile, a => a.SubdivisionId, (model, property) => { model.SubdivisionId = (int?)property; }, v => subdivisions.FirstOrDefault (a => a.Key == v).Value, "Subdivision", subdivisions, !subdivisionEnabled);
            zipCell.ValueChanged += (arg1, arg2) => ZipCell_ValueChanged (arg1, arg2, subdivisionCell);
            cells.Add (zipCell); //zip mask
            cells.Add (subdivisionCell); //Subdivision Selector

            cells.Add (BooleanEditCell.Create (profile, a => a.MehspotNotificationsEnabled, "Email notifications enabled")); //True-False selector
            cells.Add (BooleanEditCell.Create (profile, a => a.AllGroupsNotificationsEnabled, "Group notifications enabled")); //True-False selector
        }

        async void ZipCell_ValueChanged (MaskedTextEditCell sender, string value, PickerCell subdivisionCell)
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