using System;

using Foundation;
using UIKit;

namespace mehspot.iOS.Views.Cell
{
    [Register ("BadgeCollectionViewCell")]
    public class BadgeCollectionViewCell : UICollectionViewCell
    {
        public static readonly NSString Key = new NSString ("BadgeCollectionViewCell");
        public static readonly UINib Nib;

        static BadgeCollectionViewCell ()
        {
            Nib = UINib.FromName ("BadgeCollectionViewCell", NSBundle.MainBundle);
        }

        protected BadgeCollectionViewCell (IntPtr handle) : base (handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        [Outlet]
        public UIKit.UIImageView BadgeImage { get; set; }

        [Outlet]
        public UIKit.UILabel BadgeName { get; set; }

        [Outlet]
        public UIKit.UILabel LikesCount { get; set; }

        [Outlet]
        public UIKit.UILabel RecommendationsCount { get; set; }

        [Outlet]
        public UIKit.UILabel ReferencesCount { get; set; }

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
