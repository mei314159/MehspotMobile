using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Mehspot.Core.DTO.Badges;

namespace Mehspot.AndroidApp
{

	public class RecommendationCell : RelativeLayout
	{
		public RecommendationCell(Activity activity, BadgeUserRecommendationDTO item) : base(activity)
		{
			LayoutInflater inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
			inflater.Inflate(Resource.Layout.RecommendationCell, this);

			this.FindViewById<TextView>(Resource.RecommendationCell.UserName).Text = item.FromUserName;
			this.FindViewById<TextView>(Resource.RecommendationCell.Date).Text = item.Date.ToString("d");
			this.FindViewById<TextView>(Resource.RecommendationCell.Message).Text = item.Comment;
			Picture.ClipToOutline = true;
			Task.Run(() =>
			{
				if (!string.IsNullOrWhiteSpace(item.FromUserProfilePicturePath))
				{
					var imageBitmap = activity.GetImageBitmapFromUrl(item.FromUserProfilePicturePath);
					activity.RunOnUiThread(() => Picture.SetImageBitmap(imageBitmap));
				}
			});
		}

		public ImageView Picture => this.FindViewById<ImageView>(Resource.RecommendationCell.Picture);
	}
}
