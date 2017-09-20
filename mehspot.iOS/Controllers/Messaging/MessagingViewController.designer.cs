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
    [Register ("MessagingViewController")]
    partial class MessagingViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView messageFieldWrapper { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView messagesList { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint MessageWrapperBottomConstraint { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton sendButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView textField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint TextViewHeightConstraint { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel TitleLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton UserPic { get; set; }

        [Action ("CloseButtonTouched:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void CloseButtonTouched (UIKit.UIButton sender);

        [Action ("SendButtonTouched:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SendButtonTouched (UIKit.UIButton sender);

        [Action ("UserPicTouched:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void UserPicTouched (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (messageFieldWrapper != null) {
                messageFieldWrapper.Dispose ();
                messageFieldWrapper = null;
            }

            if (messagesList != null) {
                messagesList.Dispose ();
                messagesList = null;
            }

            if (MessageWrapperBottomConstraint != null) {
                MessageWrapperBottomConstraint.Dispose ();
                MessageWrapperBottomConstraint = null;
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

            if (TitleLabel != null) {
                TitleLabel.Dispose ();
                TitleLabel = null;
            }

            if (UserPic != null) {
                UserPic.Dispose ();
                UserPic = null;
            }
        }
    }
}