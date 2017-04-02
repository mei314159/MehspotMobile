using System;

using Foundation;
using Mehspot.Core.DTO;
using UIKit;

namespace mehspot.iOS.Views
{
    public partial class BadgeItemCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString ("BadgeItemCell");
        public static readonly UINib Nib;

        static BadgeItemCell ()
        {
            Nib = UINib.FromName ("BadgeItemCell", NSBundle.MainBundle);
        }

        protected BadgeItemCell (IntPtr handle) : base (handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public BadgeSummaryDTO BadgeSummary { get; set; }

        public Action<UIButton> SearchButtonTouch { get; set; }
        public Action<UIButton> BadgeRegisterButtonTouch { get; set; }

        partial void SearchButtonTouched (UIButton sender)
        {
            SearchButtonTouch?.Invoke (sender);
        }

        partial void BadgeRegisterButtonTouched (UIButton sender)
        {
            BadgeRegisterButtonTouch?.Invoke (sender);
        }
    }
}
