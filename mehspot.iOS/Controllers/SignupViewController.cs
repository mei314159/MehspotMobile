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
using System.Threading.Tasks;

namespace Mehspot.iOS
{
	public partial class SignupViewController : UIViewController
	{
		private const float IPhone5ScreenHeight = 568f;

		private NSObject willHideNotificationObserver;
		private NSObject willShowNotificationObserver;
		private SignUpModel model;
		private ViewHelper viewHelper;


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
			this.PasswordField.EditingChanged += (sender, e) => ShowPasswordButton.Hidden = string.IsNullOrEmpty(this.PasswordField.Text);
			this.ConfirmationPasswordField.EditingChanged += (sender, e) => ShowConfirmationPasswordButton.Hidden = string.IsNullOrEmpty(this.ConfirmationPasswordField.Text);
			this.ConfirmationPasswordField.ShouldReturn += TextFieldShouldReturn;
			this.SetScreenConstraints();
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

		private void SetScreenConstraints()
		{
			if (UIScreen.MainScreen.Bounds.Height > IPhone5ScreenHeight)
			{
				this.ScrollView.ScrollEnabled = false;
			}
		}

		private async void Model_SignedUp(Result result)
		{
			string email = null;
			string password = null;
			InvokeOnMainThread(() =>
			{
				email = this.EmailField.Text;
				password = this.PasswordField.Text;
			});

			model.SignInAsync(email, password);
		}

		private void Model_SignedIn(AuthenticationResult result, ProfileDto profile)
		{
			InvokeOnMainThread(() =>
			{
				this.viewHelper.ShowOverlay("Wait...");
				UIViewController targetViewController = UIStoryboard.FromName("Walkthrough", null).InstantiateInitialViewController();
				this.View.Window.SwapController(targetViewController);
			});
		}

		private async Task SignUpAsync()
		{
			await model.SignUpAsync(this.EmailField.Text, this.UserNameField.Text, this.PasswordField.Text, this.ConfirmationPasswordField.Text, AgreeWithTerms.On).ConfigureAwait(false);
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

		partial void ShowPasswordButtonTouchedUp(UIButton sender)
		{
			PasswordField.SecureTextEntry = true;
		}

		partial void ShowPasswordButtonTouchedDown(UIButton sender)
		{
			PasswordField.SecureTextEntry = false;
		}

		partial void ShowConfirmationPasswordButtonTouchedUp(UIButton sender)
		{
			ConfirmationPasswordField.SecureTextEntry = true;
		}

		partial void ShowConfirmationPasswordButtonTouchedDown(UIButton sender)
		{
			ConfirmationPasswordField.SecureTextEntry = false;
		}
	}
}
