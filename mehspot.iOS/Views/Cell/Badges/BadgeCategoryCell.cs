using System;

using Foundation;
using Mehspot.Core;
using Mehspot.Core.DTO;
using ObjCRuntime;
using UIKit;

namespace mehspot.iOS.Views.Cell
{
	public partial class BadgeCategoryCell : UITableViewCell
	{
		private static UIColor SelectedColor = UIColor.FromRGB(235, 246, 240);
		public static readonly UINib Nib;
		public static readonly NSString Key = new NSString("BadgeCategoryCell");

		static BadgeCategoryCell()
		{
			Nib = UINib.FromName("BadgeCategoryCell", NSBundle.MainBundle);
		}

		protected BadgeCategoryCell(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public BadgeGroup BadgeGroup { get; internal set; }

		public static BadgeCategoryCell Create(UIColor iconColor, BadgeGroup badgeGroup)
		{
			var arr = NSBundle.MainBundle.LoadNib("BadgeCategoryCell", null, null);
			var v = Runtime.GetNSObject<BadgeCategoryCell>(arr.ValueAt(0));

			v.BadgeGroup = badgeGroup;
			v.BadgeLabel.TextColor = iconColor;
			v.BadgeCircle.Layer.BorderColor = iconColor.CGColor;
			v.BadgeCircle.Layer.BorderWidth = 4;
			v.BadgeCircle.Layer.CornerRadius = v.BadgeCircle.Frame.Width / 2;

			v.BadgeLabel.Text = MehspotResources.ResourceManager.GetString("BadgeGroup_" + badgeGroup.ToString());
			v.BadgeDescription.Text = MehspotResources.ResourceManager.GetString("BadgeGroupDescription_" + badgeGroup.ToString());
			v.SelectedBackgroundView = new UIView { BackgroundColor = SelectedColor };
			return v;
		}
	}
}
