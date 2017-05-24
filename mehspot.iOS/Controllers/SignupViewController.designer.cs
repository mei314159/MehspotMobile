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

namespace Mehspot.iOS
{
    [Register ("SignupViewController")]
    partial class SignupViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISwitch AgreeWithTerms { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField ConfirmationPasswordField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView ContentWrapper { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField EmailField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField PasswordField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIScrollView ScrollView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField UserNameField { get; set; }

        [Action ("CommunityGuidelinesButtonTouched:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void CommunityGuidelinesButtonTouched (UIKit.UIButton sender);

        [Action ("PrivacyPolicyButtonTouched:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void PrivacyPolicyButtonTouched (UIKit.UIButton sender);

        [Action ("SignupButtonTouched:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SignupButtonTouched (UIKit.UIButton sender);

        [Action ("TermsofUseButtonTouched:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void TermsofUseButtonTouched (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (AgreeWithTerms != null) {
                AgreeWithTerms.Dispose ();
                AgreeWithTerms = null;
            }

            if (ConfirmationPasswordField != null) {
                ConfirmationPasswordField.Dispose ();
                ConfirmationPasswordField = null;
            }

            if (ContentWrapper != null) {
                ContentWrapper.Dispose ();
                ContentWrapper = null;
            }

            if (EmailField != null) {
                EmailField.Dispose ();
                EmailField = null;
            }

            if (PasswordField != null) {
                PasswordField.Dispose ();
                PasswordField = null;
            }

            if (ScrollView != null) {
                ScrollView.Dispose ();
                ScrollView = null;
            }

            if (UserNameField != null) {
                UserNameField.Dispose ();
                UserNameField = null;
            }
        }
    }
}