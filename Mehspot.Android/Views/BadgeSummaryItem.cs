using System;
using System.Net;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using Mehspot.Core;
using Mehspot.Core.DTO;

namespace Mehspot.AndroidApp.Resources.layout
{

	public class BadgeSummaryItem : RelativeLayout
	{
		readonly BadgeSummaryDTO dto;

		public event Action<BadgeSummaryDTO, BadgeSummaryItem> Clicked;

		public BadgeSummaryItem(Context context, BadgeSummaryDTO dto) : base(context)
		{
			this.dto = dto;
			LayoutInflater inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
			inflater.Inflate(Resource.Layout.BadgeSummaryItem, this);

			BadgeName.Text = MehspotResources.ResourceManager.GetString(dto.BadgeName);
			BadgeDescription.Text = MehspotResources.ResourceManager.GetString(dto.BadgeName + "_Description");
			LikesCount.Text = dto.Likes.ToString();
			RecommendationsCount.Text = dto.Recommendations.ToString();
			ReferencesCount.Text = dto.References.ToString();
			RegisterButton.Text = dto.IsRegistered ? "Update" : "Register";
            
			var identifier = Resources.GetIdentifier(dto.BadgeName.ToLower() + (dto.IsRegistered ? string.Empty : "b"), "drawable", context.PackageName);
			Picture.SetImageResource(identifier);
			this.Click += Handle_Click;
		}

		public TextView BadgeName => (TextView)FindViewById(Resource.BadgeSummary.BadgeName);
		public TextView BadgeDescription => (TextView)FindViewById(Resource.BadgeSummary.BadgeDescription);
		public TextView LikesCount => (TextView)FindViewById(Resource.BadgeSummary.LikesCount);
		public TextView RecommendationsCount => (TextView)FindViewById(Resource.BadgeSummary.RecommendationsCount);
		public TextView ReferencesCount => (TextView)FindViewById(Resource.BadgeSummary.ReferencesCount);
		public ImageView Picture => (ImageView)FindViewById(Resource.BadgeSummary.Picture);
		public Button RegisterButton => (Button)FindViewById(Resource.BadgeSummary.RegisterButton);

		void Handle_Click(object sender, EventArgs e)
		{
			this.Clicked?.Invoke(this.dto, this);
		}

		private Bitmap GetImageBitmapFromUrl(string url)
		{
			Bitmap imageBitmap = null;

			using (var webClient = new WebClient())
			{
				var imageBytes = webClient.DownloadData(url);
				if (imageBytes != null && imageBytes.Length > 0)
				{
					imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
				}
			}

			return imageBitmap;
		}
	}
}
