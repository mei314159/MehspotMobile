using System;
using UIKit;
using Mehspot.Core.Models;
using mehspot.iOS.Wrappers;
using mehspot.Core.Auth;
using Mehspot.Core;
using mehspot.iOS.Extensions;
using Foundation;
using Facebook.CoreKit;
using Facebook.LoginKit;
using CoreGraphics;
using System.Collections.Generic;

namespace mehspot.iOS
{
    public partial class LoginViewController : UIViewController
    {
        List<string> readPermissions = new List<string> { "public_profile", "email" };

        SignInModel model;
        LoginButton loginView;
        public LoginViewController (IntPtr handle) : base (handle)
        {
            model = new SignInModel (MehspotAppContext.Instance.AuthManager, new ViewHelper (this.View));
            model.SignedIn += Model_SignedIn;
            model.SignInError += Model_SignInError;
        }

        [Action ("UnwindToLoginViewController:")]
        public void UnwindToLoginViewController (UIStoryboardSegue segue)
        {
        }

        public override void ViewDidLoad ()
        {
            this.View.AddGestureRecognizer (new UITapGestureRecognizer (HideKeyboard));
            this.EmailField.ShouldReturn += TextFieldShouldReturn;
            this.PasswordField.ShouldReturn += TextFieldShouldReturn;
            Profile.Notifications.ObserveDidChange ((sender, e) => {

                if (e.NewProfile == null)
                    return;

                //nameLabel.Text = e.NewProfile.Name;
            });

            // Set the Read and Publish permissions you want to get
            loginView = new LoginButton (new CGRect (0, 0, 218, 46)) {
                LoginBehavior = LoginBehavior.Browser,
                ReadPermissions = readPermissions.ToArray ()
            };

            // Handle actions once the user is logged in
            loginView.Completed += async (sender, e) => {
                if (e.Error != null) {

                }

                if (e.Result.IsCancelled) {
                    // Handle if the user cancelled the login request
                }

                await model.SignInExternalAsync (e.Result.Token.TokenString, "Facebook");
            };
            this.FbAuthButtonWrapper.AddSubview (loginView);
        }

        partial void SignInButtonTouched (UIButton sender)
        {
            sender.BecomeFirstResponder ();
            SignInAsync ();
        }

        private void Model_SignedIn (AuthenticationResult result)
        {
            var targetViewController = UIStoryboard.FromName ("Main", null).InstantiateInitialViewController ();
            this.View.Window.SwapController (targetViewController);
        }

        private async void SignInAsync ()
        {
            await model.SignInAsync (this.EmailField.Text, this.PasswordField.Text);
        }

        void Model_SignInError (AuthenticationResult result)
        {
            new LoginManager ().LogOut ();
        }

        private bool TextFieldShouldReturn (UITextField textField)
        {
            var nextTag = textField.Tag + 1;
            UIResponder nextResponder = this.View.ViewWithTag (nextTag);
            if (nextResponder != null && nextTag < 2) {
                nextResponder.BecomeFirstResponder ();
            } else {
                // Not found, so remove keyboard.
                textField.ResignFirstResponder ();
                SignInAsync ();
            }

            return false; // We do not want UITextField to insert line-breaks.
        }

        public void HideKeyboard ()
        {
            this.View.FindFirstResponder ()?.ResignFirstResponder ();
        }
    }
}