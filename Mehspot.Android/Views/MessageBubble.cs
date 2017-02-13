
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
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
            this.LayoutParameters = new RelativeLayout.LayoutParams (ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent) {
                TopMargin = ConvertDpsToPixels(20)
            };
            this.SetBackgroundColor (isMyMessage ? new Color (58, 155, 252) : new Color (249, 217, 128));
        }

        private int ConvertPixelsToDp (float pixelValue)
        {
            var dp = (int)((pixelValue) / Context.Resources.DisplayMetrics.Density);
            return dp;
        }

        private int ConvertDpsToPixels (float dpValue)
        {
            return (int)(Context.Resources.DisplayMetrics.Density * dpValue);
        }
    }
}
