using System;
using UIKit;
using Mehspot.Core.Models;
using Mehspot.Core;
using Mehspot.iOS.Wrappers;
using Mehspot.Core.DTO;
using Mehspot.iOS.Extensions;
using Mehspot.Core.Services;

namespace Mehspot.iOS
{
	public partial class ResetPasswordViewController : UIViewController
	{
		ResetPasswordModel model;
		public ResetPasswordViewController(IntPtr handle) : base(handle)
		{
		}


		public string Email { get; set; }
		public string Code { get; set; }

		public override void ViewDidLoad()
		{
			model = new ResetPasswordModel(MehspotAppContext.Instance.AuthManager, new ProfileService(MehspotAppContext.Instance.DataStorage), new ViewHelper(this.View));
			model.OnResetPasswordSuccess += OnResetPasswordSuccess;
			model.SignedIn += Model_SignedIn;
			this.View.AddGestureRecognizer(new UITapGestureRecognizer(this.HideKeyboard));
			this.PasswordField.ShouldReturn += TextFieldShouldReturn;
			this.ConfirmationPasswordField.ShouldReturn += TextFieldShouldReturn;
		}

		partial void ResetPasswordButtonTouched(UIButton sender)
		{
			sender.BecomeFirstResponder();
			ResetPasswordAsync();
		}

		private async void OnResetPasswordSuccess(Result result)
		{
			await model.SignInAsync(this.Email, this.PasswordField.Text);
		}

		void Model_SignedIn(Mehspot.Core.Auth.AuthenticationResult result, ProfileDto profile)
		{
			var targetViewController = UIStoryboard.FromName("Main", null).InstantiateInitialViewController();
			this.View.Window.SwapController(targetViewController);
		}

		private async void ResetPasswordAsync()
		{
			await model.ResetPasswordAsync(this.Email, this.Code, this.PasswordField.Text, this.ConfirmationPasswordField.Text);
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
				ResetPasswordAsync();
			}

			return false; // We do not want UITextField to insereaks.
		}

		partial void BackButtonTouched(UIBarButtonItem sender)
		{
			var targetViewController = UIStoryboard.FromName("Login", null).InstantiateViewController("LoginViewController");
			this.View.Window.SwapController(targetViewController);
		}
	}
}