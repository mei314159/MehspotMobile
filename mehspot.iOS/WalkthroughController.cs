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

namespace mehspot.iOS
{
	public partial class WalkthroughController : UIPageViewController
	{
		private WalkthroughStep1Controller step1;
		private WalkthroughStep2Controller step2;
		ProfileService profileService;

		ProfileDto profile;
		byte[] profileImage;
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
			step1.OnContinue += Step1OnContinue;
			step2.OnContinue += Step2OnContinue;
			this.DataSource = new PageDataSource(new List<UIViewController> { step1, step2 });
			this.SetViewControllers(new[] { step1 }, UIPageViewControllerNavigationDirection.Forward, false, (finished) => { });

			viewHelper.ShowOverlay("Loading Profile");
			var result = await profileService.GetProfileAsync();
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

		public override void ViewDidAppear(bool animated)
		{
			foreach (var pc in this.View.Subviews.OfType<UIPageControl>())
			{
				pc.PageIndicatorTintColor = UIColor.LightGray;
				pc.CurrentPageIndicatorTintColor = UIColor.FromRGB(174, 210, 122);
				pc.BackgroundColor = UIColor.White;
			}
		}

		public override bool PrefersStatusBarHidden()
		{
			return true;
		}

		void Step1OnContinue(byte[] image)
		{
			this.profileImage = image;
			this.SetViewControllers(new[] { step2 }, UIPageViewControllerNavigationDirection.Forward, true, (finished) => { });
		}

		async void Step2OnContinue(string zip, int subdivisionId, int? optionId)
		{
			profile.Zip = zip;
			profile.SubdivisionId = subdivisionId;
			profile.SubdivisionOptionId = optionId;
			if (profileImage == null && string.IsNullOrWhiteSpace(profile.ProfilePicturePath))
			{
				viewHelper.ShowPrompt("Error", "Please, upload a profile picture", "OK", () =>
				{
					this.SetViewControllers(new[] { step1 }, UIPageViewControllerNavigationDirection.Reverse, true, (finished) => { });
				});
				return;
			}

			viewHelper.ShowOverlay("Saving profile");
			Result photoResult = null;
			if (profileImage != null)
			{
				photoResult = await profileService.UploadProfileImageAsync(this.profileImage);
			}

			if (photoResult == null || photoResult.IsSuccess)
			{
				var result = await profileService.UpdateAsync(this.profile);
				viewHelper.HideOverlay();
				if (result.IsSuccess)
				{
					MehspotAppContext.Instance.DataStorage.WalkthroughPassed = true;
					var targetViewController = UIStoryboard.FromName("Main", null).InstantiateInitialViewController();
					View.Window.SwapController(targetViewController);
				}
				else
				{
					viewHelper.ShowAlert("Error", "Can not save user profile");
				}
			}
			else
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
			}

			base.Dispose(disposing);
		}
	}
}