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
        private AccountService accountService;

        ResetPasswordModel model;
        public ForgotPasswordViewController (IntPtr handle) : base (handle)
        {
            model = new ResetPasswordModel (MehspotAppContext.Instance.AuthManager, new ViewHelper (this.View));
            model.OnSuccess += Model_OnSuccess;
        }

        public override void ViewDidLoad ()
        {
            this.View.AddGestureRecognizer (new UITapGestureRecognizer (this.HideKeyboard));
            this.EmailField.ShouldReturn += TextFieldShouldReturn;
        }

        partial void ResetPasswordButtonTouched (UIButton sender)
        {
            sender.BecomeFirstResponder ();
            ResetPasswordAsync ();
        }

        private void Model_OnSuccess (Result result)
        {
            var targetViewController = UIStoryboard.FromName ("Main", null).InstantiateInitialViewController ();
            this.View.Window.SwapController (targetViewController);
        }

        void Model_OnError (AuthenticationResult obj)
        {
        }

        private async void ResetPasswordAsync ()
        {
            await model.ResetPasswordAsync (this.EmailField.Text);
        }

        private bool TextFieldShouldReturn (UITextField textField)
        {
            textField.ResignFirstResponder ();
            ResetPasswordAsync ();

            return false; // We do not want UITextField to insert line-breaks.
        }
    }
}