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
    [Register ("NoResultsView")]
    partial class NoResultsView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel Message { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton RegisterButton { get; set; }

        [Action ("RegisterButtonTouched:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void RegisterButtonTouched (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (Message != null) {
                Message.Dispose ();
                Message = null;
            }

            if (RegisterButton != null) {
                RegisterButton.Dispose ();
                RegisterButton = null;
            }
        }
    }
}