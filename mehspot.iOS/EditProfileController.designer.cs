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
    [Register ("EditProfileController")]
    partial class EditProfileController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView ProfilePicture { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView ProfileTableView { get; set; }

        [Action ("ChangePhotoButtonTouched:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ChangePhotoButtonTouched (UIKit.UIButton sender);

        [Action ("SaveButtonTouched:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SaveButtonTouched (UIKit.UIBarButtonItem sender);

        void ReleaseDesignerOutlets ()
        {
            if (ProfilePicture != null) {
                ProfilePicture.Dispose ();
                ProfilePicture = null;
            }

            if (ProfileTableView != null) {
                ProfileTableView.Dispose ();
                ProfileTableView = null;
            }
        }
    }
}