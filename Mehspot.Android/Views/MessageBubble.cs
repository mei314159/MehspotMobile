using System;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using Mehspot.Core.DTO;

namespace Mehspot.AndroidApp.Resources.layout
{
    public class MessageBubble : RelativeLayout
    {
        public MessageBubble (Context context, string text, DateTime date, bool isMyMessage) :
            base (context)
        {
            LayoutInflater inflater = (LayoutInflater)Context.GetSystemService (Context.LayoutInflaterService);
            inflater.Inflate (Resource.Layout.MessageBubble, this);
            var textView = (TextView)FindViewById (Resource.Id.messageBubbleText);
			var messageDate = (TextView)FindViewById(Resource.MessageBubble.MessageDateText);
            textView.Text = text;
			messageDate.Text = date.ToLocalTime().ToString("MM/dd/yyyy hh:mm tt");


			RelativeLayout.LayoutParams parameters;
			parameters = (RelativeLayout.LayoutParams)textView.LayoutParameters;
            parameters.AddRule (isMyMessage ? LayoutRules.AlignParentRight : LayoutRules.AlignParentLeft);
            textView.LayoutParameters = parameters; //causes layout update
            textView.SetBackgroundColor (isMyMessage ? new Color (58, 155, 252) : new Color (249, 217, 128));

			parameters = (RelativeLayout.LayoutParams)messageDate.LayoutParameters;
            parameters.AddRule (isMyMessage? LayoutRules.AlignParentRight : LayoutRules.AlignParentLeft);
			messageDate.LayoutParameters = parameters;
        }
    }

    
}
