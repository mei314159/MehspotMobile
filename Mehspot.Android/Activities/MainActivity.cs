using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using BottomNavigationBar;
using BottomNavigationBar.Listeners;
using HockeyApp.Android;

namespace Mehspot.AndroidApp
{
    [Activity(Label = "Mehspot", Icon = "@mipmap/ic_launcher")]
    public class MainActivity : AppCompatActivity, IOnMenuTabClickListener
    {
        internal BottomBar BottomBar;
        private Dictionary<Android.Support.V4.App.Fragment, bool> fragments;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.MainActivity);
            fragments = new Dictionary<Android.Support.V4.App.Fragment, bool>{
                {new BadgesFragment(), false},
                {new MessageBoardFragment(), false},
                {new ProfileFragment(), false}
            };

            BottomBar = BottomBar.Attach(this, savedInstanceState);

            BottomBar.SetActiveTabColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.dark_green)));
            BottomBar.SetItems(Resource.Menu.bottombar_menu);
            BottomBar.SetOnMenuTabClickListener(this);

            // Setting colors for different tabs when there's more than three of them.
            // You can set colors for tabs in three different ways as shown below.
            //_bottomBar.MapColorForTab(0, new Color(ContextCompat.GetColor(this, Resource.Color.dark_green)));
            //_bottomBar.MapColorForTab(1, "#5D4037");
            //_bottomBar.MapColorForTab(2, "#7B1FA2");

        }

        public void OnClick(IDialogInterface dialog, int which)
        {
            dialog.Dismiss();
        }

        protected override void OnResume()
        {
            base.OnResume();
            CrashManager.Register(this, Constants.HockeyAppId);
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);

            // Necessary to restore the BottomBar's state, otherwise we would
            // lose the current tab on orientation change.
            BottomBar.OnSaveInstanceState(outState);
        }



        internal void SelectTab(Type type)
        {
            for (int i = 0; i < fragments.Keys.Count; i++)
            {
                if (fragments.Keys.ElementAt(i).GetType() == type)
                {
                    BottomBar.SelectTabAtPosition(i, true);
                    return;
                }
            }
        }

        public void OnMenuTabSelected(int menuItemId)
        {
            Android.Support.V4.App.Fragment currentFragment;
            switch (menuItemId)
            {
                case Resource.BottomBar.badges:
                    {
                        currentFragment = fragments.Keys.ElementAt(0);
                        break;
                    }
                case Resource.BottomBar.messages:
                    {
                        currentFragment = fragments.Keys.ElementAt(1);
                        break;
                    }
                case Resource.BottomBar.profile:
                    {
                        currentFragment = fragments.Keys.ElementAt(2);
                        break;
                    }
                default:
                    return;
            }

            var fragmentTransaction = SupportFragmentManager.BeginTransaction();
            fragmentTransaction.Replace(Resource.MainActivity.Root, currentFragment);
            fragmentTransaction.SetTransition(Android.Support.V4.App.FragmentTransaction.TransitFragmentFade);
            fragmentTransaction.AddToBackStack(null);
            fragmentTransaction.Commit();

            foreach (var fragment in fragments.Keys.ToArray())
            {
                fragments[fragment] = fragment == currentFragment;
            }
        }

        public void OnMenuTabReSelected(int menuItemId)
        {
            //throw new NotImplementedException();
        }
    }
}
