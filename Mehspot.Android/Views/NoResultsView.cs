using System;
using Android.Content;
using Android.Views;
using Android.Widget;
using Mehspot.AndroidApp.Views;

namespace Mehspot.AndroidApp.Resources.layout
{

	public class NoResultsView : RelativeLayout
	{
		public NoResultsView(Context context) : base(context)
		{
			LayoutInflater inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
			inflater.Inflate(Resource.Layout.NoResultsView, this);
		}

		internal void HideRegisterButton()
		{
			FindViewById<SearchLimitCell>(Resource.NoResults.Register).Visibility = ViewStates.Gone;
		}

		internal void ShowRegisterButton(string requiredBadgeName, string badgeName)
		{
			var searchLimitCell = FindViewById<SearchLimitCell>(Resource.NoResults.Register);
			searchLimitCell.SetTargetBadge(requiredBadgeName, badgeName);
			searchLimitCell.Visibility = ViewStates.Visible;
		}

	}
}
