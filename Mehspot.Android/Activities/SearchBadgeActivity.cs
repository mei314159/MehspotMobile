﻿
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Mehspot.AndroidApp.Wrappers;
using Mehspot.Core;
using Mehspot.Core.Contracts.Wrappers;
using Mehspot.Core.DTO;
using Mehspot.Core.Services;
using Mehspot.iOS.Views.Cell;

namespace Mehspot.AndroidApp
{
	[Activity(Label = "Search Badge")]
	public class SearchBadgeActivity : Activity, ISearchFilterController
	{
		private SearchBadgeModel<View> model;
		public BadgeSummaryDTO BadgeSummary => Intent.GetExtra<BadgeSummaryDTO>("badgeSummary");

		public IViewHelper ViewHelper { get; private set; }

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.SearchBadgeActivity);

			this.ViewHelper = new ActivityHelper(this);
			var badgeService = new BadgeService(MehspotAppContext.Instance.DataStorage);
			this.model = new SearchBadgeModel<View>(this, badgeService, new CellFactory(this, badgeService, BadgeSummary.BadgeId));
			this.Title = this.model.GetTitle();
		}

		protected override async void OnStart()
		{
			base.OnStart();
			await this.model.LoadCellsAsync();
		}

		public void ReloadData()
		{
			var messagesWrapper = this.FindViewById<LinearLayout>(Resource.Id.messagesWrapper);
			foreach (var cell in model.Cells)
			{
				messagesWrapper.AddView(cell);
			}
		}
	}
}
