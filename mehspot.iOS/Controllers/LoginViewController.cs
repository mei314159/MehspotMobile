using System;
using UIKit;
using Mehspot.Core.Models;
using Mehspot.iOS.Wrappers;
using Mehspot.Core.Auth;
using Mehspot.Core;
using Mehspot.iOS.Extensions;
using Foundation;
using Facebook.CoreKit;
using Facebook.LoginKit;
using CoreGraphics;

namespace Mehspot.iOS
{
	public partial class LoginViewController : UIViewController
	{

		SignInModel model;
		LoginButton loginView;
		public LoginViewController(IntPtr handle) : base(handle)
		{
			model = new SignInModel(MehspotAppContext.Instance.AuthManager, new ViewHelper(this.View));
			model.SignedIn += Model_SignedIn;
			model.SignInError += Model_SignInError;
		}

		[Action("UnwindToLoginViewController:")]
		public void UnwindToLoginViewController(UIStoryboardSegue segue)
		{
		}

		public override void ViewDidLoad()
		{
            RegisterForKeyboardNotifications ();
			this.View.AddGestureRecognizer(new UITapGestureRecognizer(HideKeyboard));
			this.EmailField.ShouldReturn += TextFieldShouldReturn;
			this.PasswordField.ShouldReturn += TextFieldShouldReturn;
			Profile.Notifications.ObserveDidChange((sender, e) =>
			{

				if (e.NewProfile == null)
					return;

				//nameLabel.Text = e.NewProfile.Name;
			});

			// Set the Read and Publish permissions you want to get
			loginView = new LoginButton(new CGRect(0, 0, 218, 46))
			{
				LoginBehavior = LoginBehavior.Native,
				ReadPermissions = SignInModel.FbReadPermissions
			};

			// Handle actions once the user is logged in
			loginView.Completed += async (sender, e) =>
			{
				if (e.Error != null || e.Result.IsCancelled)
				{
					return;
				}

				await model.SignInExternalAsync(e.Result.Token.TokenString, "Facebook");
			};
			this.FbAuthButtonWrapper.AddSubview(loginView);
		}

		public override void ViewDidAppear(bool animated)
		{
			this.ScrollView.ContentSize = new CGSize(ScrollView.ContentSize.Width, ScrollView.ContentSize.Height + 110);
		}

		partial void SignInButtonTouched(UIButton sender)
		{
			sender.BecomeFirstResponder();
			SignInAsync();
		}

		private void Model_SignedIn(AuthenticationResult result)
		{
			var targetViewController = UIStoryboard.FromName("Main", null).InstantiateInitialViewController();
			this.View.Window.SwapController(targetViewController);
		}

		private async void SignInAsync()
		{
			await model.SignInAsync(this.EmailField.Text, this.PasswordField.Text);
		}

		void Model_SignInError(AuthenticationResult result)
		{
			new LoginManager().LogOut();
		}

		private bool TextFieldShouldReturn(UITextField textField)
		{
			var nextTag = textField.Tag + 1;
			UIResponder nextResponder = this.View.ViewWithTag(nextTag);
			if (nextResponder != null && nextTag < 2)
			{
				nextResponder.BecomeFirstResponder();
			}
			else
			{
				// Not found, so remove keyboard.
				textField.ResignFirstResponder();
				SignInAsync();
			}

			return false; // We do not want UITextField to insert line-breaks.
		}

		public void HideKeyboard()
		{
			this.View.FindFirstResponder()?.ResignFirstResponder();
		}

		protected virtual void RegisterForKeyboardNotifications()
		{
			NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, OnKeyboardNotification);
			NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, OnKeyboardNotification);
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
				this.ScrollView.ContentOffset = new CGPoint(0, 110);
			}
			else
			{
				this.ScrollView.ContentOffset = new CGPoint(0, 0);
			}
		}
	}
}