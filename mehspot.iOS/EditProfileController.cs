using Foundation;
using System;
using UIKit;
using System.Collections.Generic;
using mehspot.iOS.Views;
using Mehspot.Core.DTO;

namespace mehspot.iOS
{
    public partial class EditProfileController : UIViewController, IUITableViewDataSource, IUITableViewDelegate
    {
        private List<UITableViewCell> cells = new List<UITableViewCell> ();
        public EditProfileDto profile;

        public EditProfileController (IntPtr handle) : base (handle)
        {
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
            InitializeCells ();
            ProfileTableView.WeakDataSource = this;
            ProfileTableView.Delegate = this;
        }

        partial void SaveButtonTouched (UIBarButtonItem sender)
        {
        }

        private void InitializeCells ()
        {
            cells.Add (TextEditCell.Create (profile, a => a.UserName, "User Name", true));
            cells.Add (TextEditCell.Create (profile, a => a.Email, "Email"));
            cells.Add (TextEditCell.Create (profile, a => a.PhoneNumber, "Phone Number"));// TODO: Mask field
            cells.Add (DateEditCell.Create (profile, a => a.DateOfBirth, "Date Of Birth"));// TODO: Date picker
            cells.Add (TextEditCell.Create (profile, a => a.Gender, "Gender"));// TODO: Date picker
            // TODO: Email field
            cells.Add (TextEditCell.Create (profile, a => a.FirstName, "First Name"));
            cells.Add (TextEditCell.Create (profile, a => a.LastName, "Last Name"));
            cells.Add (TextEditCell.Create (profile, a => a.AddressLine1, "Address Line 1"));
            cells.Add (TextEditCell.Create (profile, a => a.AddressLine2, "Address Line 2"));
            cells.Add (TextEditCell.Create (profile, a => a.State, "State")); // State selector
            cells.Add (TextEditCell.Create (profile, a => a.City, "City"));
            cells.Add (TextEditCell.Create (profile, a => a.Zip, "Zip")); //zip mask
            cells.Add (TextEditCell.Create (profile, a => a.SubdivisionName, "Subdivision")); //Subdivision Selector

            cells.Add (BooleanEditCell.Create (profile, a => a.MehspotNotificationsEnabled, "Email notifications enabled")); //True-False selector
            cells.Add (BooleanEditCell.Create (profile, a => a.AllGroupsNotificationsEnabled, "Group notifications enabled")); //True-False selector
        }
    }
}