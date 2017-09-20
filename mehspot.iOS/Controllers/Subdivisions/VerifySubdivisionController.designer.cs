// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace Mehspot.iOS.Controllers
{
    [Register ("VerifySubdivisionController")]
    partial class VerifySubdivisionController
    {
        [Outlet]
        UIKit.NSLayoutConstraint MainTableHeight { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView MainTable { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView MapWrapperView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton SaveButton { get; set; }


        [Action ("CloseButtonTouched:")]
        partial void CloseButtonTouched (UIKit.UIBarButtonItem sender);


        [Action ("SaveButtonTouched:")]
        partial void SaveButtonTouched (UIKit.UIBarButtonItem sender);

        [Action ("SaveButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SaveButton_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (MainTable != null) {
                MainTable.Dispose ();
                MainTable = null;
            }

            if (MainTableHeight != null) {
                MainTableHeight.Dispose ();
                MainTableHeight = null;
            }

            if (MapWrapperView != null) {
                MapWrapperView.Dispose ();
                MapWrapperView = null;
            }

            if (SaveButton != null) {
                SaveButton.Dispose ();
                SaveButton = null;
            }
        }
    }
}