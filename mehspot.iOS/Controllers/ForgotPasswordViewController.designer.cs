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
    [Register ("ForgotPasswordViewController")]
    partial class ForgotPasswordViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView ContentWrapper { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField EmailField { get; set; }

        [Action ("ResetPasswordButtonTouched:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ResetPasswordButtonTouched (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (ContentWrapper != null) {
                ContentWrapper.Dispose ();
                ContentWrapper = null;
            }

            if (EmailField != null) {
                EmailField.Dispose ();
                EmailField = null;
            }
        }
    }
}