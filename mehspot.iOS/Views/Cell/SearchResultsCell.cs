using System;

using Foundation;
using UIKit;

namespace mehspot.iOS.Views.Cell
{
    public partial class SearchResultsCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString ("SearchResultsCell");
        public static readonly UINib Nib;

        static SearchResultsCell ()
        {
            Nib = UINib.FromName ("SearchResultsCell", NSBundle.MainBundle);
        }

        public Action<UIButton> SendMessageButtonAction;
        public Action<UIButton> ViewProfileButtonAction;

        protected SearchResultsCell (IntPtr handle) : base (handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        partial void SendMessageButtonTouched (UIButton sender)
        {
            SendMessageButtonAction?.Invoke (sender);
        }

        partial void ViewProfileButtonTouched (UIButton sender)
        {
            ViewProfileButtonAction?.Invoke (sender);
        }
    }
}
