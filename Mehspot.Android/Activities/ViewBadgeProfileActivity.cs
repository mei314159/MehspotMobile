
using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
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

		public Button SearchButton => this.FindViewById<Button>(Resource.SearchFilter.SearchButton);

		#region IViewBadgeProfileController

		public string WindowTitle
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		public string Subdivision
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		public string Distance
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		public string Likes
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		public string FirstName
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		public string InfoLabel1
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		public string InfoLabel2
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		public bool HideFavoriteIcon
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		public bool EnableSendMessageButton
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
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
			//Reload recommendations;
		}

		void RecommendationsDataSource_OnWriteReviewButtonTouched()
		{
			//TODO: go to write recommendation window
		}
	}
}
