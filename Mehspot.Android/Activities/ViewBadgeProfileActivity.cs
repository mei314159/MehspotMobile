
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
using Mehspot.iOS.Views.Cell;
using Mehspot.Models.ViewModels;

namespace Mehspot.AndroidApp
{
	[Activity(Label = "View Badge Profile")]
	public class ViewBadgeProfileActivity : Activity, IViewBadgeProfileController
	{
		private ViewBadgeProfileModel<View> model;
		public BadgeSummaryDTO BadgeSummary => Intent.GetExtra<BadgeSummaryDTO>("badgeSummary");
		public ISearchResultDTO SearchResultDTO => Intent.GetExtra<ISearchResultDTO>("searchResult");

		public IViewHelper ViewHelper { get; private set; }

		public TextView UserNameLabel => (TextView)FindViewById(Resource.ViewBadgeProfileActivity.UserNameLabel);
		public TextView LikesCount => (TextView)FindViewById(Resource.ViewBadgeProfileActivity.LikesCount);
		public TextView RecommendationsCount => (TextView)FindViewById(Resource.ViewBadgeProfileActivity.RecommendationsCount);
		public TextView DistanceLabel => (TextView)FindViewById(Resource.ViewBadgeProfileActivity.DistanceLabel);
		public TextView InfoLabel1View => (TextView)FindViewById(Resource.ViewBadgeProfileActivity.InfoLabel1);
		public TextView InfoLabel2View => (TextView)FindViewById(Resource.ViewBadgeProfileActivity.InfoLabel2);
		public TextView SubdivisionLabel => (TextView)FindViewById(Resource.ViewBadgeProfileActivity.SubdivisionLabel);

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
				return Segments.MessageButton.Visibility == ViewStates.Gone;
			}

			set
			{
				Segments.MessageButton.Visibility = value ? ViewStates.Gone : ViewStates.Visible;
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
			model = new ViewBadgeProfileModel<View>(new CellFactory(this, new BadgeService(MehspotAppContext.Instance.DataStorage), BadgeSummary.BadgeId), this);
			model.OnRefreshing += Model_OnRefreshing;
			model.OnRefreshed += Model_OnRefreshed;
			model.OnWriteReviewButtonTouched += RecommendationsDataSource_OnWriteReviewButtonTouched;
			model.OnGoToMessaging += GoToMessaging;

			await model.RefreshView();
		}

		protected override async void OnStart()
		{
			base.OnStart();
			await model.RefreshView();
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
			throw new NotImplementedException();
		}

		void Model_OnRefreshing()
		{
		}

		void Model_OnRefreshed()
		{
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
			//TODO: go to write recommendation window
		}
	}
}
