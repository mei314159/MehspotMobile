
using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Java.Lang;
using Mehspot.Core.Auth;
using Mehspot.AndroidApp.Wrappers;
using Mehspot.Core;
using Mehspot.Core.Models;
using Xamarin.Facebook;
using Xamarin.Facebook.Login;
using Xamarin.Facebook.Login.Widget;

namespace Mehspot.AndroidApp
{
	[Activity(Label = "Sign In")]
	public class SignInActivity : Activity, IFacebookCallback
	{
		SignInModel model;
		ICallbackManager callbackManager;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.SignIn);
			model = new SignInModel(MehspotAppContext.Instance.AuthManager, new ActivityHelper(this));
			model.SignedIn += Model_SignedIn;

			FindViewById<Button>(Resource.Id.SignIn_Button).Click += SignInButton_Click;
			FindViewById<Button>(Resource.Id.GoToSignUp_Button).Click += GoToSignUpButton_Click;

			var fbButton = FindViewById<LoginButton>(Resource.Id.fb_login_button);
			fbButton.SetReadPermissions(SignInModel.FbReadPermissions);

			callbackManager = CallbackManagerFactory.Create();
			fbButton.RegisterCallback(callbackManager, this);
		}

		private void GoToSignUpButton_Click(object sender, EventArgs e)
		{
			this.StartActivity(new Intent(Application.Context, typeof(SignUpActivity)));
		}

		void SignInButton_Click(object sender, EventArgs e)
		{
			SignInAsync();
		}

		private async void SignInAsync()
		{
			var emailField = FindViewById<EditText>(Resource.Id.EmailField);
			var passwordField = FindViewById<EditText>(Resource.Id.PasswordField);
			await model.SignInAsync(emailField.Text, passwordField.Text);
		}

		private void Model_SignedIn(AuthenticationResult result)
		{
			this.StartActivity(new Intent(Application.Context, typeof(MainActivity)));
		}

		public void OnCancel()
		{

		}

		public void OnError(FacebookException p0)
		{
		}

		public async void OnSuccess(Java.Lang.Object p0)
		{
			var e = (LoginResult)p0;
			await model.SignInExternalAsync(e.AccessToken.Token, "Facebook");
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);
			callbackManager.OnActivityResult(requestCode, (int)resultCode, data);
		}

		public override void OnBackPressed()
		{
			Finish();
		}
	}
}
