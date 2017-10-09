// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace Mehspot.iOS
{
    [Register ("GroupMessagingViewController")]
    partial class GroupMessagingViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem InfoButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView messageFieldWrapper { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint messagesBottomMargin { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView messagesList { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint MessageWrapperBottomConstraint { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UINavigationItem NavBar { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton sendButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView textField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint TextViewHeightConstraint { get; set; }

        [Action ("CloseButtonTouched:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void CloseButtonTouched (UIKit.UIBarButtonItem sender);

        [Action ("InfoButtonTouched:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void InfoButtonTouched (UIKit.UIBarButtonItem sender);

        [Action ("SendButtonTouched:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SendButtonTouched (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (InfoButton != null) {
                InfoButton.Dispose ();
                InfoButton = null;
            }

            if (messageFieldWrapper != null) {
                messageFieldWrapper.Dispose ();
                messageFieldWrapper = null;
            }

            if (messagesBottomMargin != null) {
                messagesBottomMargin.Dispose ();
                messagesBottomMargin = null;
            }

            if (messagesList != null) {
                messagesList.Dispose ();
                messagesList = null;
            }

            if (MessageWrapperBottomConstraint != null) {
                MessageWrapperBottomConstraint.Dispose ();
                MessageWrapperBottomConstraint = null;
            }

            if (NavBar != null) {
                NavBar.Dispose ();
                NavBar = null;
            }

            if (sendButton != null) {
                sendButton.Dispose ();
                sendButton = null;
            }

            if (textField != null) {
                textField.Dispose ();
                textField = null;
            }

            if (TextViewHeightConstraint != null) {
                TextViewHeightConstraint.Dispose ();
                TextViewHeightConstraint = null;
            }
        }
    }
}