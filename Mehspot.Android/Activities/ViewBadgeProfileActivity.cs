using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Mehspot.AndroidApp.Views;
using Mehspot.AndroidApp.Wrappers;
using Mehspot.Core;
using Mehspot.Core.Contracts.Wrappers;
using Mehspot.Core.DTO;
using Mehspot.Core.DTO.Badges;
using Mehspot.Core.Models;
using Mehspot.Core.Services;
using Mehspot.Models.ViewModels;
using Mehspot.AndroidApp.Adapters;
using Mehspot.AndroidApp.Activities;
using Mehspot.AndroidApp.Core.Builders;

namespace Mehspot.AndroidApp
{
	[Activity(Label = "View Badge Profile")]
	public class ViewBadgeProfileActivity : Activity, IViewBadgeProfileController
	{
		private ViewBadgeProfileModel<View> model;
		public int BadgeId => Intent.GetIntExtra("badgeId", 0);

		public string BadgeName => Intent.GetStringExtra("badgeName");

		public string UserId => Intent.GetStringExtra("userId");

		public IViewHelper ViewHelper { get; private set; }

		public TextView UserNameLabel => (TextView)FindViewById(Resource.ViewBadgeProfileActivity.UserNameLabel);
		public TextView LikesCount => (TextView)FindViewById(Resource.ViewBadgeProfileActivity.LikesCount);
		public TextView DistanceLabel => (TextView)FindViewById(Resource.ViewBadgeProfileActivity.DistanceLabel);
		public TextView InfoLabel1View => (TextView)FindViewById(Resource.ViewBadgeProfileActivity.InfoLabel1);
		public TextView InfoLabel2View => (TextView)FindViewById(Resource.ViewBadgeProfileActivity.InfoLabel2);
		public TextView SubdivisionLabel => (TextView)FindViewById(Resource.ViewBadgeProfileActivity.SubdivisionLabel);
		public ListView ListView => (ListView)FindViewById(Resource.ViewBadgeProfileActivity.ListView);

		public ImageView Picture => (ImageView)FindViewById(Resource.ViewBadgeProfileActivity.Picture);
		public ImageView FavoriteIcon => (ImageView)FindViewById(Resource.ViewBadgeProfileActivity.FavoriteIcon);
		public SegmentedControl Segments => (SegmentedControl)FindViewById(Resource.ViewBadgeProfileActivity.Segments);

		#region IViewBadgeProfileController

		public string WindowTitle
		{
			get
			{
				return this.Title;
			}

			set
			{
				this.Title = value;
			}
		}

		public string Subdivision
		{
			get
			{
				return this.SubdivisionLabel.Text;
			}

			set
			{
				this.SubdivisionLabel.Text = value;
			}
		}

		public string Distance
		{
			get
			{
				return this.DistanceLabel.Text;
			}

			set
			{
				this.DistanceLabel.Text = value;
			}
		}

		public string Likes
		{
			get
			{
				return this.LikesCount.Text;
			}

			set
			{
				this.LikesCount.Text = value;
			}
		}

		public string FirstName
		{
			get
			{
				return this.UserNameLabel.Text;
			}

			set
			{
				this.UserNameLabel.Text = value;
			}
		}

		public string InfoLabel1
		{
			get
			{
				return this.InfoLabel1View.Text;
			}

			set
			{
				this.InfoLabel1View.Text = value;
			}
		}

		public string InfoLabel2
		{
			get
			{
				return this.InfoLabel2View.Text;
			}

			set
			{
				this.InfoLabel2View.Text = value;
			}
		}

		public bool HideFavoriteIcon
		{
			get
			{
				return FavoriteIcon.Visibility == ViewStates.Gone;
			}

			set
			{
				FavoriteIcon.Visibility = value ? ViewStates.Gone : ViewStates.Visible;
			}
		}

		public bool EnableSendMessageButton
		{
			get
			{
				return Segments.MessageButton.Enabled;
			}

			set
			{
				Segments.MessageButton.Enabled = value;
			}
		}
		#endregion

		protected override async void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.ViewBadgeProfileActivity);

			this.ViewHelper = new ActivityHelper(this);
			model = new ViewBadgeProfileModel<View>(this, new BadgeService(MehspotAppContext.Instance.DataStorage), new AndroidCellBuilder(this));
			model.OnRefreshing += Model_OnRefreshing;
			model.OnRefreshed += Model_OnRefreshed;
			model.OnWriteReviewButtonTouched += RecommendationsDataSource_OnWriteReviewButtonTouched;
			model.OnGoToMessaging += GoToMessaging;
			Segments.DetailsButton.Click += DetailsButton_Click;
			Segments.RecommendationsButton.Click += RecommendationsButton_Click;
			Segments.MessageButton.Click += MessageButton_Click;
			ListView.Adapter = new ViewListAdapter(this, model);
		}

		void DetailsButton_Click(object sender, EventArgs e)
		{
			model.LoadProfile();
		}

		async void RecommendationsButton_Click(object sender, EventArgs e)
		{
			await model.LoadRecommendations();
		}

		void MessageButton_Click(object sender, EventArgs e)
		{
			var messagingActivity = new Intent(this, typeof(MessagingActivity));
			messagingActivity.PutExtra("toUserId", this.SearchResultDTO.Details.UserId);
			messagingActivity.PutExtra("toUserName", this.SearchResultDTO.Details.FirstName);
			this.StartActivity(messagingActivity);
		}

		protected override void OnStart()
		{
			base.OnStart();
			model.RefreshView();
		}

		public void SetProfilePictureUrl(string value)
		{
			Picture.ClipToOutline = true;
			Task.Run(() =>
			{
				if (!string.IsNullOrWhiteSpace(value))
				{
					var imageBitmap = this.GetImageBitmapFromUrl(value);
					this.RunOnUiThread(() => Picture.SetImageBitmap(imageBitmap));
				}
				else
				{
					var identifier = Resources.GetIdentifier("profile_image", "drawable", this.PackageName);
					this.RunOnUiThread(() => Picture.SetImageResource(identifier));
				}
			});
		}

		public void ReloadCells()
		{
			Segments.HighlightSelectedButton(Segments.DetailsButton);
			((ViewListAdapter)ListView.Adapter).NotifyDataSetChanged();
		}

		void Model_OnRefreshing()
		{
			ViewHelper.ShowOverlay("Loading...");
		}

		void Model_OnRefreshed()
		{
			ViewHelper.HideOverlay();
		}

		void GoToMessaging(string userId, string userName)
		{
			var messagingActivity = new Intent(this, typeof(MessagingActivity));
			messagingActivity.PutExtra("toUserId", userId);
			messagingActivity.PutExtra("toUserName", userName);
			this.StartActivity(messagingActivity);
		}

		void RecommendationAdded(BadgeUserRecommendationDTO recommendation)
		{
			model.AddRecommendation(recommendation);
			model.HideCreateButton();
			ReloadCells();
		}

		void RecommendationsDataSource_OnWriteReviewButtonTouched()
		{
			var activity = new Intent(this, typeof(WriteReviewActivity));
			activity.PutExtra("userId", this.SearchResultDTO.Details.UserId);
			activity.PutExtra("badgeId", this.SearchResultDTO.Details.BadgeId);
			this.StartActivity(activity);
		}
	}
}
