
using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using Mehspot.AndroidApp.Adapters;
using Mehspot.AndroidApp.Core.Builders;
using Mehspot.AndroidApp.Wrappers;
using Mehspot.Core;
using Mehspot.Core.Contracts.ViewControllers;
using Mehspot.Core.Contracts.Wrappers;
using Mehspot.Core.DTO;
using Mehspot.Core.Services;

namespace Mehspot.AndroidApp
{
	[Activity(Label = "Edit Badge")]
	public class EditBadgeProfileActivity : Activity, IEditBadgeProfileController
	{
		private EditBadgeProfileModel<View> model;
		public BadgeSummaryDTO BadgeSummary => Intent.GetExtra<BadgeSummaryDTO>("badgeSummary");

		public IViewHelper ViewHelper { get; private set; }

		public Button SearchButton => this.FindViewById<Button>(Resource.SearchFilter.SearchButton);

		public string BadgeName => Intent.GetStringExtra("badgeName");

		public int BadgeId => Intent.GetIntExtra("badgeId", 0);

		public bool BadgeIsRegistered { get; set; }

		public bool RedirectToSearchResults => Intent.GetBooleanExtra("redirectToSearchResults", false);

		public bool SaveButtonEnabled
		{
			get
			{
				return SaveButton.Enabled;
			}
			set
			{
				SaveButton.Enabled = value;
			}
		}

		public string WindowTitle
		{
			get
			{
				return Toolbar.Title;
			}
			set
			{
				Toolbar.Title = value;
			}
		}

		public LinearLayout ContentWrapper => FindViewById<LinearLayout>(Resource.EditBadgeProfileActivity.ContentWrapper);
		public Button SaveButton => (Button)FindViewById(Resource.EditBadgeProfileActivity.SaveButton);

		public Android.Support.V7.Widget.Toolbar Toolbar => this.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.EditBadgeProfileActivity.toolbar);

		public SwipeRefreshLayout Refresher => this.FindViewById<SwipeRefreshLayout>(Resource.EditBadgeProfileActivity.Refresher);

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.EditBadgeProfileActivity);
			this.BadgeIsRegistered = Intent.GetBooleanExtra("badgeIsRegistered", false);

			this.ViewHelper = new ActivityHelper(this);
			var badgeService = new BadgeService(MehspotAppContext.Instance.DataStorage);
			var subdivisionService = new SubdivisionService(MehspotAppContext.Instance.DataStorage);
			this.model = new EditBadgeProfileModel<View>(this, badgeService, subdivisionService, new AndroidCellBuilder(this));
			model.LoadingStarted += Model_LoadingStarted;
			model.LoadingEnded += Model_LoadingEnded;

			ListView.Adapter = new ViewListAdapter(this, model);
			SaveButton.Click += SaveButton_Click;
			Refresher.SetColorSchemeColors(Resource.Color.xam_dark_blue,
											Resource.Color.xam_purple,
											Resource.Color.xam_gray,
										   	Resource.Color.xam_green);
			Refresher.Refresh += async (sender, e) => await model.ReloadAsync();
		}

		protected override void OnStart()
		{
			base.OnStart();
			this.model.ReloadAsync();
		}

		public void ReloadData()
		{
			ContentWrapper.RemoveAllViews();
			foreach (var item in model.Cells)
			{
				ContentWrapper.AddView(item);
			}
		}

		public void Dismiss()
		{
			this.Finish();
		}

		public void HideKeyboard()
		{

		}

		public void GoToSearchResults()
		{
			this.Dismiss();
		}

		private void Model_LoadingStarted()
		{
			this.SaveButtonEnabled = false;
			ViewHelper.ShowOverlay("Loading");
		}

		private void Model_LoadingEnded()
		{
			ViewHelper.HideOverlay();
			this.SaveButtonEnabled = true;
		}

		async void SaveButton_Click(object sender, EventArgs e)
		{
			await model.SaveAsync();
		}
	}
}
