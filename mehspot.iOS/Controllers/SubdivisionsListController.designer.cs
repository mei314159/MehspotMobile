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
    [Register ("SubdivisionsListController")]
    partial class SubdivisionsListController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView MapWrapperView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIPickerView PickerView { get; set; }

        [Action ("CloseButtonTouched:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void CloseButtonTouched (UIKit.UIBarButtonItem sender);

        [Action ("SaveButtonTouched:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SaveButtonTouched (UIKit.UIBarButtonItem sender);

        void ReleaseDesignerOutlets ()
        {
            if (MapWrapperView != null) {
                MapWrapperView.Dispose ();
                MapWrapperView = null;
            }

            if (PickerView != null) {
                PickerView.Dispose ();
                PickerView = null;
            }
        }
    }
}