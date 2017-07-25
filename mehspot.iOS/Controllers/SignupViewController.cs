using Foundation;
using System;
using UIKit;
using Mehspot.Core;
using Mehspot.Core.Models;
using Mehspot.iOS.Wrappers;
using Mehspot.Core.DTO;
using Mehspot.iOS.Extensions;
using CoreGraphics;
using Mehspot.Core.Auth;
using Mehspot.Core.Services;

namespace Mehspot.iOS
{
	public partial class SignupViewController : UIViewController
	{
		private NSObject willHideNotificationObserver;
		private NSObject willShowNotificationObserver;
		SignUpModel model;
		ViewHelper viewHelper;
		public SignupViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			viewHelper = new ViewHelper(this.View);
			model = new SignUpModel(MehspotAppContext.Instance.AuthManager, new ProfileService(MehspotAppContext.Instance.DataStorage), viewHelper);
			model.SignedUp += Model_SignedUp;
			model.SignedIn += Model_SignedIn;
			this.View.AddGestureRecognizer(new UITapGestureRecognizer(this.HideKeyboard));
			this.EmailField.ShouldReturn += TextFieldShouldReturn;
			this.UserNameField.ShouldReturn += TextFieldShouldReturn;
			this.PasswordField.ShouldReturn += TextFieldShouldReturn;
			this.ConfirmationPasswordField.ShouldReturn += TextFieldShouldReturn;

		}

		public override void ViewDidAppear(bool animated)
		{
			RegisterForKeyboardNotifications();
			this.ScrollView.ContentSize = new CGSize(ScrollView.ContentSize.Width, ScrollView.ContentSize.Height + 170);
		}

		public override void ViewDidDisappear(bool animated)
		{
			if (willHideNotificationObserver != null)
				NSNotificationCenter.DefaultCenter.RemoveObserver(willHideNotificationObserver);
			if (willShowNotificationObserver != null)
				NSNotificationCenter.DefaultCenter.RemoveObserver(willShowNotificationObserver);
		}

		private async void Model_SignedUp(Result result)
		{
			await model.SignInAsync(this.EmailField.Text, this.PasswordField.Text);
		}

		void Model_SignedIn(AuthenticationResult result, ProfileDto profile)
		{
			this.viewHelper.ShowOverlay("Wait...");
			UIViewController targetViewController;
			targetViewController = UIStoryboard.FromName("Walkthrough", null).InstantiateInitialViewController();
			this.View.Window.SwapController(targetViewController);
		}

		private async void SignUpAsync()
		{
			await model.SignUpAsync(this.EmailField.Text, this.UserNameField.Text, this.PasswordField.Text, this.ConfirmationPasswordField.Text, AgreeWithTerms.On);
		}

		private bool TextFieldShouldReturn(UITextField textField)
		{
			var nextTag = textField.Tag + 1;
			UIResponder nextResponder = this.View.ViewWithTag(nextTag);
			if (nextResponder != null)
			{
				nextResponder.BecomeFirstResponder();
			}
			else
			{
				// Not found, so remove keyboard.
				textField.ResignFirstResponder();
				SignUpAsync();
			}

			return false; // We do not want UITextField to insert line-breaks.
		}

		partial void CommunityGuidelinesButtonTouched(UIButton sender)
		{
			UIApplication.SharedApplication.OpenUrl(new NSUrl(Constants.ApiHost + "/Account/CommunityGuidelines"));
		}

		partial void PrivacyPolicyButtonTouched(UIButton sender)
		{
			UIApplication.SharedApplication.OpenUrl(new NSUrl(Constants.ApiHost + "/Account/PrivacyPolicy"));
		}

		partial void TermsofUseButtonTouched(UIButton sender)
		{
			UIApplication.SharedApplication.OpenUrl(new NSUrl(Constants.ApiHost + "/Account/TermsOfUse"));
		}

		partial void SignupButtonTouched(UIButton sender)
		{
			this.HideKeyboard();
			sender.BecomeFirstResponder();
			SignUpAsync();
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
				this.ScrollView.ContentOffset = new CGPoint(0, 170);
			}
			else
			{
				this.ScrollView.ContentOffset = new CGPoint(0, 0);
			}
		}
	}
}