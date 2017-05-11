
using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using mehspot.Core.Auth;
using Mehspot.Android.Wrappers;
using Mehspot.Core;
using Mehspot.Core.Models;

namespace Mehspot.Android
{
    [Activity (Label = "Sign In")]
    public class SignInActivity : Activity
    {
        SignInModel model;

        protected override void OnCreate (Bundle savedInstanceState)
        {
            base.OnCreate (savedInstanceState);
            model = new SignInModel (MehspotAppContext.Instance.AuthManager, new ActivityHelper (this));
            model.SignedIn += Model_SignedIn;


            SetContentView (Resource.Layout.SignIn);

            var signInButton = FindViewById<Button> (Resource.Id.SignIn_Button);
            signInButton.Click += SignInButton_Click;
        }

        void SignInButton_Click (object sender, EventArgs e)
        {
            SignInAsync ();
        }

        private async void SignInAsync ()
        {
            var emailField = FindViewById<EditText> (Resource.Id.EmailField);
            var passwordField = FindViewById<EditText> (Resource.Id.PasswordField);
            await model.SignInAsync (emailField.Text, passwordField.Text);
        }

        private void Model_SignedIn (AuthenticationResult result)
        {
            this.StartActivity (new Intent (Application.Context, typeof (MessageBoardActivity)));
        }
    }
}
