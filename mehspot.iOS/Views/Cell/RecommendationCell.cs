using System;

using Foundation;
using Mehspot.Core.DTO.Badges;
using SDWebImage;
using UIKit;

namespace mehspot.iOS.Views.Cell
{
    public partial class RecommendationCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString ("RecommendationCell");
        public static readonly UINib Nib;

        static RecommendationCell ()
        {
            Nib = UINib.FromName ("RecommendationCell", NSBundle.MainBundle);
        }

        protected RecommendationCell (IntPtr handle) : base (handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        internal void Configure (BadgeUserRecommendationDTO item)
        {
            UserName.Text = item.FromUserName;
            DateField.Text = item.Date.ToString ("d");
            Message.Text = item.Comment;
            NSUrl url = null;
            if (!string.IsNullOrEmpty (item.FromUserProfilePicturePath)) {
                url = NSUrl.FromString (item.FromUserProfilePicturePath);
            }

            if (url != null) {

                this.ProfilePicture.SetImage (url);
            } else {

                this.ProfilePicture.Image = UIImage.FromFile ("profile_image");
            }
        }
    }
}
