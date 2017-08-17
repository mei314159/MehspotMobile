using System;
using UIKit;
using System.Collections.Generic;
using System.Linq;
using Mehspot.Core.Services;
using Mehspot.Core;
using Mehspot.Core.DTO;
using Mehspot.iOS.Extensions;
using Mehspot.Core.Contracts.Wrappers;
using Mehspot.iOS.Wrappers;
using System.IO;
using System.Threading.Tasks;

namespace mehspot.iOS
{
	public partial class WalkthroughController : UIPageViewController
	{
		private WalkthroughStep1Controller step1;
		private WalkthroughStep2Controller step2;
		private WalkthroughStep3Controller step3;
		private WalkthroughStep4Controller step4;
		ProfileService profileService;

		ProfileDto profile;
		Stream profileImageStream;
		private IViewHelper viewHelper;


		public WalkthroughController(IntPtr handle) : base(handle)
		{
		}

		public override async void ViewDidLoad()
		{
			viewHelper = new ViewHelper(this.View);
			profileService = new ProfileService(MehspotAppContext.Instance.DataStorage);
			step1 = (WalkthroughStep1Controller)Storyboard.InstantiateViewController("WalkthroughStep1Controller");
			step2 = (WalkthroughStep2Controller)Storyboard.InstantiateViewController("WalkthroughStep2Controller");
			step3 = (WalkthroughStep3Controller)Storyboard.InstantiateViewController("WalkthroughStep3Controller");
			step4 = (WalkthroughStep4Controller)Storyboard.InstantiateViewController("WalkthroughStep4Controller");
			step1.OnContinue += Step1OnContinue;
			step2.OnContinue += Step2OnContinue;
			step3.OnContinue += Step3OnContinue;
			step4.OnContinue += Step4OnContinue;
			this.DataSource = new PageDataSource(new List<UIViewController> { step1, step2, step3, step4 });
			this.SetViewControllers(new[] { step1 }, UIPageViewControllerNavigationDirection.Forward, false, (finished) => { });
			LoadProfile();
		}

		public override void ViewDidAppear(bool animated)
		{
			foreach (var pc in this.View.Subviews.OfType<UIPageControl>())
			{
				pc.PageIndicatorTintColor = UIColor.LightGray;
				pc.CurrentPageIndicatorTintColor = UIColor.FromRGB(174, 210, 122);
				pc.BackgroundColor = UIColor.White;
			}
		}

		async Task LoadProfile()
		{
			viewHelper.ShowOverlay("Loading Profile");
			var result = await profileService.LoadProfileAsync();
			viewHelper.HideOverlay();
			if (result.IsSuccess)
			{
				this.profile = result.Data;
				step1.ProfilePicturePath = profile.ProfilePicturePath;
				step2.ZipCode = profile.Zip;
				step2.SelectedSubdivisionId = profile.SubdivisionId;
			}
			else
			{
				viewHelper.ShowAlert("Error", "Can not load profile");
			}
		}

		public override bool PrefersStatusBarHidden()
		{
			return true;
		}

		void Step1OnContinue(Stream imageStream)
		{
			this.profileImageStream = imageStream;
			this.SetViewControllers(new[] { step2 }, UIPageViewControllerNavigationDirection.Forward, true, null);
		}

		void Step2OnContinue(string zip, int subdivisionId, int? optionId)
		{
			profile.Zip = zip;
			profile.SubdivisionId = subdivisionId;
			profile.SubdivisionOptionId = optionId;
			this.SetViewControllers(new[] { step3 }, UIPageViewControllerNavigationDirection.Forward, true, null);
		}

		void Step3OnContinue()
		{
			this.SetViewControllers(new[] { step4 }, UIPageViewControllerNavigationDirection.Forward, true, null);
		}

		async void Step4OnContinue(BadgeGroup badgeGroup)
		{
			if (profile == null)
			{
				await LoadProfile();
				return;
			}

			if (profileImageStream == null && string.IsNullOrWhiteSpace(profile.ProfilePicturePath))
			{
				viewHelper.ShowPrompt("Error", "Please, upload a profile picture", "OK", () =>
				{
					this.SetViewControllers(new[] { step1 }, UIPageViewControllerNavigationDirection.Reverse, true, null);
				});
				return;
			}

			if (profile.Zip == null)
			{
				viewHelper.ShowPrompt("Error", "Please, set Zip code and Subdivision", "OK", () =>
				{
					this.SetViewControllers(new[] { step2 }, UIPageViewControllerNavigationDirection.Reverse, true, null);
				});
				return;
			}

			viewHelper.ShowOverlay("Saving profile");
			if (profileImageStream != null)
			{
				await UploadProfilePictureAsync(profileImageStream);
			}

			var result = await profileService.UpdateAsync(this.profile);

			if (result.IsSuccess)
			{
				MehspotAppContext.Instance.DataStorage.PreferredBadgeGroup = badgeGroup;
				InvokeOnMainThread(() =>
				{
					var targetViewController = UIStoryboard.FromName("Main", null).InstantiateInitialViewController();
					View.Window.SwapController(targetViewController);
				});
			}
			else
			{
				viewHelper.ShowAlert("Error", "Can not save user profile");
				viewHelper.HideOverlay();
			}

		}

		async Task UploadProfilePictureAsync(Stream photoStream)
		{
			var photoResult = await profileService.UploadProfileImageAsync(photoStream).ConfigureAwait(false);
			if (photoResult != null && !photoResult.IsSuccess)
			{
				viewHelper.ShowAlert("Error", "Can not save profile picture");
				viewHelper.HideOverlay();
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				step1?.Dispose();
				step2?.Dispose();
				step3?.Dispose();
				step4?.Dispose();
			}

			base.Dispose(disposing);
		}
	}
}