using System;
using Foundation;
using Mehspot.Core;
using Mehspot.Core.DTO;
using ObjCRuntime;
using UIKit;

namespace Mehspot.iOS.Views
{
	[Register("BadgeItemCellSimplified")]
	public partial class BadgeItemCellSimplified : UITableViewCell
	{
		public static readonly NSString Key = new NSString("BadgeItemCellSimplified");
		public static readonly UINib Nib;

		static BadgeItemCellSimplified()
		{
			Nib = UINib.FromName("BadgeItemCellSimplified", NSBundle.MainBundle);
		}

		protected BadgeItemCellSimplified(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public BadgeSummaryBaseDTO BadgeSummary { get; set; }

		[Outlet]
		public UILabel BadgeName { get; set; }

		[Outlet]
		public UIImageView BadgePicture { get; set; }

		public static BadgeItemCellSimplified Create()
		{
			var arr = NSBundle.MainBundle.LoadNib("BadgeItemCellSimplified", null, null);
			var v = Runtime.GetNSObject<BadgeItemCellSimplified>(arr.ValueAt(0));
			return null;
		}

		public void Configure(BadgeSummaryBaseDTO badge)
		{
			var cell = this;
			cell.BadgePicture.Image = UIImage.FromFile("badges/" + badge.BadgeName.ToLower());
			cell.BadgeName.Text = MehspotResources.ResourceManager.GetString(badge.BadgeName);
			cell.BadgeSummary = badge;
		}

		void ReleaseDesignerOutlets()
		{
			if (BadgeName != null)
			{
				BadgeName.Dispose();
				BadgeName = null;
			}

			if (BadgePicture != null)
			{
				BadgePicture.Dispose();
				BadgePicture = null;
			}
		}
	}
}
