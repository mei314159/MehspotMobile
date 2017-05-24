// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace Mehspot.iOS.Views
{
    [Register ("BadgeItemCell")]
    partial class BadgeItemCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel BadgeDescription { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel BadgeName { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView BadgePicture { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton BadgeRegisterButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LikesCount { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel RecommendationsCount { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel ReferencesCount { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton SearchButton { get; set; }

        [Action ("BadgeRegisterButtonTouched:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void BadgeRegisterButtonTouched (UIKit.UIButton sender);

        [Action ("SearchButtonTouched:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SearchButtonTouched (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (BadgeDescription != null) {
                BadgeDescription.Dispose ();
                BadgeDescription = null;
            }

            if (BadgeName != null) {
                BadgeName.Dispose ();
                BadgeName = null;
            }

            if (BadgePicture != null) {
                BadgePicture.Dispose ();
                BadgePicture = null;
            }

            if (BadgeRegisterButton != null) {
                BadgeRegisterButton.Dispose ();
                BadgeRegisterButton = null;
            }

            if (LikesCount != null) {
                LikesCount.Dispose ();
                LikesCount = null;
            }

            if (RecommendationsCount != null) {
                RecommendationsCount.Dispose ();
                RecommendationsCount = null;
            }

            if (ReferencesCount != null) {
                ReferencesCount.Dispose ();
                ReferencesCount = null;
            }

            if (SearchButton != null) {
                SearchButton.Dispose ();
                SearchButton = null;
            }
        }
    }
}