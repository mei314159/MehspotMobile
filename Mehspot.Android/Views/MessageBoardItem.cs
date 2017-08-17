using System;
using System.Net;
using System.Threading.Tasks;
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

		public MessageBoardItem(Activity activity, MessageBoardItemDto dto) : base(activity)
		{
			this.dto = dto;
			LayoutInflater inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
			inflater.Inflate(Resource.Layout.MessageBoardItem, this);
			var userNameView = (TextView)FindViewById(Resource.Id.userName);
			userNameView.Text = dto.WithUser.UserName;

			Message.Text = dto.LastMessage;
			UnreadMessagesCount.Text = dto.UnreadMessagesCount.ToString();
			if (dto.UnreadMessagesCount == 0)
			{
				UnreadMessagesCount.Visibility = ViewStates.Invisible;
			}
			ProfilePicture.ClipToOutline = true;
			Task.Run(() =>
			{
				if (!string.IsNullOrWhiteSpace(dto.WithUser.ProfilePicturePath))
				{
					var imageBitmap = activity.GetImageBitmapFromUrl(dto.WithUser.ProfilePicturePath);
                    activity.RunOnUiThread(() =>
                    {
                        try
                        {
							ProfilePicture?.SetImageBitmap(imageBitmap);
                        }
                        catch (Exception ex)
                        {
                        }
                    });
				}
			});

			this.Click += Handle_Click;
		}

		public TextView Message
		{
			get
			{
				return (TextView)FindViewById(Resource.Id.message);
			}
		}

		public TextView UnreadMessagesCount
		{
			get
			{
				return (TextView)FindViewById(Resource.Id.unreadMessagesCount);
			}
		}

		public ImageView ProfilePicture
		{
			get
			{
				return FindViewById(Resource.Id.profilePicture) as ImageView;
			}
		}

		void Handle_Click(object sender, EventArgs e)
		{
			this.Clicked?.Invoke(this.dto);
		}
	}
}
