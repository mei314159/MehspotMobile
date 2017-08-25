using System;
using CoreGraphics;
using Foundation;
using Mehspot.Core.DTO.Badges;
using SDWebImage;
using UIKit;

namespace Mehspot.iOS.Views.Cell
{
    public partial class RecommendationCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString ("RecommendationCell");
        public static readonly UINib Nib;

        public BadgeUserRecommendationDTO Dto;


        static RecommendationCell ()
        {
            Nib = UINib.FromName ("RecommendationCell", NSBundle.MainBundle);
        }

        protected RecommendationCell (IntPtr handle) : base (handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        private void Configure (BadgeUserRecommendationDTO item)
        {
            this.Dto = item;
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

        internal static RecommendationCell Create (BadgeUserRecommendationDTO item)
        {
            var cell = (RecommendationCell)Nib.Instantiate(null, null)[0];
            cell.Configure(item);
            cell.UpdateSize();
            return cell;
        }

        private void UpdateSize()
        {
            var initilaCellBound = this.Message.Frame.Location.Y + this.Message.Frame.Height;
            var textSize = this.Message.SizeThatFits(new CGSize(this.Message.Frame.Width, nfloat.MaxValue));
            var height = textSize.Height > this.Message.Frame.Height ? textSize.Height : this.Message.Frame.Height;
            this.RecommendationHeight.Constant = height;
        }
    }
}
