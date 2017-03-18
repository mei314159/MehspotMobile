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
    [Register ("EditProfileViewController")]
    partial class EditProfileViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ChangePhotoButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel FullName { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView ProfilePicture { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem SaveButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel UserNameLabel { get; set; }

        [Action ("ChangePhotoButtonTouched:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ChangePhotoButtonTouched (UIKit.UIButton sender);

        [Action ("SaveButtonTouched:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SaveButtonTouched (UIKit.UIBarButtonItem sender);

        void ReleaseDesignerOutlets ()
        {
            if (ChangePhotoButton != null) {
                ChangePhotoButton.Dispose ();
                ChangePhotoButton = null;
            }

            if (FullName != null) {
                FullName.Dispose ();
                FullName = null;
            }

            if (ProfilePicture != null) {
                ProfilePicture.Dispose ();
                ProfilePicture = null;
            }

            if (SaveButton != null) {
                SaveButton.Dispose ();
                SaveButton = null;
            }

            if (UserNameLabel != null) {
                UserNameLabel.Dispose ();
                UserNameLabel = null;
            }
        }
    }
}