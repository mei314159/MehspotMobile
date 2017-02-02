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
    [Register ("MessagingViewController")]
    partial class MessagingViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField messageField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIScrollView messagesScrollView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton sendButton { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (messageField != null) {
                messageField.Dispose ();
                messageField = null;
            }

            if (messagesScrollView != null) {
                messagesScrollView.Dispose ();
                messagesScrollView = null;
            }

            if (sendButton != null) {
                sendButton.Dispose ();
                sendButton = null;
            }
        }
    }
}