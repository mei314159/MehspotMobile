
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Mehspot.Core.DTO;

namespace Mehspot.AndroidApp
{
	[Activity(Label = "Search Badge")]
	public class SearchBadgeActivity : Activity
	{
		//private SearchModel model;

		public BadgeSummaryDTO BadgeSummary => Intent.GetExtra<BadgeSummaryDTO>("badgeSummary");

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.SearchBadgeActivity);
			//this.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.SearchFilter.toolbar).Title =
			//	    this.model.GetSearchFilterTitle();
		}
	}
}
