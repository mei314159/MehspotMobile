using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace mehspot.iOS.Views
{
    [Register ("TextViewCell")]
    public class TextViewCell : UITableViewCell
    {
        private string mask;
        public static readonly NSString Key = new NSString ("TextViewCell");
        public static readonly UINib Nib;

        static TextViewCell ()
        {
            Nib = UINib.FromName ("TextViewCell", NSBundle.MainBundle);
        }

        protected TextViewCell (IntPtr handle) : base (handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        [Outlet]
        UILabel FieldLabel { get; set; }

        [Outlet]
        public UITextView Text { get; set; }

        public static TextViewCell Create (string text, string label)
        {
            var cell = (TextViewCell)Nib.Instantiate (null, null) [0];
            cell.SelectionStyle = UITableViewCellSelectionStyle.None;
            cell.FieldLabel.Text = label;
            cell.Text.Text = text;

            var textSize = cell.Text.SizeThatFits (new CGSize (cell.Text.Frame.Width, nfloat.MaxValue));
            var height = textSize.Height > cell.Text.Frame.Height ? textSize.Height : cell.Text.Frame.Height;
            cell.Text.Frame = new CGRect (cell.Text.Frame.Location, new CGSize(textSize.Width, height));
            cell.Frame = new CGRect (cell.Frame.Location, new CGSize (cell.Frame.Width, height + 10));

            return cell;
        }

        void ReleaseDesignerOutlets ()
        {
            if (FieldLabel != null) {
                FieldLabel.Dispose ();
                FieldLabel = null;
            }

            if (Text != null) {
                Text.Dispose ();
                Text = null;
            }
        }
    }
}
