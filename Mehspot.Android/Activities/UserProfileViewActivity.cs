using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Renderscripts;
using Android.Views;
using Android.Widget;
using Java.Net;
using Mehspot.AndroidApp.Resources.layout;
using Mehspot.AndroidApp.Views;
using Mehspot.AndroidApp.Wrappers;
using Mehspot.Core;
using Mehspot.Core.Contracts.ViewControllers;
using Mehspot.Core.Contracts.Wrappers;
using Mehspot.Core.DTO;
using Mehspot.Core.Models;
using Mehspot.Core.Services;

namespace Mehspot.AndroidApp
{
	[Activity(Label = "UserProfileView")]
	public class UserProfileViewActivity : Activity, IUserProfileViewController
	{
		private const int BlurRadius = 3;

		private UserProfileViewModel model;
		private BadgeService badgeService;
		private ProfileService profileService;
		private string profilePicturePath;
		private RenderScript renderScript;
		private List<BadgeSummaryItemSimpified> wrapList = new List<BadgeSummaryItemSimpified>();

		public ImageView BlurPicture => FindViewById<ImageView>(Resource.Id.UserProfileBlurPicture);
		public ImageView UserPicture => FindViewById<ImageView>(Resource.Id.UserProfilePicture);
		public View ServiceView => FindViewById<View>(Resource.Id.UserProfileServiceView);
		public TextView ReferencesCountLabel => FindViewById<TextView>(Resource.UserProfileViewActivity.referenceCount);
		public TextView RecommendationsCountLabel => FindViewById<TextView>(Resource.UserProfileViewActivity.recommendationCount);
		public TextView UserNameLabel => FindViewById<TextView>(Resource.UserProfileViewActivity.UserName);
		public Android.Support.V7.Widget.Toolbar ProfileToolbar => FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.UserProfileViewActivity.UserProfileToolbar);
		public LinearLayout ContentWrapper => this.FindViewById<LinearLayout>(Resource.UserProfileViewActivity.ContentWrapper);

		public string ToUserName => Intent.GetStringExtra("toUserName");
		public string ToUserId => Intent.GetStringExtra("toUserId");
		public IViewHelper ViewHelper { get; set; }

		#region IUserProfileViewController
		public string UserName
		{
			get
			{
				return ProfileToolbar.Title;
			}

			set
			{
				ProfileToolbar.Title = value;
			}
		}

		public string FullName
		{
			get
			{
				return UserNameLabel.Text;
			}

			set
			{
				UserNameLabel.Text = value;
			}
		}

		public string ProfilePicturePath
		{
			get
			{
				return profilePicturePath;
			}

			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					profilePicturePath = value;
					var url = new URL(profilePicturePath);

					if (profilePicturePath != null)
					{
						var bitmap = this.GetImageBitmapFromUrl(profilePicturePath);
						UserPicture.SetImageBitmap(bitmap);
						DisplayBlurredImage(BlurRadius, bitmap);
					}
				}
			}
		}

		public int RecommendationsCount
		{
			get
			{
				return Int32.Parse(RecommendationsCountLabel.Text);
			}

			set
			{
				RecommendationsCountLabel.Text = value.ToString();
			}
		}

		public int ReferencesCount
		{
			get
			{
				return Int32.Parse(ReferencesCountLabel.Text);
			}

			set
			{
				ReferencesCountLabel.Text = value.ToString();
			}
		}
		#endregion

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.UserProfileViewActivity);

			this.ViewHelper = new ActivityHelper(this);
			badgeService = new BadgeService(MehspotAppContext.Instance.DataStorage);
			profileService = new ProfileService(MehspotAppContext.Instance.DataStorage);

			UserPicture.ClipToOutline = true;
			renderScript = RenderScript.Create(this);

			ViewHelper.ShowOverlay("Wait...", true);
			model = new UserProfileViewModel(badgeService, profileService, this);
			model.UserId = this.ToUserId;
		}

		protected override async void OnStart()
		{
			base.OnStart();

			if (!model.dataLoaded)
			{
				await model.LoadAsync();
			}
		}

		public void ReloadData()
		{
			DisplayBadges();
		}

		private BadgeSummaryItemSimpified CreateItem(BadgeSummaryBaseDTO dto)
		{
			var item = new BadgeSummaryItemSimpified(this, dto);
			item.Tag = dto.BadgeId;
			item.Clicked += Item_Clicked;

			return item;
		}

		private void Item_Clicked(BadgeSummaryBaseDTO dto, BadgeSummaryItemSimpified sender)
		{
			this.model.SelectBadge(dto);

			var selectedBadge = model.SelectedBadge;
			var target = new Intent(this, typeof(ViewBadgeProfileActivity));
			target.PutExtra("badgeId", selectedBadge.BadgeId);
			target.PutExtra("badgeName", selectedBadge.BadgeName);
			target.PutExtra("userId", model.UserId);
			this.StartActivity(target);
		}

		public void DisplayBadges()
		{
			ContentWrapper.RemoveAllViews();

			foreach (var element in wrapList)
			{
				element.Dispose();
			}

			wrapList.Clear();

			foreach (var item in model.Items)
			{
				var bubble = CreateItem(item);
				ContentWrapper.AddView(bubble);
				wrapList.Add(bubble);
			}
		}

		private void DisplayBlurredImage(int radius, Bitmap photo)
		{
			Task.Run(() =>
			{
				Bitmap bmp = CreateBlurredImage(radius, photo);
				return bmp;
			}).ContinueWith(task =>
			{
				Bitmap bmp = task.Result;
				BlurPicture.SetImageBitmap(bmp);
			}, TaskScheduler.FromCurrentSynchronizationContext());
		}

		private Bitmap CreateBlurredImage(int radius, Bitmap photo)
		{
			Bitmap blurredBitmap = photo.Copy(photo.GetConfig(), true);

			ScriptIntrinsicBlur script = ScriptIntrinsicBlur.Create(renderScript, Element.U8_4(renderScript));

			Allocation input = Allocation.CreateFromBitmap(renderScript, blurredBitmap,
														   Allocation.MipmapControl.MipmapFull,
														   AllocationUsage.Script);

			script.SetInput(input);
			script.SetRadius(radius);

			Allocation output = Allocation.CreateTyped(renderScript, input.Type);

			script.ForEach(output);
			output.CopyTo(blurredBitmap);
			output.Destroy();
			input.Destroy();
			script.Destroy();

			return blurredBitmap;
		}

		public void ShowLabel()
		{
			var ScrollView = FindViewById<ScrollView>(Resource.UserProfileViewActivity.profileScrollView);
			var Message = FindViewById<TextView>(Resource.UserProfileViewActivity.Message);

			ScrollView.Visibility = ViewStates.Invisible;
			Message.Visibility = ViewStates.Visible;
		}
	}
}
