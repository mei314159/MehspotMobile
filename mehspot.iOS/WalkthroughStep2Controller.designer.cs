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
    [Register ("WalkthroughStep2Controller")]
    partial class WalkthroughStep2Controller
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ContinueButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView ProfilePicture { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton SubdivisionField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint Top { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView Wrapper { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        Mehspot.iOS.ExtendedTextField ZipField { get; set; }

        [Action ("ContinueButtonTouched:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ContinueButtonTouched (UIKit.UIButton sender);

        [Action ("SubdivisionButtonTouched:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SubdivisionButtonTouched (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (ContinueButton != null) {
                ContinueButton.Dispose ();
                ContinueButton = null;
            }

            if (ProfilePicture != null) {
                ProfilePicture.Dispose ();
                ProfilePicture = null;
            }

            if (SubdivisionField != null) {
                SubdivisionField.Dispose ();
                SubdivisionField = null;
            }

            if (Top != null) {
                Top.Dispose ();
                Top = null;
            }

            if (Wrapper != null) {
                Wrapper.Dispose ();
                Wrapper = null;
            }

            if (ZipField != null) {
                ZipField.Dispose ();
                ZipField = null;
            }
        }
    }
}