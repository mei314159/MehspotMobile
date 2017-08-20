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
		public ISearchResultDTO SearchResultDTO => Intent.GetExtra<ISearchResultDTO>("searchResult");

		public string BadgeName => Intent.GetStringExtra("badgeName");

		public string UserId => Intent.GetStringExtra("userId");

		public TextView UserNameLabel => FindViewById<TextView>(Resource.ViewBadgeProfileActivity.UserNameLabel);
		public TextView LikesCount => FindViewById<TextView>(Resource.ViewBadgeProfileActivity.LikesCount);
		public TextView DistanceLabel => FindViewById<TextView>(Resource.ViewBadgeProfileActivity.DistanceLabel);
		public TextView InfoLabel1View => FindViewById<TextView>(Resource.ViewBadgeProfileActivity.InfoLabel1);
		public TextView InfoLabel2View => FindViewById<TextView>(Resource.ViewBadgeProfileActivity.InfoLabel2);
		public TextView SubdivisionLabel => FindViewById<TextView>(Resource.ViewBadgeProfileActivity.SubdivisionLabel);
		public LinearLayout ContentWrapper => FindViewById<LinearLayout>(Resource.ViewBadgeProfileActivity.profileContentWrapper);

		public ImageView Picture => FindViewById<ImageView>(Resource.ViewBadgeProfileActivity.Picture);
		public ImageView FavoriteIcon => FindViewById<ImageView>(Resource.ViewBadgeProfileActivity.FavoriteIcon);
		public SegmentedControl Segments => FindViewById<SegmentedControl>(Resource.ViewBadgeProfileActivity.Segments);

		public IViewHelper ViewHelper { get; private set; }

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

		protected override void OnCreate(Bundle savedInstanceState)
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
		}

		protected override void OnResume()
		{
			base.OnResume();
			Segments.HighlightSelectedButton(Segments.DetailsButton);
			model.LoadProfile();
		}

		void DetailsButton_Click(object sender, EventArgs e)
		{
			Segments.HighlightSelectedButton(Segments.DetailsButton);
			model.LoadProfile();
		}

		async void RecommendationsButton_Click(object sender, EventArgs e)
		{
			Segments.HighlightSelectedButton(Segments.RecommendationsButton);
			await model.LoadRecommendations();
		}

		void MessageButton_Click(object sender, EventArgs e)
		{
			Segments.HighlightSelectedButton(Segments.MessageButton);
			GoToMessaging(this.UserId, this.FirstName);
		}

		protected override void OnStart()
		{
			base.OnStart();
			Segments.HighlightSelectedButton(Segments.DetailsButton);
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
			ContentWrapper.RemoveAllViews();
			foreach (var item in model.Cells)
			{
				ContentWrapper.AddView(item);
			}
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
			messagingActivity.PutExtra("toProfilePicturePath", this.model.Profile.Details.ProfilePicturePath);
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
			//activity.PutExtra("userId", this.SearchResultDTO.Details.UserId);
			//activity.PutExtra("badgeId", this.SearchResultDTO.Details.BadgeId);
			this.StartActivity(activity);
		}
	}
}
