
using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Mehspot.AndroidApp.Core.Builders;
using Mehspot.AndroidApp.Wrappers;
using Mehspot.Core;
using Mehspot.Core.Contracts.Wrappers;
using Mehspot.Core.DTO;
using Mehspot.Core.Services;

namespace Mehspot.AndroidApp
{
	[Activity(Label = "Search Badge")]
	public class SearchBadgeActivity : Activity, ISearchFilterController
	{
		private SearchBadgeModel<View> model;
		public BadgeSummaryDTO BadgeSummary => Intent.GetExtra<BadgeSummaryDTO>("badgeSummary");
		public string TitleKey { get; set; }

		public IViewHelper ViewHelper { get; private set; }

		public Button SearchButton => this.FindViewById<Button>(Resource.SearchFilter.SearchButton);

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.SearchBadgeActivity);

			this.ViewHelper = new ActivityHelper(this);
			var badgeService = new BadgeService(MehspotAppContext.Instance.DataStorage);
			this.model = new SearchBadgeModel<View>(this, badgeService, new AndroidCellBuilder(this));
			this.Title = this.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.SearchFilter.toolbar).Title = this.model.GetTitle();

			SearchButton.Click += SearchButton_Click;
		}

		protected override async void OnStart()
		{
			base.OnStart();
			await this.model.LoadCellsAsync();
		}

		public void ReloadData()
		{
			var messagesWrapper = this.FindViewById<LinearLayout>(Resource.SearchFilter.FieldsWrapper);
			foreach (var cell in model.Cells)
			{
				messagesWrapper.AddView(cell);
			}
		}

		void SearchButton_Click(object sender, System.EventArgs e)
		{
			var target = new Intent(this, typeof(SearchResultsActivity));
			target.PutExtra("badgeSummary", this.BadgeSummary);
			target.PutExtra("searchQuery", this.model.SearchQuery);
			this.StartActivity(target);
		}
	}
}
