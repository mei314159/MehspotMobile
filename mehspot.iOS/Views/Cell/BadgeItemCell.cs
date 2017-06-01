using System;

using Foundation;
using Mehspot.Core;
using Mehspot.Core.DTO;
using UIKit;

namespace Mehspot.iOS.Views
{
    public partial class BadgeItemCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString ("BadgeItemCell");
        public static readonly UINib Nib;

        static BadgeItemCell ()
        {
            Nib = UINib.FromName ("BadgeItemCell", NSBundle.MainBundle);
        }

        protected BadgeItemCell (IntPtr handle) : base (handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public BadgeSummaryDTO BadgeSummary { get; set; }

        public Action<UIButton> SearchButtonTouch { get; set; }
        public Action<UIButton> BadgeRegisterButtonTouch { get; set; }

        partial void SearchButtonTouched (UIButton sender)
        {
            SearchButtonTouch?.Invoke (sender);
        }

        partial void BadgeRegisterButtonTouched (UIButton sender)
        {
            BadgeRegisterButtonTouch?.Invoke (sender);
        }

        public void Configure (BadgeSummaryDTO badge)
        {
            var cell = this;
            cell.BadgePicture.Image = UIImage.FromFile ("badges/" + badge.BadgeName.ToLower () + (badge.IsRegistered ? string.Empty : "b"));
            cell.BadgeName.Text = MehspotResources.ResourceManager.GetString (badge.BadgeName);
            cell.BadgeSummary = badge;
            cell.SearchButton.Layer.BorderWidth = cell.BadgeRegisterButton.Layer.BorderWidth = 1;
            cell.SearchButton.Layer.BorderColor = cell.SearchButton.TitleColor (UIControlState.Normal).CGColor;
            cell.BadgeRegisterButton.Layer.BorderColor = cell.BadgeRegisterButton.TitleColor (UIControlState.Normal).CGColor;
            cell.BadgeRegisterButton.SetTitle (badge.IsRegistered ? "Update" : "Register", UIControlState.Normal);
            cell.BadgeDescription.Text = MehspotResources.ResourceManager.GetString (badge.BadgeName + "_Description");
            cell.LikesCount.Text = badge.Likes.ToString ();
            cell.RecommendationsCount.Text = badge.Recommendations.ToString ();
            cell.ReferencesCount.Text = badge.References.ToString ();
        }
    }
}
