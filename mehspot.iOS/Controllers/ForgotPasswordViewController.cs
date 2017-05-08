using System;
using UIKit;
using mehspot.Core.Auth;
using Mehspot.Core.Models;
using Mehspot.Core;
using mehspot.iOS.Extensions;
using mehspot.iOS.Wrappers;
using Mehspot.Core.DTO;

namespace mehspot.iOS
{
    public partial class ForgotPasswordViewController : UIViewController
    {
        ForgotPasswordModel model;
        public ForgotPasswordViewController (IntPtr handle) : base (handle)
        {

        }

        public override void ViewDidLoad ()
        {
            model = new ForgotPasswordModel (MehspotAppContext.Instance.AuthManager, new ViewHelper (this.View));
            model.OnSuccess += Model_OnSuccess;
            this.View.AddGestureRecognizer (new UITapGestureRecognizer (this.HideKeyboard));
            this.EmailField.ShouldReturn += TextFieldShouldReturn;
        }

        partial void ResetPasswordButtonTouched (UIButton sender)
        {
            sender.BecomeFirstResponder ();
            ForgotPasswordAsync ();
        }

        private void Model_OnSuccess (Result result)
        {
            var alert = new UIAlertView ("Success", "Please check your email to reset your password", (IUIAlertViewDelegate)null, "OK");
            alert.Clicked += (s, e) => this.PerformSegue ("UnwindToLoginSegue", this);
            alert.Show ();
        }

        private async void ForgotPasswordAsync ()
        {
            await model.ForgotPasswordAsync (this.EmailField.Text);
        }

        private bool TextFieldShouldReturn (UITextField textField)
        {
            textField.ResignFirstResponder ();
            ForgotPasswordAsync ();

            return false; // We do not want UITextField to insert line-breaks.
        }
    }
}