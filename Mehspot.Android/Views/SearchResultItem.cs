using System;
using System.Net;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using Mehspot.Models.ViewModels;

namespace Mehspot.AndroidApp.Views
{

	public class SearchResultItem : RelativeLayout
	{
		public ISearchResultDTO Dto;

		public event Action<ISearchResultDTO, SearchResultItem> Clicked;
		public event Action<ISearchResultDTO> MessageButtonClicked;
		public event Action<ISearchResultDTO> ViewProfileButtonClicked;

		readonly Activity activity;

		public SearchResultItem(Activity context) : base(context)
		{
			this.activity = context;
			LayoutInflater inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
			inflater.Inflate(Resource.Layout.SearchResultItem, this);

			this.Click += Handle_Click;
			this.MessageButton.Click += (sender, e) => MessageButtonClicked?.Invoke(Dto);
			this.ProfileButton.Click += (sender, e) => ViewProfileButtonClicked?.Invoke(Dto);
		}

		public TextView UserNameLabel => (TextView)FindViewById(Resource.SearchResultItem.UserNameLabel);
		public TextView LikesCount => (TextView)FindViewById(Resource.SearchResultItem.LikesCount);
		public TextView RecommendationsCount => (TextView)FindViewById(Resource.SearchResultItem.RecommendationsCount);
		public TextView DistanceLabel => (TextView)FindViewById(Resource.SearchResultItem.DistanceLabel);
		public TextView InfoLabel1 => (TextView)FindViewById(Resource.SearchResultItem.InfoLabel1);
		public TextView InfoLabel2 => (TextView)FindViewById(Resource.SearchResultItem.InfoLabel2);
		public TextView SubdivisionLabel => (TextView)FindViewById(Resource.SearchResultItem.SubdivisionLabel);

		public ImageView Picture => (ImageView)FindViewById(Resource.SearchResultItem.Picture);
		public ImageView FavoriteIcon => (ImageView)FindViewById(Resource.SearchResultItem.FavoriteIcon);
		public Button MessageButton => (Button)FindViewById(Resource.SearchResultItem.SendMessageButton);
		public Button ProfileButton => (Button)FindViewById(Resource.SearchResultItem.ViewProfileButton);

		public void Init(ISearchResultDTO item)
		{
			this.Dto = item;

			Picture.ClipToOutline = true;
			this.UserNameLabel.Text = item.Details.FirstName;
			this.DistanceLabel.Text = Math.Round(item.Details.DistanceFrom ?? 0, 2) + " miles";
			this.SubdivisionLabel.Text = !string.IsNullOrWhiteSpace(item.Details.Subdivision) ? $"{item.Details.Subdivision} ({item.Details.ZipCode})" : item.Details.ZipCode;
			this.FavoriteIcon.Visibility = item.Details.Favourite ? ViewStates.Visible : ViewStates.Gone;
			this.LikesCount.Text = item.Details.Likes.ToString();
			this.RecommendationsCount.Text = item.Details.Recommendations.ToString();
			this.InfoLabel1.Text = item.InfoLabel1;
			this.InfoLabel2.Text = item.InfoLabel2;

			Task.Run(() =>
			{
				if (!string.IsNullOrWhiteSpace(item.Details.ProfilePicturePath))
				{
					var imageBitmap = GetImageBitmapFromUrl(item.Details.ProfilePicturePath);
					activity.RunOnUiThread(() => Picture.SetImageBitmap(imageBitmap));
				}
				else {
					var identifier = Resources.GetIdentifier("profile_image", "drawable", this.activity.PackageName);
					activity.RunOnUiThread(() => Picture.SetImageResource(identifier));
				}
			});
		}

		void Handle_Click(object sender, EventArgs e)
		{
			this.Clicked?.Invoke(this.Dto, this);
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
