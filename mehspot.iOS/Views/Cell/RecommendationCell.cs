using System;
using CoreGraphics;
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
            var cell = (RecommendationCell)Nib.Instantiate (null, null) [0];
            cell.Configure (item);

            var initilaCellBound = cell.Message.Frame.Location.Y + cell.Message.Frame.Height;
            var textSize = cell.Message.SizeThatFits (new CGSize (cell.Message.Frame.Width, nfloat.MaxValue));
            var height = textSize.Height > cell.Message.Frame.Height ? textSize.Height : cell.Message.Frame.Height;
            cell.Message.Frame = new CGRect (cell.Message.Frame.Location, new CGSize (textSize.Width, height));
            var cellBound = cell.Message.Frame.Location.Y + cell.Message.Frame.Height;
            cell.Frame = new CGRect (cell.Frame.Location, new CGSize (cell.Frame.Width, cellBound > initilaCellBound ? cell.Frame.Height + cellBound - initilaCellBound : cell.Frame.Height));

            return cell;

        }
    }
}
