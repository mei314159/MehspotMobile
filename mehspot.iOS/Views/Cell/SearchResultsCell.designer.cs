// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace mehspot.iOS.Views.Cell
{
    [Register ("SearchResultsCell")]
    partial class SearchResultsCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel AgeRangeLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel DistanceLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView FavoriteIcon { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel HourlyRateLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LikesLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView ProfilePicture { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton SendMessageButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel SubdivisionLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel UserNameLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ViewProfileButton { get; set; }

        [Action ("SendMessageButtonTouched:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SendMessageButtonTouched (UIKit.UIButton sender);

        [Action ("ViewProfileButtonTouched:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ViewProfileButtonTouched (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (AgeRangeLabel != null) {
                AgeRangeLabel.Dispose ();
                AgeRangeLabel = null;
            }

            if (DistanceLabel != null) {
                DistanceLabel.Dispose ();
                DistanceLabel = null;
            }

            if (FavoriteIcon != null) {
                FavoriteIcon.Dispose ();
                FavoriteIcon = null;
            }

            if (HourlyRateLabel != null) {
                HourlyRateLabel.Dispose ();
                HourlyRateLabel = null;
            }

            if (LikesLabel != null) {
                LikesLabel.Dispose ();
                LikesLabel = null;
            }

            if (ProfilePicture != null) {
                ProfilePicture.Dispose ();
                ProfilePicture = null;
            }

            if (SendMessageButton != null) {
                SendMessageButton.Dispose ();
                SendMessageButton = null;
            }

            if (SubdivisionLabel != null) {
                SubdivisionLabel.Dispose ();
                SubdivisionLabel = null;
            }

            if (UserNameLabel != null) {
                UserNameLabel.Dispose ();
                UserNameLabel = null;
            }

            if (ViewProfileButton != null) {
                ViewProfileButton.Dispose ();
                ViewProfileButton = null;
            }
        }
    }
}