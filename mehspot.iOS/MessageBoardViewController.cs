using Foundation;
using System;
using UIKit;

namespace mehspot.iOS
{
    public partial class MessageBoardViewController : UIViewController
    {
        public MessageBoardViewController (IntPtr handle) : base (handle)
        {
        }

        partial void SubmitButtonTouched (UIButton sender)
        {
            PerformSegue ("GoToMessagingSegue", this);
        }

        public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
        {
            var controller = (MessagingViewController)segue.DestinationViewController;
            controller.ToUserName = this.toUserNameField.Text;
            base.PrepareForSegue (segue, sender);
        }
    }
}