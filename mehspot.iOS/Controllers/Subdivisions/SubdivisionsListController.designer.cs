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

namespace Mehspot.iOS.Controllers
{
    [Register ("SubdivisionsListController")]
    partial class SubdivisionsListController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView ButtonsWrapper { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView MapWrapperView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIPickerView PickerView { get; set; }

        [Action ("AddButtonTouched:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void AddButtonTouched (UIKit.UIButton sender);

        [Action ("CloseButtonTouched:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void CloseButtonTouched (UIKit.UIBarButtonItem sender);

        [Action ("MoreButtonTouched:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void MoreButtonTouched (UIKit.UIButton sender);

        [Action ("SaveButtonTouched:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SaveButtonTouched (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (ButtonsWrapper != null) {
                ButtonsWrapper.Dispose ();
                ButtonsWrapper = null;
            }

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