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
    [Register ("ViewProfileViewController")]
    partial class ViewProfileViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel FirstName { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UINavigationBar NavBar { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView ProfilePicture { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton SendMessageButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView TableView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel UserNameLabel { get; set; }

        [Action ("CloseButtonTouched:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void CloseButtonTouched (UIKit.UIBarButtonItem sender);

        void ReleaseDesignerOutlets ()
        {
            if (FirstName != null) {
                FirstName.Dispose ();
                FirstName = null;
            }

            if (NavBar != null) {
                NavBar.Dispose ();
                NavBar = null;
            }

            if (ProfilePicture != null) {
                ProfilePicture.Dispose ();
                ProfilePicture = null;
            }

            if (SendMessageButton != null) {
                SendMessageButton.Dispose ();
                SendMessageButton = null;
            }

            if (TableView != null) {
                TableView.Dispose ();
                TableView = null;
            }

            if (UserNameLabel != null) {
                UserNameLabel.Dispose ();
                UserNameLabel = null;
            }
        }
    }
}