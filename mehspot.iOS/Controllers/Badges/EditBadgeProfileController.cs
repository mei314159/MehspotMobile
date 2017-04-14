using System;
using UIKit;
using Mehspot.Core.Contracts.Wrappers;
using mehspot.iOS.Wrappers;
using mehspot.iOS.Extensions;
using System.Threading.Tasks;
using CoreGraphics;
using mehspot.iOS.Controllers.Badges.BadgeProfileDataSource;
using Mehspot.Core;
using Mehspot.Core.Services;
using Mehspot.Core.DTO.Badges;
using System.Linq;

namespace mehspot.iOS
{

    public partial class EditBadgeProfileController : UITableViewController
    {
        volatile bool loading;
        volatile bool dataLoaded;
        private BadgeService badgeService;
        private SubdivisionService subdivisionService;
        private IViewHelper viewHelper;

        public int BadgeId;
        public string BadgeName;
        public bool BadgeIsRegistered;

        BadgeProfileDTO<EditBadgeProfileDTO> profile;


        public EditBadgeProfileController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad ()
        {
            SetTitle ();
            badgeService = new BadgeService (MehspotAppContext.Instance.DataStorage);
            subdivisionService = new SubdivisionService (MehspotAppContext.Instance.DataStorage);
            viewHelper = new ViewHelper (this.View);
            TableView.AddGestureRecognizer (new UITapGestureRecognizer (HideKeyboard));
            RefreshControl.ValueChanged += RefreshControl_ValueChanged;
        }

        private void SetTitle () {

            var badgeName = MehspotResources.ResourceManager.GetString (this.BadgeName) ?? this.BadgeName;
            var title =
                BadgeIsRegistered ?
                "Update " + (this.BadgeName == BadgeService.BadgeNames.BabysitterEmployer ? "babysitting (employer) page" : badgeName)
                : "Sign up " + (this.BadgeName == BadgeService.BadgeNames.Fitness ? "for " : this.BadgeName == BadgeService.BadgeNames.Babysitter ? "as " : string.Empty) + badgeName;

            this.NavBar.Title = title;
        }

        public override async void ViewDidAppear (bool animated)
        {
            if (!dataLoaded) {
                await RefreshView ();
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


        async partial void SaveButtonTouched (UIBarButtonItem sender)
        {
            sender.Enabled = false;
            HideKeyboard ();
            viewHelper.ShowOverlay ("Saving...");

            var result = await this.badgeService.SaveBadgeProfileAsync (profile);
            viewHelper.HideOverlay ();
            string message;
            if (result.IsSuccess) {
                if (!BadgeIsRegistered)
                    BadgeIsRegistered = true;
                SetTitle ();
                message = $"{BadgeName} badge profile successfully saved";
            } else {
                var errors = result.ModelState.ModelState?.SelectMany (a => a.Value);
                message = errors != null ? string.Join (Environment.NewLine, errors) : result.ErrorMessage;
            }
            var alert = new UIAlertView (
                                    result.IsSuccess ? "Success" : "Error",
                                    message,
                                    null,
                                    "OK");
            alert.Show ();

            sender.Enabled = true;
        }


        private async Task RefreshView ()
        {
            if (loading)
                return;
            loading = true;
            TableView.UserInteractionEnabled = this.SaveButton.Enabled = false;
            RefreshControl.BeginRefreshing ();
            TableView.SetContentOffset (new CGPoint (0, -this.TableView.RefreshControl.Frame.Size.Height), true);
            var profileResult = await badgeService.GetMyBadgeProfileAsync (this.BadgeId);
            dataLoaded = profileResult.IsSuccess;
            if (profileResult.IsSuccess) {
                this.profile = profileResult.Data;
                base.TableView.Source = await EditBadgeTableSource.Create (profileResult.Data, subdivisionService);
                TableView.ReloadData ();
                TableView.UserInteractionEnabled = this.SaveButton.Enabled = true;
                RefreshControl.EndRefreshing ();
            } else {
                new UIAlertView ("Error", "Can not load profile data", null, "OK").Show ();
                RefreshControl.EndRefreshing ();
                return;
            }

            loading = false;
        }
    }
}