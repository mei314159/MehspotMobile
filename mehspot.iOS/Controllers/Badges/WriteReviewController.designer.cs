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
    [Register ("WriteReviewController")]
    partial class WriteReviewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        mehspot.iOS.ExtendedTextView CommentView { get; set; }

        [Action ("CancelButtonTouched:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void CancelButtonTouched (UIKit.UIBarButtonItem sender);

        [Action ("SendButtonTouched:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SendButtonTouched (UIKit.UIBarButtonItem sender);

        void ReleaseDesignerOutlets ()
        {
            if (CommentView != null) {
                CommentView.Dispose ();
                CommentView = null;
            }
        }
    }
}