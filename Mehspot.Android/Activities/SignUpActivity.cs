﻿
using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using mehspot.Core.Auth;
using Mehspot.AndroidApp.Wrappers;
using Mehspot.Core;
using Mehspot.Core.Models;

namespace Mehspot.AndroidApp
{
    [Activity(Label = "Sign Up")]
    public class SignUpActivity : Activity
    {
        SignUpModel model;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            model = new SignUpModel(MehspotAppContext.Instance.AuthManager, new ActivityHelper(this));
            model.SignedIn += Model_SignedIn;
            model.SignedUp += Model_SignedUp;

            SetContentView(Resource.Layout.SignUp);

            FindViewById<Button>(Resource.Id.SignUpButton).Click += SignUpButton_Click;
            TermsOfUseLabel.Click += TermsOfUseLabel_Click;
            PrivacyPolicyLabel.Click += PrivacyPolicyLabel_Click;
            CommunityGuidelinesLabel.Click += CommunityGuidelinesLabel_Click;
        }

        private void CommunityGuidelinesLabel_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void PrivacyPolicyLabel_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void TermsOfUseLabel_Click(object sender, EventArgs e)
        {
            var uri = Android.Net.Uri.Parse("http://www.xamarin.com");
            var intent = new Intent(Intent.ActionView, uri);
			StartActivity(intent);
        }

        public EditText UserNameField => FindViewById<EditText>(Resource.Id.UserNameField);
        public EditText EmailField => FindViewById<EditText>(Resource.Id.EmailField);
        public EditText PasswordField => FindViewById<EditText>(Resource.Id.PasswordField);
        public EditText ConfirmationPasswordField => FindViewById<EditText>(Resource.Id.ConfirmationPasswordField);

        public TextView TermsOfUseLabel => FindViewById<TextView>(Resource.Id.TermsOfUseLabel);
        public TextView PrivacyPolicyLabel => FindViewById<TextView>(Resource.Id.PrivacyPolicyLabel);
        public TextView CommunityGuidelinesLabel => FindViewById<TextView>(Resource.Id.CommunityGuidelinesLabel);
        public CheckBox AgreeWithTerms => FindViewById<CheckBox>(Resource.Id.AgreeWithTerms);

        void SignUpButton_Click(object sender, EventArgs e)
        {
            SignUpAsync();
        }

        private async void SignUpAsync()
        {
            await model.SignUpAsync(this.EmailField.Text, this.UserNameField.Text, this.PasswordField.Text, this.ConfirmationPasswordField.Text, AgreeWithTerms.Checked);
        }

        private async void Model_SignedUp(Mehspot.Core.DTO.Result result)
        {
            await model.SignInAsync(this.EmailField.Text, this.PasswordField.Text);
        }

        private void Model_SignedIn(AuthenticationResult result)
        {
            this.StartActivity(new Intent(Application.Context, typeof(MessageBoardActivity)));
        }
    }
}
