using System;
using System.Net;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using Mehspot.Core;
using Mehspot.Core.DTO;
using Mehspot.Models.ViewModels;

namespace Mehspot.AndroidApp.Resources.layout
{

	public class SearchResultItem : RelativeLayout
	{
		readonly ISearchResultDTO dto;

		public event Action<ISearchResultDTO, SearchResultItem> Clicked;
		public event Action<ISearchResultDTO> MessageButtonClicked;
		public event Action<ISearchResultDTO> ViewProfileButtonClicked;

		public SearchResultItem(Context context, ISearchResultDTO dto) : base(context)
		{
			this.dto = dto;
			LayoutInflater inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
			inflater.Inflate(Resource.Layout.SearchResultItem, this);

			BadgeName.Text = MehspotResources.ResourceManager.GetString(dto.BadgeName);
			BadgeDescription.Text = MehspotResources.ResourceManager.GetString(dto.BadgeName + "_Description");

			var identifier = Resources.GetIdentifier(dto.BadgeName.ToLower() + (dto.IsRegistered ? string.Empty : "b"), "drawable", context.PackageName);
			Picture.SetImageResource(identifier);
			this.Click += Handle_Click;
			this.RegisterButton.Click += (sender, e) => RegisterButtonClicked(dto);
			this.SearchButton.Click += (sender, e) => SearchButtonClicked(dto);
		}

		public TextView BadgeName => (TextView)FindViewById(Resource.BadgeSummary.BadgeName);
		public TextView BadgeDescription => (TextView)FindViewById(Resource.BadgeSummary.BadgeDescription);
		public ImageView Picture => (ImageView)FindViewById(Resource.BadgeSummary.Picture);
		public Button RegisterButton => (Button)FindViewById(Resource.BadgeSummary.RegisterButton);
		public Button SearchButton => (Button)FindViewById(Resource.BadgeSummary.SearchButton);

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
