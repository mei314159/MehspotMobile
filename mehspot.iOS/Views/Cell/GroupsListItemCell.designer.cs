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

namespace mehspot.iOS
{
    [Register ("GroupsListItemCell")]
    partial class GroupsListItemCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel GroupName { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView GroupType { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel GroupTypeName { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LastMessage { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView UnreadIcon { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (GroupName != null) {
                GroupName.Dispose ();
                GroupName = null;
            }

            if (GroupType != null) {
                GroupType.Dispose ();
                GroupType = null;
            }

            if (GroupTypeName != null) {
                GroupTypeName.Dispose ();
                GroupTypeName = null;
            }

            if (LastMessage != null) {
                LastMessage.Dispose ();
                LastMessage = null;
            }

            if (UnreadIcon != null) {
                UnreadIcon.Dispose ();
                UnreadIcon = null;
            }
        }
    }
}