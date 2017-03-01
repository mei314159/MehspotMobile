using Foundation;
using System;
using UIKit;
using ObjCRuntime;

namespace mehspot.iOS
{
    [Register ("BadgeItemView")]
    public class BadgeItemView : UIView
    {
        public BadgeItemView (IntPtr handle) : base (handle)
        {
        }

        [Outlet]
        public UIImageView BadgeImage { get; set; }

        [Outlet]
        public UILabel BadgeName { get; set; }

        [Outlet]
        public UILabel LikesCount { get; set; }

        [Outlet]
        public UILabel RecommendationsCount { get; set; }

        [Outlet]
        public UILabel ReferencesCount { get; set; }


        public static BadgeItemView Create ()
        {

            var arr = NSBundle.MainBundle.LoadNib ("BadgeItemView", null, null);


            var v = Runtime.GetNSObject<BadgeItemView> (arr.ValueAt (0));
            return v;
        }

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