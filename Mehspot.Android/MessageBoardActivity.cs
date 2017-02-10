using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views.InputMethods;
using Android.Widget;

namespace Mehspot.Android
{
    [Activity (Label = "Message Board")]
    public class MessageBoardActivity : Activity
    {
        protected override void OnCreate (Bundle savedInstanceState)
        {
            base.OnCreate (savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.MessageBoard);

            Button button = FindViewById<Button> (Resource.Id.submitButton);
            button.Click += SubmitButtonClicked;


            TextView toUserName = FindViewById<TextView> (Resource.Id.toUserNameField);
            toUserName.EditorAction += ToUserName_EditorAction;
        }

        void SubmitButtonClicked (object sender, EventArgs e)
        {
            GoToMessaging ();
        }

        void ToUserName_EditorAction (object sender, TextView.EditorActionEventArgs e)
        {
            if (e.ActionId == ImeAction.Done) {
                GoToMessaging ();
            } else {
                e.Handled = false;
            }
        }

        void GoToMessaging ()
        {
            TextView toUserName = FindViewById<TextView> (Resource.Id.toUserNameField);
            var messagingActivity = new Intent (Application.Context, typeof (MessagingActivity));
            messagingActivity.PutExtra ("toUserName", toUserName.Text);
            this.StartActivity (messagingActivity);
        }
    }
}
