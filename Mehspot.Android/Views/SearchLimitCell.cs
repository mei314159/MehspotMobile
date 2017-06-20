using System;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Mehspot.Core;

namespace Mehspot.AndroidApp.Views
{

	public class SearchLimitCell : RelativeLayout
	{
		public event Action OnRegisterButtonTouched;

		readonly Activity activity;


		public SearchLimitCell(Activity context, string requiredBadgeName, string searchBadgeName) : base(context)
		{
			this.activity = context;
			LayoutInflater inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
			inflater.Inflate(Resource.Layout.SearchLimitCell, this);


			string badgeNameLocalized = null;
			if (requiredBadgeName != null)
			{
				badgeNameLocalized = MehspotResources.ResourceManager.GetString(requiredBadgeName);
				badgeNameLocalized = badgeNameLocalized ?? requiredBadgeName;
			}
			else if (searchBadgeName != null)
			{
				badgeNameLocalized = MehspotResources.ResourceManager.GetString(searchBadgeName);
				badgeNameLocalized = badgeNameLocalized ?? searchBadgeName;
			}
			string badgeNamePart = (requiredBadgeName == Mehspot.Core.Constants.BadgeNames.Fitness || searchBadgeName == Mehspot.Core.Constants.BadgeNames.Golf || requiredBadgeName == Mehspot.Core.Constants.BadgeNames.OtherJobs ? "for " : "as ") + badgeNameLocalized;
			Message.Text = string.Format(MehspotResources.SearchLimitMessageTemplate, badgeNamePart);


			this.RegisterButton.Click += (sender, e) => OnRegisterButtonTouched?.Invoke();
		}

		public TextView Message => (TextView)FindViewById(Resource.SearchLimitCell.Message);
		public Button RegisterButton => (Button)FindViewById(Resource.SearchLimitCell.RegisterButton);
	}
}
