using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;

namespace Mehspot.Android.Resources.layout
{
    public class MessageBubble : RelativeLayout
    {
        public MessageBubble (Context context, string text, bool isMyMessage) :
            base (context)
        {
            LayoutInflater inflater = (LayoutInflater)Context.GetSystemService (Context.LayoutInflaterService);
            inflater.Inflate (Resource.Layout.MessageBubble, this);
            var textView = (TextView)FindViewById (Resource.Id.messageBubbleText);
            textView.Text = text;


            RelativeLayout.LayoutParams parameters = (RelativeLayout.LayoutParams)textView.LayoutParameters;
            parameters.AddRule (isMyMessage ? LayoutRules.AlignParentRight : LayoutRules.AlignParentLeft);
            textView.LayoutParameters = parameters; //causes layout update
            textView.SetBackgroundColor (isMyMessage ? new Color (58, 155, 252) : new Color (249, 217, 128));
        }
    }
}
