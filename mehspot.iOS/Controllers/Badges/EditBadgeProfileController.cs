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

namespace mehspot.iOS
{

    public partial class EditBadgeProfileController : UITableViewController
    {
        volatile bool loading;
        volatile bool dataLoaded;
        volatile bool profileImageChanged;
        private BadgeService badgeService;
        private ProfileService profileService;
        private IViewHelper viewHelper;

        public int BadgeId;

        public EditBadgeProfileController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad ()
        {
            badgeService = new BadgeService (MehspotAppContext.Instance.DataStorage);
            profileService = new ProfileService (MehspotAppContext.Instance.DataStorage);
            viewHelper = new ViewHelper (this.View);
            TableView.AddGestureRecognizer (new UITapGestureRecognizer (HideKeyboard));
            RefreshControl.ValueChanged += RefreshControl_ValueChanged;
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
            //sender.Enabled = false;
            //HideKeyboard ();
            //viewHelper.ShowOverlay ("Saving...");

            //var result = await this.profileService.UpdateAsync (profile);
            //viewHelper.HideOverlay ();
            //if (!result.IsSuccess) {
            //    var error = string.Join (Environment.NewLine, result.ModelState.ModelState.SelectMany (a => a.Value));
            //    UIAlertView alert = new UIAlertView (
            //                        result.ErrorMessage,
            //                        error,
            //                        null,
            //                        "OK");
            //    alert.Show ();
            //}

            //this.SaveButton.Enabled = true;
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

            if (profileResult.IsSuccess) {
                TableView.Source = await RegisterBabysitterTableSource.Create (profileResult.Data, profileService);
                TableView.ReloadData ();
            } else {
                new UIAlertView ("Error", "Can not load profile data", null, "OK").Show ();
                return;
            }

            RefreshControl.EndRefreshing ();
            TableView.UserInteractionEnabled = this.SaveButton.Enabled = true;
            dataLoaded = profileResult.IsSuccess;
            loading = false;
        }
    }
}