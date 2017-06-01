using System;
using UIKit;
using Mehspot.Core.Contracts.Wrappers;
using Mehspot.iOS.Wrappers;
using Mehspot.iOS.Extensions;
using System.Threading.Tasks;
using CoreGraphics;
using Mehspot.iOS.Controllers.Badges.BadgeProfileDataSource;
using Mehspot.Core;
using Mehspot.Core.Services;
using Mehspot.Core.DTO.Badges;
using System.Linq;

namespace Mehspot.iOS
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
        public bool RedirectToSearchResults;

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
            TableView.AddGestureRecognizer (new UITapGestureRecognizer (this.HideKeyboard));
            this.RefreshControl.ValueChanged += RefreshControl_ValueChanged;
        }

        public override void PrepareForSegue (UIStoryboardSegue segue, Foundation.NSObject sender)
        {
            if (segue.Identifier == "UnwindToSearchResults") {
                var controller = (SearchResultsViewController)segue.DestinationViewController;
                controller.ReqiredBadgeWasRegistered ();
            }

            base.PrepareForSegue (segue, sender);
        }

        private void SetTitle ()
        {
            var badgeName = MehspotResources.ResourceManager.GetString (this.BadgeName) ?? this.BadgeName;
            var title =
                BadgeIsRegistered ?
                "Update " + (this.BadgeName == Constants.BadgeNames.BabysitterEmployer ? "babysitting (employer) page" : badgeName)
                : "Sign up " +
                (this.BadgeName == Constants.BadgeNames.Fitness || this.BadgeName == Constants.BadgeNames.Golf || this.BadgeName == Constants.BadgeNames.OtherJobs ? "for " : "as ") + badgeName;

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

        async partial void SaveButtonTouched (UIBarButtonItem sender)
        {
            sender.Enabled = false;
            this.HideKeyboard ();
            viewHelper.ShowOverlay ("Saving...");

            var result = await this.badgeService.SaveBadgeProfileAsync (profile);
            viewHelper.HideOverlay ();
            string message;
            if (result.IsSuccess) {
                if (!BadgeIsRegistered) {
                    BadgeIsRegistered = this.profile.IsEnabled = true;
                    ((EditBadgeTableSource)TableView.Source).ShowToggleEnabledCell ();
                    TableView.ReloadData ();
                }
                SetTitle ();
                message = $"{BadgeName} badge profile successfully saved";
            } else {
                var errors = result.ModelState.ModelState?.SelectMany (a => a.Value);
                message = errors != null ? string.Join (Environment.NewLine, errors) : result.ErrorMessage;
            }
            var alert = new UIAlertView (result.IsSuccess ? "Success" : "Error", message, (IUIAlertViewDelegate)null, "OK");
            alert.Clicked += (object s, UIButtonEventArgs e) => {
                if (result.IsSuccess && RedirectToSearchResults) {
                    PerformSegue ("UnwindToSearchResults", this);
                }
            };
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
            TableView.SetContentOffset (new CGPoint (0, -this.RefreshControl.Frame.Size.Height), true);
            var profileResult = await badgeService.GetMyBadgeProfileAsync (this.BadgeId);
            dataLoaded = profileResult.IsSuccess;
            if (profileResult.IsSuccess) {
                this.profile = profileResult.Data;
                var tableSource = await EditBadgeTableSource.Create (profileResult.Data, subdivisionService);
                tableSource.OnEnabledStateChanged += TableSource_OnEnabledStateChanged;
                tableSource.OnDeleteBadgeButtonTouched += TableSource_OnDeleteBadgeButtonTouched;
                base.TableView.Source = tableSource;
                TableView.ReloadData ();
                TableView.UserInteractionEnabled = this.SaveButton.Enabled = true;
                TableView.SetContentOffset (CGPoint.Empty, true);
                RefreshControl.EndRefreshing ();
            } else {
                viewHelper.ShowAlert ("Error", "Can not load profile data");
                TableView.SetContentOffset (CGPoint.Empty, true);
                RefreshControl.EndRefreshing ();
                return;
            }

            loading = false;
        }

        async void TableSource_OnEnabledStateChanged (bool isEnabled)
        {
            await badgeService.ToggleEnabledState (this.BadgeId, isEnabled);
        }

        void TableSource_OnDeleteBadgeButtonTouched ()
        {
            UIAlertView alert = new UIAlertView (
                                            "Delete Badge",
                                            "Do you want to delete this badge registration?",
                                            (IUIAlertViewDelegate)null,
                                            "Cancel",
                                            new string [] { "Yes, I do" });
            alert.Clicked += async (object sender, UIButtonEventArgs e) => {
                MehspotAppContext.Instance.DataStorage.PushIsEnabled = true;
                if (e.ButtonIndex != alert.CancelButtonIndex) {

                    var result = await badgeService.DeleteBadgeAsync (this.BadgeId);
                    if (result.IsSuccess) {
                        this.NavigationController.PopViewController (true);
                    } else {
                        viewHelper.ShowAlert ("Error", result.ErrorMessage);
                    }
                }
            };

            alert.Show ();
        }
    }
}