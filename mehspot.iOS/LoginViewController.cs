using Foundation;
using System;
using UIKit;

namespace mehspot.iOS
{
    public partial class LoginViewController : UIViewController
    {
        public LoginViewController (IntPtr handle) : base (handle)
        {
        }

        async partial void SignInButtonTouched (UIButton sender)
        {
            var authenticationResult = await AppDelegate.AuthManager.AuthenticateAsync (this.EmailField.Text, this.PasswordField.Text);

            if (authenticationResult.IsSuccess) {
                SwapRootView (UIStoryboard.FromName ("Main", null).InstantiateInitialViewController (), UIViewAnimationOptions.TransitionFlipFromRight);
            } else { 
                var avAlert = new UIAlertView ("Error", authenticationResult.ErrorMessage, null, "OK", null);
                avAlert.Show ();
            }
        }

        private static void SwapRootView (UIViewController newView, UIViewAnimationOptions opt)
        {
            UIView.Transition (UIApplication.SharedApplication.KeyWindow, 0.5, opt, delegate {
                UIApplication.SharedApplication.KeyWindow.RootViewController = newView;

            }, null);
        }
    }
}