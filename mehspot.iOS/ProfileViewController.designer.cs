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
    [Register ("ProfileViewController")]
    partial class ProfileViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView BadgesContainer { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel FullName { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIScrollView MainScrollView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView ProfileInfoContainer { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView ProfilePicture { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton SignoutButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel UserNameLabel { get; set; }

        [Action ("SignoutButtonTouched:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SignoutButtonTouched (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (BadgesContainer != null) {
                BadgesContainer.Dispose ();
                BadgesContainer = null;
            }

            if (FullName != null) {
                FullName.Dispose ();
                FullName = null;
            }

            if (MainScrollView != null) {
                MainScrollView.Dispose ();
                MainScrollView = null;
            }

            if (ProfileInfoContainer != null) {
                ProfileInfoContainer.Dispose ();
                ProfileInfoContainer = null;
            }

            if (ProfilePicture != null) {
                ProfilePicture.Dispose ();
                ProfilePicture = null;
            }

            if (SignoutButton != null) {
                SignoutButton.Dispose ();
                SignoutButton = null;
            }

            if (UserNameLabel != null) {
                UserNameLabel.Dispose ();
                UserNameLabel = null;
            }
        }
    }
}