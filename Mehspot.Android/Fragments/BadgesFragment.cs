using System;
using System.Collections.Generic;
using System.Linq;
using Android.Animation;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using Mehspot.AndroidApp.Resources.layout;
using Mehspot.AndroidApp.Wrappers;
using Mehspot.Core;
using Mehspot.Core.Contracts.ViewControllers;
using Mehspot.Core.Contracts.Wrappers;
using Mehspot.Core.DTO;
using Mehspot.Core.Models;
using Mehspot.Core.Services;

namespace Mehspot.AndroidApp
{
	public class BadgesFragment : Android.Support.V4.App.Fragment, IBadgesViewController
	{
		private TabLayout tabLayout;
		private BadgesModel model;
		
		public IViewHelper ViewHelper { get; private set; }


		public override Android.Views.View OnCreateView(Android.Views.LayoutInflater inflater, Android.Views.ViewGroup container, Bundle savedInstanceState)
		{
			return inflater.Inflate(Resource.Layout.BadgesActivity, container, false);
		}

		public override void OnViewCreated(Android.Views.View view, Bundle savedInstanceState)
		{
			base.OnViewCreated(view, savedInstanceState);
			this.ViewHelper = new ActivityHelper(this.Activity);

			tabLayout = View.FindViewById<TabLayout>(Resource.Id.sliding_tabs);
			model = new BadgesModel(new BadgeService(MehspotAppContext.Instance.DataStorage), this);
			model.LoadingStart += Model_LoadingStart;
			model.LoadingEnd += Model_LoadingEnd;
		}

		public override void OnStart()
		{
			base.OnStart();
            (this.Activity as MainActivity)?.SelectTab(this.GetType());
            model.RefreshAsync(model.Items == null, true);
		}

		void Model_LoadingStart()
		{
		}

		void Model_LoadingEnd()
		{
		}

        public void DisplayBadges()
        {
			tabLayout.SetTabTextColors(ContextCompat.GetColor(this.Activity, Resource.Color.black), ContextCompat.GetColor(this.Activity, Resource.Color.dark_orange));

			var groups = model.BadgeHelper.GetGroups();

			//Fragment array
			var fragments = groups.Select(a => new BadgeGroupFragment(a.Key, a.Value)).ToArray();

			//Tab title array
			var titles = CharSequence.ArrayFromStringArray(groups.Select(a => MehspotResources.ResourceManager.GetString("BadgeGroup_" + a.Key.ToString())).ToArray());
			var viewPager = View.FindViewById<ViewPager>(Resource.Id.viewpager);
			//viewpager holding fragment array and tab title text
			viewPager.Adapter = new TabsFragmentPagerAdapter(Activity.SupportFragmentManager, fragments, titles);
			// Give the TabLayout the ViewPager 
			tabLayout.SetupWithViewPager(viewPager);
        }
    }
}
