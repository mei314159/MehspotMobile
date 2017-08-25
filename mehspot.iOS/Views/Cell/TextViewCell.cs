using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Mehspot.iOS.Views
{
    public partial class TextViewCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString("TextViewCell");
        public static readonly UINib Nib;

        static TextViewCell()
        {
            Nib = UINib.FromName("TextViewCell", NSBundle.MainBundle);
        }

        protected TextViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public static TextViewCell Create(string text, string label)
        {
            var cell = (TextViewCell)Nib.Instantiate(null, null)[0];
            cell.SelectionStyle = UITableViewCellSelectionStyle.None;
            cell.FieldLabel.Text = label;
            cell.Text.Text = text?.Trim();

            cell.UpdateSize();

            return cell;
        }

        public void UpdateSize()
        {
            var textSize = this.Text.SizeThatFits(new CGSize(this.Text.Frame.Width, nfloat.MaxValue));
            var height = textSize.Height > 31 ? textSize.Height > 100 ? 100 : textSize.Height : 31;
            TextViewHeight.Constant = height;
        }
    }
}
