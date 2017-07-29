using System;

using Foundation;
using Mehspot.Core;
using Mehspot.Core.DTO;
using ObjCRuntime;
using UIKit;

namespace mehspot.iOS.Views
{
	public partial class GrouppingViewCell : UICollectionViewCell, IGrouppingViewCell<BadgeGroup>
	{
		public static readonly NSString Key = new NSString("GrouppingViewCell");
		public static readonly UINib Nib;

		static GrouppingViewCell()
		{
			Nib = UINib.FromName("GrouppingViewCell", NSBundle.MainBundle);
		}

		protected GrouppingViewCell(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public BadgeGroup GroupKey { get; private set; }

		public static GrouppingViewCell Create()
		{
			var arr = NSBundle.MainBundle.LoadNib("GrouppingViewCell", null, null);
			var v = Runtime.GetNSObject<GrouppingViewCell>(arr.ValueAt(0));

			return v;
		}

		public void Initialize(BadgeGroup group)
		{
			this.GroupKey = group;
			string text = MehspotResources.ResourceManager.GetString("BadgeGroup_" + group.ToString());
			UIColor color;
			switch (group)
			{
				case BadgeGroup.Friends:
					color = UIColor.FromRGB(251, 114, 0);
					break;
				case BadgeGroup.Jobs:
					color = UIColor.FromRGB(3, 155, 67);
					break;
				case BadgeGroup.Helpers:
					color = UIColor.FromRGB(13, 109, 182);
					break;
				default:
					color = UIColor.Black;
					break;
			}


			this.Label.Text = text;
			this.Label.TextColor = color;
			this.Layer.BorderWidth = 3;
			this.Layer.BorderColor = color.CGColor;
		}
	}
}
