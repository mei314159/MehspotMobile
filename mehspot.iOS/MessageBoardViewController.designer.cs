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
    [Register ("MessageBoardViewController")]
    partial class MessageBoardViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField toUserNameField { get; set; }

        [Action ("SubmitButtonTouched:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SubmitButtonTouched (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (toUserNameField != null) {
                toUserNameField.Dispose ();
                toUserNameField = null;
            }
        }
    }
}