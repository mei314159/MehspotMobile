﻿using System;
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
using Mehspot.Core.Services;
using Mehspot.AndroidApp.Activities;
using Android.Content.PM;

namespace Mehspot.AndroidApp
{
	[Activity(Label = "Sign In", ScreenOrientation = ScreenOrientation.Portrait)]
	public class SignInActivity : Activity, IFacebookCallback
    {
		ActivityHelper activityHelper;
        SignInModel model;
        ICallbackManager callbackManager;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SignIn);
            activityHelper = new ActivityHelper(this);
            model = new SignInModel(MehspotAppContext.Instance.AuthManager, new ProfileService(MehspotAppContext.Instance.DataStorage), activityHelper);
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

		private void Model_SignedIn(AuthenticationResult result, Mehspot.Core.DTO.ProfileDto profile)
		{
			
            this.activityHelper.ShowOverlay("Wait...");
            RunOnUiThread(() =>
			{
				if (string.IsNullOrWhiteSpace(profile.Zip) || profile.SubdivisionId == null || string.IsNullOrWhiteSpace(profile.ProfilePicturePath))
				{
                    this.StartActivity(new Intent(Application.Context, typeof(WalkthroughActivity)));
				}
				else
				{
					this.StartActivity(new Intent(Application.Context, typeof(MainActivity)));
				}
			});
		}

		public void OnCancel()
		{

		}

		public async void OnError(FacebookException p0)
		{
			await model.SignInExternalErrorAsync("No internet connection.");
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
