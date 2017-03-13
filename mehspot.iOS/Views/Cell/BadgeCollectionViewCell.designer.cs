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
    [Register ("BadgeCollectionViewCell")]
    partial class BadgeCollectionViewCell
    {
        void ReleaseDesignerOutlets ()
        {
            if (BadgeImage != null) {
                BadgeImage.Dispose ();
                BadgeImage = null;
            }

            if (BadgeName != null) {
                BadgeName.Dispose ();
                BadgeName = null;
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
        }
    }
}