using System;

using Foundation;
using Mehspot.Core;
using Mehspot.Core.DTO;
using Mehspot.Core.Services.Badges;
using ObjCRuntime;
using UIKit;

namespace Mehspot.iOS.Views
{
	[Register("BadgeItemCell")]
	public partial class BadgeItemCell : UITableViewCell
	{
		public static readonly NSString Key = new NSString("BadgeItemCell");
		public static readonly UINib Nib;

		static BadgeItemCell()
		{
			Nib = UINib.FromName("BadgeItemCell", NSBundle.MainBundle);
		}

		protected BadgeItemCell(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public BadgeInfo BadgeInfo { get; set; }


		[Outlet]
		UIKit.UILabel BadgeDescription { get; set; }

		[Outlet]
		public UILabel BadgeName { get; set; }

		[Outlet]
		public UIImageView BadgePicture { get; set; }

		[Outlet]
		UIKit.UIButton BadgeRegisterButton { get; set; }

		[Outlet]
		UIKit.UILabel LikesCount { get; set; }

		[Outlet]
		UIKit.UILabel RecommendationsCount { get; set; }

		[Outlet]
		UIKit.UILabel ReferencesCount { get; set; }

		[Outlet]
		UIKit.UIButton SearchButton { get; set; }

		public Action<UIButton> SearchButtonTouch { get; set; }
		public Action<UIButton> BadgeRegisterButtonTouch { get; set; }

		public static BadgeItemCell Create()
		{
			var arr = NSBundle.MainBundle.LoadNib("BadgeItemCell", null, null);
			var v = Runtime.GetNSObject<BadgeItemCell>(arr.ValueAt(0));
			return v;
		}


		[Action("SearchButtonTouched:")]
		void SearchButtonTouched(UIButton sender)
		{
			SearchButtonTouch?.Invoke(sender);
		}

		[Action("BadgeRegisterButtonTouched:")]
		void BadgeRegisterButtonTouched(UIButton sender)
		{
			BadgeRegisterButtonTouch?.Invoke(sender);
		}

		public void Initialize(BadgeInfo badge)
		{
			var cell = this;
			cell.BadgePicture.Image = UIImage.FromFile("badges/" + badge.BadgeName.ToLower() + (badge.Badge.IsRegistered ? string.Empty : "b"));
			cell.BadgeName.Text = badge.CustomLabel ?? MehspotResources.ResourceManager.GetString("Find_" + badge.SearchBadge);
			cell.BadgeInfo = badge;
			cell.SearchButton.Layer.BorderWidth = cell.BadgeRegisterButton.Layer.BorderWidth = 1;
			cell.SearchButton.Layer.BorderColor = cell.SearchButton.TitleColor(UIControlState.Normal).CGColor;
			cell.BadgeRegisterButton.Layer.BorderColor = cell.BadgeRegisterButton.TitleColor(UIControlState.Normal).CGColor;
			cell.BadgeRegisterButton.SetTitle(badge.Badge.IsRegistered ? "Update" : "Register", UIControlState.Normal);
			cell.BadgeDescription.Text = badge.CustomDescription ?? MehspotResources.ResourceManager.GetString(badge.SearchBadge + "_Description");
			cell.LikesCount.Text = badge.Badge.Likes.ToString();
			cell.RecommendationsCount.Text = badge.Badge.Recommendations.ToString();
			cell.ReferencesCount.Text = badge.Badge.References.ToString();
		}
		void ReleaseDesignerOutlets()
		{
			if (BadgeDescription != null)
			{
				BadgeDescription.Dispose();
				BadgeDescription = null;
			}

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

			if (BadgeRegisterButton != null)
			{
				BadgeRegisterButton.Dispose();
				BadgeRegisterButton = null;
			}

			if (LikesCount != null)
			{
				LikesCount.Dispose();
				LikesCount = null;
			}

			if (RecommendationsCount != null)
			{
				RecommendationsCount.Dispose();
				RecommendationsCount = null;
			}

			if (ReferencesCount != null)
			{
				ReferencesCount.Dispose();
				ReferencesCount = null;
			}

			if (SearchButton != null)
			{
				SearchButton.Dispose();
				SearchButton = null;
			}
		}
	}
}
