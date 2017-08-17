using System;
using Android.Content;
using Android.Views;
using Android.Widget;
using Mehspot.Core;
using Mehspot.Core.Services.Badges;

namespace Mehspot.AndroidApp.Resources.layout
{

	public class BadgeSummaryItem : RelativeLayout
	{
		readonly BadgeInfo dto;

		public event Action<BadgeInfo, BadgeSummaryItem> Clicked;
		public event Action<BadgeInfo> RegisterButtonClicked;
		public event Action<BadgeInfo> SearchButtonClicked;

		public BadgeSummaryItem(Context context, BadgeInfo dto) : base(context)
		{
			this.dto = dto;
			LayoutInflater inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
			inflater.Inflate(Resource.Layout.BadgeSummaryItem, this);

            BadgeName.Text = MehspotResources.ResourceManager.GetString(dto.CustomKey != null ? "Find_" + dto.CustomKey : "Find_" + dto.SearchBadge);
            BadgeDescription.Text = dto.CustomDescription ?? MehspotResources.ResourceManager.GetString(dto.SearchBadge + "_Description");
            LikesCount.Text = dto.Badge.Likes.ToString();
			RecommendationsCount.Text = dto.Badge.Recommendations.ToString();
			ReferencesCount.Text = dto.Badge.References.ToString();
			RegisterButton.Text = "Update My Info";

			var identifier = Resources.GetIdentifier(dto.BadgeName.ToLower() + (dto.Badge.IsRegistered ? string.Empty : "b"), "drawable", context.PackageName);
			using (var Picture = FindViewById<ImageView>(Resource.BadgeSummary.Picture))
			{
				Picture.SetImageResource(identifier);
			}

			this.Click += Handle_Click;
			this.RegisterButton.Click += (sender, e) => RegisterButtonClicked(dto);
			this.SearchButton.Click += (sender, e) => SearchButtonClicked(dto);
		}

		public TextView BadgeName => (TextView)FindViewById(Resource.BadgeSummary.BadgeName);
		public TextView BadgeDescription => (TextView)FindViewById(Resource.BadgeSummary.BadgeDescription);
		public TextView LikesCount => (TextView)FindViewById(Resource.BadgeSummary.LikesCount);
		public TextView RecommendationsCount => (TextView)FindViewById(Resource.BadgeSummary.RecommendationsCount);
		public TextView ReferencesCount => (TextView)FindViewById(Resource.BadgeSummary.ReferencesCount);
		public Button RegisterButton => (Button)FindViewById(Resource.BadgeSummary.RegisterButton);
		public Button SearchButton => (Button)FindViewById(Resource.BadgeSummary.SearchButton);

		void Handle_Click(object sender, EventArgs e)
		{
			this.Clicked?.Invoke(this.dto, this);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
				FindViewById<ImageView>(Resource.BadgeSummary.Picture).Dispose();

			base.Dispose(disposing);
		}
	}
}
