using Foundation;
using System;
using UIKit;
using Mehspot.iOS.Controllers;
using System.Collections.Generic;
using Mehspot.Core.DTO.Subdivision;
using System.Threading.Tasks;
using Mehspot.Core.Services;
using Mehspot.Core;
using Mehspot.iOS.Wrappers;
using Mehspot.Core.Contracts.Wrappers;
using CoreGraphics;
using Mehspot.iOS.Extensions;

namespace mehspot.iOS
{
	public delegate void WalkthroughStep2Delegate(string zip, int subdivisionId, int? optionId);
	public partial class WalkthroughStep2Controller : UIViewController
	{
		private SubdivisionService subdivisionService;
		private IViewHelper viewHelper;
		private bool subdivisionSelectorEnabled;

		private bool subdivisionsLoaded;
		private NSObject willHideNotificationObserver;
		private NSObject willShowNotificationObserver;


		public int? SelectedSubdivisionId { get; set; }
		public int? SelectedSubdivisionOptionId { get; set; }
		public string ZipCode { get; set; }

		List<SubdivisionDTO> Subdivisions;
		public event WalkthroughStep2Delegate OnContinue;


		public WalkthroughStep2Controller(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			subdivisionService = new SubdivisionService(MehspotAppContext.Instance.DataStorage);
			this.View.AddGestureRecognizer(new UITapGestureRecognizer(this.HideKeyboard));
			viewHelper = new ViewHelper(this.ParentViewController.View);
			ZipField.Layer.BorderWidth = SubdivisionField.Layer.BorderWidth = 1;
			ZipField.Layer.BorderColor = SubdivisionField.Layer.BorderColor = UIColor.FromRGB(174, 210, 122).CGColor;
			ZipField.Mask = "#####";
			ZipField.Placeholder = "Zip Code";
			ZipField.Text = ZipCode;
			ZipField.EditingChanged += ZipField_ValueChanged;
			subdivisionSelectorEnabled = ZipField.IsValid;
			View.LayoutIfNeeded();
			ProfilePicture.Layer.CornerRadius = ProfilePicture.Frame.Width / 2;
		}

		public override void ViewWillAppear(bool animated)
		{
			RegisterForKeyboardNotifications();
		}

		public override void ViewDidDisappear(bool animated)
		{
			if (willHideNotificationObserver != null)
				NSNotificationCenter.DefaultCenter.RemoveObserver(willHideNotificationObserver);
			if (willShowNotificationObserver != null)
				NSNotificationCenter.DefaultCenter.RemoveObserver(willShowNotificationObserver);
		}


		public async override void ViewDidAppear(bool animated)
		{
			if (!subdivisionsLoaded && subdivisionSelectorEnabled)
			{
				await LoadSubdivisionsAsync();
			}
		}

		partial void ContinueButtonTouched(UIButton sender)
		{
			if (!string.IsNullOrWhiteSpace(this.ZipField.Text) && SelectedSubdivisionId.HasValue && OnContinue != null)
			{
				OnContinue(this.ZipField.Text, this.SelectedSubdivisionId.Value, this.SelectedSubdivisionOptionId);
			}
		}

		async partial void SubdivisionButtonTouched(UIButton sender)
		{
			if (!subdivisionSelectorEnabled)
			{
				return;
			}

			var subdivisionsListController = new SubdivisionsListController();
			subdivisionsListController.ZipCode = this.ZipField.Text;

			subdivisionsListController.Subdivisions = this.Subdivisions;

			subdivisionsListController.SelectedSubdivisionId = this.SelectedSubdivisionId;

			subdivisionsListController.OnDismissed += (dto) =>
						{
							SubdivisionField.SetTitle(dto.DisplayName, UIControlState.Normal);

							SelectedSubdivisionId = dto.Id;
							SelectedSubdivisionOptionId = dto.OptionId;
							this.ContinueButton.Hidden = false;
						};

			await PresentViewControllerAsync(subdivisionsListController, true);
		}

		async void ZipField_ValueChanged(object sender, EventArgs e)
		{
			if (ZipField.IsValid)
			{
				this.HideKeyboard();
			}

			await LoadSubdivisionsAsync();
			if (ZipField.IsValid)
			{
				SubdivisionButtonTouched(SubdivisionField);
			}
		}

		private async Task LoadSubdivisionsAsync()
		{
			subdivisionSelectorEnabled = false;
			if (ZipField.IsValid)
			{
				viewHelper.ShowOverlay("Loading Subdivisions");
				var subdivisionsResult = await subdivisionService.ListSubdivisionsAsync(ZipField.Text).ConfigureAwait(false);
				if (subdivisionsResult.IsSuccess)
				{
					this.subdivisionsLoaded = true;
					Subdivisions = subdivisionsResult.Data;
				}
				else
				{
					viewHelper.ShowAlert("Error", subdivisionsResult.ErrorMessage);
				}

				InvokeOnMainThread(() => SubdivisionField
								   .SetTitle(Subdivisions?.Count <= 0 ? "Add Subdivision" : "Subdivision", UIControlState.Normal));
				viewHelper.HideOverlay();
			}

			InvokeOnMainThread(() =>
			{
				subdivisionSelectorEnabled = ZipField.IsValid;
			});
		}

		protected virtual void RegisterForKeyboardNotifications()
		{
			this.willHideNotificationObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, OnKeyboardNotification);
			this.willShowNotificationObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, OnKeyboardNotification);
		}

		public void OnKeyboardNotification(NSNotification notification)
		{
			if (!IsViewLoaded)
				return;

			//Check if the keyboard is becoming visible
			var visible = notification.Name == UIKeyboard.WillShowNotification;

			//Start an animation, using values from the keyboard
			UIView.BeginAnimations("AnimateForKeyboard");
			UIView.SetAnimationBeginsFromCurrentState(true);
			UIView.SetAnimationDuration(UIKeyboard.AnimationDurationFromNotification(notification));
			UIView.SetAnimationCurve((UIViewAnimationCurve)UIKeyboard.AnimationCurveFromNotification(notification));

			//Pass the notification, calculating keyboard height, etc.
			var keyboardFrame = visible
									? UIKeyboard.FrameEndFromNotification(notification)
									: UIKeyboard.FrameBeginFromNotification(notification);
			OnKeyboardChanged(visible, keyboardFrame);
			//Commit the animation
			UIView.CommitAnimations();
		}

		public virtual void OnKeyboardChanged(bool visible, CGRect keyboardFrame)
		{
			if (View.Superview == null)
			{
				return;
			}
			if (visible)
			{
				this.Top.Constant = -130;
			}
			else
			{
				this.Top.Constant = 0;
			}

			this.View.LayoutIfNeeded();
		}
	}
}