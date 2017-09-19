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