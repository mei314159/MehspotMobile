
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using mehspot.Android.Core;
using mehspot.Core.Auth;
using Mehspot.Android.Wrappers;
using Mehspot.Core.Models;

namespace Mehspot.Android
{
    [Activity (Label = "Messaging")]
    public class MessagingActivity : Activity
    {
        private string toUserName;

        protected override void OnCreate (Bundle savedInstanceState)
        {
            base.OnCreate (savedInstanceState);
            toUserName = Intent.GetStringExtra ("toUserName");

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Messaging);

            Button button = FindViewById<Button> (Resource.Id.sendMessageButton);
            button.Click += SendMessageButtonClicked;

        }

        void SendMessageButtonClicked (object sender, EventArgs e)
        {
            throw new NotImplementedException ();
        }
   }
}
