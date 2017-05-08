// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace mehspot.iOS
{
    [Register ("ResetPasswordViewController")]
    partial class ResetPasswordViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField ConfirmationPasswordField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView ContentWrapper { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField PasswordField { get; set; }

        [Action ("ResetPasswordButtonTouched:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ResetPasswordButtonTouched (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (ConfirmationPasswordField != null) {
                ConfirmationPasswordField.Dispose ();
                ConfirmationPasswordField = null;
            }

            if (ContentWrapper != null) {
                ContentWrapper.Dispose ();
                ContentWrapper = null;
            }

            if (PasswordField != null) {
                PasswordField.Dispose ();
                PasswordField = null;
            }
        }
    }
}