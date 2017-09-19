using System;
using CoreGraphics;
using Foundation;
using Mehspot.Core;
using Mehspot.Core.DTO;
using Mehspot.Core.DTO.Groups;
using Mehspot.iOS.Extensions;
using ObjCRuntime;
using UIKit;

namespace Mehspot.iOS
{
	public partial class MessageCell : UITableViewCell
	{
		public static readonly NSString Key = new NSString("MessageCell");
		public static readonly UINib Nib;

		static MessageCell()
		{
			Nib = UINib.FromName("MessageCell", NSBundle.MainBundle);
		}

		protected MessageCell(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}


		public static MessageCell Create()
		{
			var arr = NSBundle.MainBundle.LoadNib("MessageCell", null, null);
			var v = Runtime.GetNSObject<MessageCell>(arr.ValueAt(0));
			return v;
		}

		public void Initialize(MessageDto dto)
		{
			if (dto == null)
				return;

			var isMyMessage = dto.FromUserId == MehspotAppContext.Instance.AuthManager.AuthInfo.UserId;
			this.MessageWrapper.BackgroundColor = isMyMessage ? UIColor.FromRGB(58, 155, 252) : UIColor.FromRGB(249, 217, 128);
			this.message.TextColor = isMyMessage ? UIColor.White : UIColor.DarkTextColor;
			this.message.Text = dto.Message;
			this.DateLabel.Text = dto.SentDate.AddSeconds(NSTimeZone.SystemTimeZone.GetSecondsFromGMT).ToString("MM/dd/yyyy hh:mm tt");

			CGRect titleLabelBounds = new CGRect(CGPoint.Empty, new CGSize(MessageWrapper.Frame.Width - 20, int.MaxValue - 20));
			CGRect minimumTextRect = this.message.TextRectForBounds(titleLabelBounds, 0);
			var titleLabelHeightDelta = minimumTextRect.Size.Height - this.message.Frame.Size.Height;
			this.message.Frame = new CGRect(10, 10, titleLabelBounds.Width, this.message.Frame.Height + titleLabelHeightDelta);

			var wrapperSize = new CGSize(message.Frame.Size.Width + 20, message.Frame.Size.Height + 20);
			var x = isMyMessage ? (this.Frame.Width - 10) - wrapperSize.Width : 10;
			this.MessageWrapper.Frame = new CGRect(new CGPoint(x, 10), wrapperSize);
			this.XConstraint.Constant = x;
		}

		public void Initialize(GroupMessageDTO dto)
		{
			if (dto == null)
				return;

			var isMyMessage = dto.UserId == MehspotAppContext.Instance.AuthManager.AuthInfo.UserId;
			this.MessageWrapper.BackgroundColor = isMyMessage ? UIColor.FromRGB(58, 155, 252) : UIColor.FromRGB(249, 217, 128);
			this.message.TextColor = isMyMessage ? UIColor.White : UIColor.DarkTextColor;
			this.message.Text = dto.Message;
			this.DateLabel.Text = dto.FullName + " at " + dto.Posted.AddSeconds(NSTimeZone.SystemTimeZone.GetSecondsFromGMT).ToString("MM/dd/yyyy hh:mm tt");

			CGRect titleLabelBounds = new CGRect(CGPoint.Empty, new CGSize(MessageWrapper.Frame.Width - 20, int.MaxValue - 20));
			CGRect minimumTextRect = this.message.TextRectForBounds(titleLabelBounds, 0);
			var titleLabelHeightDelta = minimumTextRect.Size.Height - this.message.Frame.Size.Height;
			this.message.Frame = new CGRect(10, 10, titleLabelBounds.Width, this.message.Frame.Height + titleLabelHeightDelta);

			var wrapperSize = new CGSize(message.Frame.Size.Width + 20, message.Frame.Size.Height + 20);
			var x = isMyMessage ? (this.Frame.Width - 10) - wrapperSize.Width : 10;
			this.MessageWrapper.Frame = new CGRect(new CGPoint(x, 10), wrapperSize);
			this.XConstraint.Constant = x;
		}
	}
}
