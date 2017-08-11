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
    [Register ("WalkthroughStep1Controller")]
    partial class WalkthroughStep1Controller
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ContinueButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton PictureButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView ProfilePicture { get; set; }

        [Action ("ContinueButtonTouched:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ContinueButtonTouched (UIKit.UIButton sender);

        [Action ("PictureButtonTouched:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void PictureButtonTouched (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (ContinueButton != null) {
                ContinueButton.Dispose ();
                ContinueButton = null;
            }

            if (PictureButton != null) {
                PictureButton.Dispose ();
                PictureButton = null;
            }

            if (ProfilePicture != null) {
                ProfilePicture.Dispose ();
                ProfilePicture = null;
            }
        }
    }
}