using Foundation;
using System;
using UIKit;
using System.Collections.Generic;
using Mehspot.Core.Messaging;
using Mehspot.Core.DTO;
using Mehspot.Core;
using CoreGraphics;
using System.Threading.Tasks;
using SDWebImage;
using mehspot.iOS.Views;
using System.Linq;
using MehSpot.Models.ViewModels;

namespace mehspot.iOS
{
    public partial class ViewProfileViewController : UIViewController, IUITableViewDataSource, IUITableViewDelegate
    {
        volatile bool loading;
        private BadgeService badgeService;

        BadgeProfileDTO<BabysitterProfileDTO> profile;

        private List<UITableViewCell> cells = new List<UITableViewCell> ();

        public ViewProfileViewController (IntPtr handle) : base (handle)
        {
        }

        public string UserId { get; set; }

        public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
        {
            if (segue.Identifier == "GoToMessagingSegue") {
                var controller = (MessagingViewController)segue.DestinationViewController;
                controller.ToUserId = profile.Details.UserId;
                controller.ToUserName = profile.Details.UserName;
                controller.ParentController = this;
            }

            base.PrepareForSegue (segue, sender);
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
            SendMessageButton.Layer.BorderWidth = 1;
            SendMessageButton.Layer.BorderColor = UIColor.LightGray.CGColor;
            this.NavBar.TopItem.Title = BadgeService.BadgeNames.Babysitter + " Profile";
            TableView.Delegate = this;
            TableView.WeakDataSource = this;
            TableView.TableHeaderView.Hidden = true;
            TableView.TableFooterView = new UIView ();
            badgeService = new BadgeService (MehspotAppContext.Instance.DataStorage);
        }

        public override async void ViewDidAppear (bool animated)
        {
            await RefreshView ();
            TableView.TableHeaderView.Hidden = false;
        }

        partial void CloseButtonTouched (UIBarButtonItem sender)
        {
            this.DismissViewController (true, null);
        }

        private async Task RefreshView ()
        {
            if (loading)
                return;
            loading = true;
            TableView.UserInteractionEnabled = false;

            var result = await badgeService.GetBadgeProfileAsync (BadgeService.BadgeNames.Babysitter, this.UserId);

            if (result.IsSuccess) {
                profile = result.Data; 
                InitializeTable (profile);
            } else {
                new UIAlertView ("Error", "Can not load profile data", null, "OK").Show ();
                return;
            }

            SendMessageButton.Enabled = true;
            TableView.UserInteractionEnabled = true;
            loading = false;
        }

        void InitializeTable (BadgeProfileDTO<BabysitterProfileDTO> profile)
        {
            this.UserNameLabel.Text = profile.Details.UserName;
            this.FirstName.Text = profile.Details.FirstName;

            if (!string.IsNullOrEmpty (profile.Details.ProfilePicturePath)) {
                var url = NSUrl.FromString (profile.Details.ProfilePicturePath);
                if (url != null) {
                    this.ProfilePicture.SetImage (url);
                }
            }

            //cells.Clear ();
            //cells.Add (TextEditCell.Create (profile, a => a.UserName, "User Name"));
            //cells.Add (TextEditCell.Create (profile, a => a.Email, "Email", true));
            //var phoneNumberCell = TextEditCell.Create (profile, a => a.PhoneNumber, "Phone Number");
            //phoneNumberCell.Mask = "(###)###-####";
            //cells.Add (phoneNumberCell);
            //cells.Add (PickerCell.Create (profile, a => a.DateOfBirth, (model, property) => { model.DateOfBirth = property; }, v => v?.ToString ("MMMM dd, yyyy"), "Date Of Birth"));
            //cells.Add (TextEditCell.Create (profile, a => a.FirstName, "First Name"));
            //cells.Add (TextEditCell.Create (profile, a => a.LastName, "Last Name"));
            //cells.Add (TextEditCell.Create (profile, a => a.AddressLine1, "Address Line 1"));
            //cells.Add (TextEditCell.Create (profile, a => a.AddressLine2, "Address Line 2"));
            //cells.Add (PickerCell.Create (profile, a => a.State, (model, property) => { model.State = property; }, v => states.FirstOrDefault (a => a.Key == v).Value, "State", states));
            //cells.Add (TextEditCell.Create (profile, a => a.City, "City"));
            //var zipCell = TextEditCell.Create (profile, a => a.Zip, "Zip");
            //zipCell.Mask = "#####";
            //var subdivisionEnabled = !string.IsNullOrWhiteSpace (profile.Zip) && zipCell.IsValid;
            //var subdivisionCell = PickerCell.Create (profile, a => a.SubdivisionId, (model, property) => { model.SubdivisionId = (int?)property; }, v => subdivisions.FirstOrDefault (a => a.Key == v).Value, "Subdivision", subdivisions, !subdivisionEnabled);
            //zipCell.ValueChanged += (arg1, arg2) => ZipCell_ValueChanged (arg1, arg2, subdivisionCell);
            //cells.Add (zipCell);
            //cells.Add (subdivisionCell);

            //cells.Add (BooleanEditCell.Create (profile, a => a.MehspotNotificationsEnabled, "Email notifications enabled"));
            //cells.Add (BooleanEditCell.Create (profile, a => a.AllGroupsNotificationsEnabled, "Group notifications enabled"));

            //TableView.ReloadData ();
        }
    }
}