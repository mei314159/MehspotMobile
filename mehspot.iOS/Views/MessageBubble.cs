using Foundation;
using System;
using UIKit;
using ObjCRuntime;
using CoreGraphics;

namespace mehspot.iOS
{
    public partial class MessageBubble : UIView
    {
        public MessageBubble (IntPtr handle) : base (handle)
        {
        }

        public static MessageBubble Create (CGSize maxSize, string text, bool isMyMessage)
        {
            
            var arr = NSBundle.MainBundle.LoadNib ("MessageBubble", null, null);


            var v = Runtime.GetNSObject<MessageBubble> (arr.ValueAt (0));

            v.textLabel.Text = text;
            CGRect titleLabelBounds = new CGRect (CGPoint.Empty, new CGSize (maxSize.Width - 20, maxSize.Height - 20));
            CGRect minimumTextRect = v.textLabel.TextRectForBounds (titleLabelBounds, 0);
            var titleLabelHeightDelta = minimumTextRect.Size.Height - v.textLabel.Frame.Size.Height;
            v.textLabel.Frame = new CGRect(10, 10, titleLabelBounds.Width, v.textLabel.Frame.Height + titleLabelHeightDelta);
            v.Frame = new CGRect (v.Frame.Location, new CGSize (v.textLabel.Frame.Size.Width + 20, v.textLabel.Frame.Size.Height + 20));
            v.BackgroundColor = isMyMessage ? UIColor.FromRGB (58, 155, 252) : UIColor.FromRGB (249, 217, 128);

            return v;
        }


   }
}