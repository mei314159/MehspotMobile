﻿using System;

using Foundation;
using Mehspot.Core;
using Mehspot.Core.DTO;
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

		public BadgeSummaryDTO BadgeSummary { get; set; }


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

		[Action("BadgeRegisterButtonTouched:")]
		partial void BadgeRegisterButtonTouched(UIKit.UIButton sender);

		[Action("SearchButtonTouched:")]
		partial void SearchButtonTouched(UIKit.UIButton sender);

		public Action<UIButton> SearchButtonTouch { get; set; }
		public Action<UIButton> BadgeRegisterButtonTouch { get; set; }

		partial void SearchButtonTouched(UIButton sender)
		{
			SearchButtonTouch?.Invoke(sender);
		}

		partial void BadgeRegisterButtonTouched(UIButton sender)
		{
			BadgeRegisterButtonTouch?.Invoke(sender);
		}

		public void Configure(BadgeSummaryDTO badge)
		{
			var cell = this;
			cell.BadgePicture.Image = UIImage.FromFile("badges/" + badge.BadgeName.ToLower() + (badge.IsRegistered ? string.Empty : "b"));
			cell.BadgeName.Text = MehspotResources.ResourceManager.GetString(badge.BadgeName);
			cell.BadgeSummary = badge;
			cell.SearchButton.Layer.BorderWidth = cell.BadgeRegisterButton.Layer.BorderWidth = 1;
			cell.SearchButton.Layer.BorderColor = cell.SearchButton.TitleColor(UIControlState.Normal).CGColor;
			cell.BadgeRegisterButton.Layer.BorderColor = cell.BadgeRegisterButton.TitleColor(UIControlState.Normal).CGColor;
			cell.BadgeRegisterButton.SetTitle(badge.IsRegistered ? "Update" : "Register", UIControlState.Normal);
			cell.BadgeDescription.Text = MehspotResources.ResourceManager.GetString(badge.BadgeName + "_Description");
			cell.LikesCount.Text = badge.Likes.ToString();
			cell.RecommendationsCount.Text = badge.Recommendations.ToString();
			cell.ReferencesCount.Text = badge.References.ToString();
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
