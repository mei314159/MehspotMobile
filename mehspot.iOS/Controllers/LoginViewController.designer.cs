// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace Mehspot.iOS
{
    [Register ("LoginViewController")]
    partial class LoginViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField EmailField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView FbAuthButtonWrapper { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField PasswordField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIScrollView ScrollView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ShowPasswordButton { get; set; }

        [Action ("ShowPasswordButtonTouchedDown:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ShowPasswordButtonTouchedDown (UIKit.UIButton sender);

        [Action ("ShowPasswordButtonTouchedUp:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ShowPasswordButtonTouchedUp (UIKit.UIButton sender);

        [Action ("SignInButtonTouched:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SignInButtonTouched (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (EmailField != null) {
                EmailField.Dispose ();
                EmailField = null;
            }

            if (FbAuthButtonWrapper != null) {
                FbAuthButtonWrapper.Dispose ();
                FbAuthButtonWrapper = null;
            }

            if (PasswordField != null) {
                PasswordField.Dispose ();
                PasswordField = null;
            }

            if (ScrollView != null) {
                ScrollView.Dispose ();
                ScrollView = null;
            }

            if (ShowPasswordButton != null) {
                ShowPasswordButton.Dispose ();
                ShowPasswordButton = null;
            }
        }
    }
}