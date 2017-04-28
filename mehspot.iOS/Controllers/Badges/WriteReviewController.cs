using System;
using mehspot.iOS.Wrappers;
using Mehspot.Core.DTO.Badges;
using Mehspot.Core.Services;
using UIKit;

namespace mehspot.iOS
{
    public partial class WriteReviewController : UIViewController
    {
        ViewHelper viewHelper;

        public WriteReviewController (IntPtr handle) : base (handle)
        {
        }

        public int BadgeId { get; internal set; }
        public string UserId { get; internal set; }
        public BadgeService BadgeService { get; internal set; }

        public event Action<BadgeUserRecommendationDTO> OnSave;

        public override void ViewDidLoad ()
        {
            this.viewHelper = new ViewHelper (this.View);
            base.ViewDidLoad ();
        }

        partial void CancelButtonTouched (UIBarButtonItem sender)
        {
            this.DismissViewController (true, null);
        }

        async partial void SendButtonTouched (UIBarButtonItem sender)
        {
            viewHelper.ShowOverlay ("Wait...");
            var result = await this.BadgeService.WriteRecommendationAsync (BadgeId, UserId, this.CommentView.Text);
            if (result.IsSuccess) {
                this.OnSave?.Invoke (result.Data);
            } else {
                viewHelper.ShowAlert ("Meh...  Sorry for the trouble.", "Mehspot support has been notified of this issue." + Environment.NewLine + "Please try again in a later time.");    
            }

            viewHelper.HideOverlay ();
            this.DismissViewController (true, null);
        }
    }
}