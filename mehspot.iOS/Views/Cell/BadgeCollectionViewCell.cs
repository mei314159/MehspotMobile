using System;

using Foundation;
using UIKit;

namespace mehspot.iOS.Views.Cell
{
    public partial class BadgeCollectionViewCell : UICollectionViewCell
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
    }
}
