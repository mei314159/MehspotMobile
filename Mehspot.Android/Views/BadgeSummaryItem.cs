using System;
using Android.Content;
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
		public event Action<BadgeSummaryDTO> RegisterButtonClicked;
		public event Action<BadgeSummaryDTO> SearchButtonClicked;

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
			this.RegisterButton.Click += (sender, e) => RegisterButtonClicked(dto);
			this.SearchButton.Click += (sender, e) => SearchButtonClicked(dto);
		}

		public TextView BadgeName => (TextView)FindViewById(Resource.BadgeSummary.BadgeName);
		public TextView BadgeDescription => (TextView)FindViewById(Resource.BadgeSummary.BadgeDescription);
		public TextView LikesCount => (TextView)FindViewById(Resource.BadgeSummary.LikesCount);
		public TextView RecommendationsCount => (TextView)FindViewById(Resource.BadgeSummary.RecommendationsCount);
		public TextView ReferencesCount => (TextView)FindViewById(Resource.BadgeSummary.ReferencesCount);
		public ImageView Picture => (ImageView)FindViewById(Resource.BadgeSummary.Picture);
		public Button RegisterButton => (Button)FindViewById(Resource.BadgeSummary.RegisterButton);
		public Button SearchButton => (Button)FindViewById(Resource.BadgeSummary.SearchButton);

		void Handle_Click(object sender, EventArgs e)
		{
			this.Clicked?.Invoke(this.dto, this);
		}
	}
}
