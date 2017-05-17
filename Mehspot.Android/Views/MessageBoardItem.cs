using System;
using System.Net;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using Mehspot.Core.DTO;

namespace Mehspot.AndroidApp.Resources.layout
{

    public class MessageBoardItem : RelativeLayout
    {
        readonly MessageBoardItemDto dto;

        public event Action<MessageBoardItemDto> Clicked;

        public MessageBoardItem (Context context, MessageBoardItemDto dto) : base (context)
        {
            this.dto = dto;
            LayoutInflater inflater = (LayoutInflater)Context.GetSystemService (Context.LayoutInflaterService);
            inflater.Inflate (Resource.Layout.MessageBoardItem, this);
            var userNameView = (TextView)FindViewById (Resource.Id.userName);
            userNameView.Text = dto.WithUser.UserName;

            Message.Text = dto.LastMessage;
            UnreadMessagesCount.Text = dto.UnreadMessagesCount.ToString ();
            if (dto.UnreadMessagesCount == 0) {
                UnreadMessagesCount.Visibility = ViewStates.Invisible;
            }

            if (!string.IsNullOrWhiteSpace (dto.WithUser.ProfilePicturePath)) {
                var imageBitmap = GetImageBitmapFromUrl (dto.WithUser.ProfilePicturePath);
                ProfilePicture.SetImageBitmap (imageBitmap);
            }
            this.Click += Handle_Click;
        }

        public TextView Message {
            get {
                return (TextView)FindViewById (Resource.Id.message);
            }
        }

        public TextView UnreadMessagesCount {
            get {
                return (TextView)FindViewById (Resource.Id.unreadMessagesCount);
            }
        }

        public ImageView ProfilePicture {
            get {
                return (ImageView)FindViewById (Resource.Id.profilePicture);
            }
        }

        void Handle_Click (object sender, EventArgs e)
        {
            this.Clicked?.Invoke(this.dto);
        }

        private Bitmap GetImageBitmapFromUrl (string url)
        {
            Bitmap imageBitmap = null;

            using (var webClient = new WebClient ()) {
                var imageBytes = webClient.DownloadData (url);
                if (imageBytes != null && imageBytes.Length > 0) {
                    imageBitmap = BitmapFactory.DecodeByteArray (imageBytes, 0, imageBytes.Length);
                }
            }

            return imageBitmap;
        }
    }
}
