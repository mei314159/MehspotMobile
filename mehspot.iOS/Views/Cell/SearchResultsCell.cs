using System;
using System.Collections.Generic;
using Foundation;
using MehSpot.Models.ViewModels;
using SDWebImage;
using UIKit;

namespace mehspot.iOS.Views.Cell
{
    public partial class SearchResultsCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString ("SearchResultsCell");
        public static readonly UINib Nib;

        static SearchResultsCell ()
        {
            Nib = UINib.FromName ("SearchResultsCell", NSBundle.MainBundle);
        }

        public Action<UIButton> SendMessageButtonAction;
        public Action<UIButton> ViewProfileButtonAction;

        protected SearchResultsCell (IntPtr handle) : base (handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        partial void SendMessageButtonTouched (UIButton sender)
        {
            SendMessageButtonAction?.Invoke (sender);
        }

        partial void ViewProfileButtonTouched (UIButton sender)
        {
            ViewProfileButtonAction?.Invoke (sender);
        }

        public void Configure (BabysitterSearchResultDTO item, KeyValuePair<int?, string> [] ageRanges)
        {
            var cell = this;
            if (!string.IsNullOrEmpty (item.Details.ProfilePicturePath)) {

                var url = NSUrl.FromString (item.Details.ProfilePicturePath);
                if (url != null) {
                    cell.ProfilePicture.SetImage (url);
                }
            }

            cell.UserNameLabel.Text = item.Details.FirstName;
            cell.DistanceLabel.Text = Math.Round (item.Details.DistanceFrom ?? 0, 2) + " miles";
            cell.SubdivisionLabel.Text = !string.IsNullOrWhiteSpace (item.Details.Subdivision) ? $"{item.Details.Subdivision} ({item.Details.ZipCode})" : item.Details.ZipCode;
            cell.HourlyRateLabel.Text = $"${item.HourlyRate}/hr";
            cell.AgeRangeLabel.Text = item.AgeRange.HasValue ? ageRanges [item.AgeRange.Value].Value : string.Empty;
            cell.FavoriteIcon.Hidden = !item.Details.Favourite;
            cell.LikesLabel.Text = $"{item.Details.Likes} Likes / {item.Details.Recommendations} Recommendations";

            cell.SendMessageButton.Layer.BorderWidth = cell.ViewProfileButton.Layer.BorderWidth = 1;
            cell.SendMessageButton.Layer.BorderColor = cell.SendMessageButton.TitleColor (UIControlState.Normal).CGColor;
            cell.ViewProfileButton.Layer.BorderColor = cell.ViewProfileButton.TitleColor (UIControlState.Normal).CGColor;
        }
    }
}
