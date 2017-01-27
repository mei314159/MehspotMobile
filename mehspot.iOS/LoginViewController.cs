using System;
using UIKit;
using Mehspot.Core.Models;
using mehspot.iOS.Wrappers;
using mehspot.Core.Auth;

namespace mehspot.iOS
{
    public partial class LoginViewController : UIViewController
    {
        SignInModel model;
        public LoginViewController (IntPtr handle) : base (handle)
        {
            model = new SignInModel (AppDelegate.AuthManager, new ViewHelper (this.View));
            model.SignedIn += Model_SignedIn;
        }

        async partial void SignInButtonTouched (UIButton sender)
        {
            await model.SignInAsync (this.EmailField.Text, this.PasswordField.Text);
        }

        void Model_SignedIn (AuthenticationResult result)
        {
            var targetViewController = UIStoryboard.FromName ("Main", null).InstantiateInitialViewController ();
            SwapRootView (targetViewController, UIViewAnimationOptions.TransitionFlipFromRight);
        }

        private static void SwapRootView (UIViewController newView, UIViewAnimationOptions opt)
        {
            UIView.Transition (UIApplication.SharedApplication.KeyWindow, 0.5, opt, delegate {
                UIApplication.SharedApplication.KeyWindow.RootViewController = newView;

            }, null);
        }
    }
}