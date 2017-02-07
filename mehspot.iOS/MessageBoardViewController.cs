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
            GoToMessaging ();
        }

        public override void ViewDidLoad ()
        {
            toUserNameField.ShouldReturn += TextFieldShouldReturn;
        }

        public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
        {
            var controller = (MessagingViewController)segue.DestinationViewController;
            controller.ToUserName = this.toUserNameField.Text;
            base.PrepareForSegue (segue, sender);
        }

        private bool TextFieldShouldReturn (UITextField textField)
        {
            GoToMessaging ();

            return false; // We do not want UITextField to insert line-breaks.
        }

        void GoToMessaging ()
        {
            PerformSegue ("GoToMessagingSegue", this);
        }
    }
}