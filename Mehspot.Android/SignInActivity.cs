
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using mehspot.Android.Core;
using mehspot.Core.Auth;
using Mehspot.Android.Wrappers;
using Mehspot.Core.Models;

namespace Mehspot.Android
{
    [Activity (Label = "Sign In")]
    public class SignInActivity : Activity
    {
        SignInModel model;

        protected override void OnCreate (Bundle savedInstanceState)
        {
            var authManager = new AuthenticationManager (new ApplicationDataStorage ());
            base.OnCreate (savedInstanceState);
            model = new SignInModel (authManager, new ActivityHelper (Application.Context));
            model.SignedIn += Model_SignedIn;


            SetContentView (Resource.Layout.SignIn);

            Button signInButton = FindViewById<Button> (Resource.Id.SignInButton);
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
            base.StartActivity (new Intent (Application.Context, typeof (MainActivity)));
        }
    }
}
