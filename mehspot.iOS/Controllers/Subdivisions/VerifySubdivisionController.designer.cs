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

namespace mehspot.iOS.Controllers
{
    [Register ("VerifySubdivisionController")]
    partial class VerifySubdivisionController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView MainTable { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView MapWrapperView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UINavigationBar NavBar { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UINavigationItem NavBarItem { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem SaveButton { get; set; }

        [Action ("CloseButtonTouched:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void CloseButtonTouched (UIKit.UIBarButtonItem sender);

        [Action ("SaveButtonTouched:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SaveButtonTouched (UIKit.UIBarButtonItem sender);

        void ReleaseDesignerOutlets ()
        {
            if (MainTable != null) {
                MainTable.Dispose ();
                MainTable = null;
            }

            if (MapWrapperView != null) {
                MapWrapperView.Dispose ();
                MapWrapperView = null;
            }

            if (NavBar != null) {
                NavBar.Dispose ();
                NavBar = null;
            }

            if (NavBarItem != null) {
                NavBarItem.Dispose ();
                NavBarItem = null;
            }

            if (SaveButton != null) {
                SaveButton.Dispose ();
                SaveButton = null;
            }
        }
    }
}