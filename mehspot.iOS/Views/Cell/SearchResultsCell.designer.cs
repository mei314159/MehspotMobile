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
        public UIKit.UILabel AgeRangeLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        public UIKit.UILabel DistanceLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        public UIKit.UIImageView FavoriteIcon { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        public UIKit.UILabel HourlyRateLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        public UIKit.UILabel LikesLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        public UIKit.UIImageView ProfilePicture { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        public UIKit.UILabel SubdivisionLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        public UIKit.UILabel UserNameLabel { get; set; }

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

            if (SubdivisionLabel != null) {
                SubdivisionLabel.Dispose ();
                SubdivisionLabel = null;
            }

            if (UserNameLabel != null) {
                UserNameLabel.Dispose ();
                UserNameLabel = null;
            }
        }
    }
}