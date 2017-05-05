using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views.InputMethods;
using Android.Widget;
using Mehspot.Android.Wrappers;
using Mehspot.Core;
using Mehspot.Core.Contracts.ViewControllers;
using Mehspot.Core.Contracts.Wrappers;
using Mehspot.Core.DTO;
using Mehspot.Core.Models;
using Mehspot.Core.Services;

namespace Mehspot.Android
{
    [Activity (Label = "Message Board")]
    public class MessageBoardActivity : Activity, IMessageBoardViewController
    {
        private MessageBoardModel model;
        public string Filter {
            get {
                return null;
            }
        }

        public IViewHelper ViewHelper { get; private set; }

        protected override void OnCreate (Bundle savedInstanceState)
        {
            base.OnCreate (savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.MessageBoard);

            this.ViewHelper = new ActivityHelper (this);
            model = new MessageBoardModel (new MessagesService (MehspotAppContext.Instance.DataStorage), this);
            model.LoadingStart += Model_LoadingStart;
            model.LoadingEnd += Model_LoadingEnd;

            //Button button = FindViewById<Button> (Resource.Id.submitButton);
            //button.Click += SubmitButtonClicked;


            //TextView toUserName = FindViewById<TextView> (Resource.Id.toUserNameField);
            //toUserName.EditorAction += ToUserName_EditorAction;
        }

        protected override void OnStart ()
        {
            base.OnStart ();

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
            int toUserId = 0;
            var messagingActivity = new Intent (Application.Context, typeof (MessagingActivity));
            messagingActivity.PutExtra ("toUserId", toUserId);
            this.StartActivity (messagingActivity);
        }

        void Model_LoadingStart ()
        {
            throw new NotImplementedException ();
        }

        void Model_LoadingEnd ()
        {
            throw new NotImplementedException ();
        }

        public void DisplayMessageBoard ()
        {
            throw new NotImplementedException ();
        }

        public void UpdateApplicationBadge (int value)
        {
            
        }

        public void UpdateMessageBoardCell (MessageBoardItemDto dto, int index)
        {
            throw new NotImplementedException ();
        }
    }
}
