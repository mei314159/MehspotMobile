
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;

namespace Mehspot.AndroidApp
{
	[Activity(Label = "Mehspot", Icon = "@mipmap/ic_launcher")]
	public class MainActivity : FragmentActivity
	{
		TabLayout tabLayout;
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.MainActivity);
			tabLayout = FindViewById<TabLayout>(Resource.Id.sliding_tabs);
			FnInitTabLayout();
		}
		void FnInitTabLayout()
		{
			tabLayout.SetTabTextColors(ContextCompat.GetColor(this, Resource.Color.white), ContextCompat.GetColor(this, Resource.Color.dark_orange));
			//Fragment array
			var fragments = new Android.Support.V4.App.Fragment[]
			{
				new MessageBoardFragment(),
				new BadgesFragment(),
				new ProfileFragment()
			};
			//Tab title array
			var titles = CharSequence.ArrayFromStringArray(new[] { "Messages", "Badges", "Profile" });
			var viewPager = FindViewById<ViewPager>(Resource.Id.viewpager);
			//viewpager holding fragment array and tab title text
			viewPager.Adapter = new TabsFragmentPagerAdapter(SupportFragmentManager, fragments, titles);
			// Give the TabLayout the ViewPager 
			tabLayout.SetupWithViewPager(viewPager);
		}

		public void OnClick(IDialogInterface dialog, int which)
		{
			dialog.Dismiss();
		}

	}


}
