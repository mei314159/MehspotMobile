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
            CGRect titleLabelBounds = new CGRect (CGPoint.Empty, maxSize);
            CGRect minimumTextRect = v.textLabel.TextRectForBounds (titleLabelBounds, 0);
            var titleLabelHeightDelta = minimumTextRect.Size.Height - v.textLabel.Frame.Size.Height;
            CGRect titleFrame = new CGRect (v.textLabel.Frame.Location, new CGSize (maxSize.Width, v.textLabel.Frame.Height + titleLabelHeightDelta));
            v.textLabel.Frame = titleFrame;
            v.Frame = new CGRect (v.Frame.Location, v.textLabel.Frame.Size);
            v.BackgroundColor = isMyMessage ? UIColor.FromRGB (58, 155, 252) : UIColor.FromRGB (249, 217, 128);

            return v;
        }


   }
}