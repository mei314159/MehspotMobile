using System;
using UIKit;
using Mehspot.Core.Models;
using mehspot.iOS.Wrappers;
using mehspot.Core.Auth;
using Mehspot.Core;
using mehspot.iOS.Extensions;

namespace mehspot.iOS
{
    public partial class LoginViewController : UIViewController
    {
        SignInModel model;
        public LoginViewController (IntPtr handle) : base (handle)
        {
            model = new SignInModel (MehspotAppContext.Instance.AuthManager, new ViewHelper (this.View));
            model.SignedIn += Model_SignedIn;
        }

        public override void ViewDidLoad ()
        {
            this.EmailField.ShouldReturn += TextFieldShouldReturn;
            this.PasswordField.ShouldReturn += TextFieldShouldReturn;
        }

        partial void SignInButtonTouched (UIButton sender)
        {
            sender.BecomeFirstResponder ();
            SignInAsync ();
        }

        private void Model_SignedIn (AuthenticationResult result)
        {
            var targetViewController = UIStoryboard.FromName ("Main", null).InstantiateInitialViewController ();
            targetViewController.SwapController (UIViewAnimationOptions.TransitionFlipFromRight);
        }

        private async void SignInAsync ()
        {
            await model.SignInAsync (this.EmailField.Text, this.PasswordField.Text);
        }

        private bool TextFieldShouldReturn (UITextField textField)
        {
            var nextTag = textField.Tag + 1;
            UIResponder nextResponder = this.View.ViewWithTag (nextTag);
            if (nextResponder != null) {
                nextResponder.BecomeFirstResponder ();
            } else {
                // Not found, so remove keyboard.
                textField.ResignFirstResponder ();
                SignInAsync ();
            }

            return false; // We do not want UITextField to insert line-breaks.
        }
    }
}