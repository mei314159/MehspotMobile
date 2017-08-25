using System;
using Foundation;
using Mehspot.Models.ViewModels;
using SDWebImage;
using UIKit;

namespace Mehspot.iOS.Views.Cell
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

        public static nfloat ExpandedHeight { get; } = 125;
        public static nfloat CollapsedHeight { get; } = 84;

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

        public void Configure (ISearchResultDTO item)
        {
            NSUrl url = null;
            if (!string.IsNullOrEmpty (item.Details.ProfilePicturePath)) {
                url = NSUrl.FromString (item.Details.ProfilePicturePath);
            }

            if (url != null) {
                this.ProfilePicture.SetImage (url);
            } else {
                this.ProfilePicture.Image = UIImage.FromFile ("profile_image");
            }

            this.UserNameLabel.Text = item.Details.FirstName;
            this.DistanceLabel.Text = Math.Round (item.Details.DistanceFrom ?? 0, 2) + " miles";
            this.SubdivisionLabel.Text = !string.IsNullOrWhiteSpace (item.Details.Subdivision) ? $"{item.Details.Subdivision} ({item.Details.ZipCode})" : item.Details.ZipCode;
            this.FavoriteIcon.Hidden = !item.Details.Favourite;
            this.LikesLabel.Text = $"{item.Details.Recommendations} Recommendations";
            this.HourlyRateLabel.Text = item.InfoLabel1;
            this.AgeRangeLabel.Text = item.InfoLabel2;

            this.SendMessageButton.Layer.BorderWidth = this.ViewProfileButton.Layer.BorderWidth = 1;
            this.SendMessageButton.Layer.BorderColor = this.SendMessageButton.TitleColor (UIControlState.Normal).CGColor;
            this.ViewProfileButton.Layer.BorderColor = this.ViewProfileButton.TitleColor (UIControlState.Normal).CGColor;
        }
    }
}
