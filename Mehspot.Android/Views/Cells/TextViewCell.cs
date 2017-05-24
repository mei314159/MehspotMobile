using System;
using System.Linq;
using System.Text.RegularExpressions;
using Android.Content;
using Android.Views;
using Android.Widget;

namespace Mehspot.AndroidApp
{
	public class TextViewCell : RelativeLayout
	{
		public TextViewCell(Context context, string text, string label) : base(context)
		{
			LayoutInflater inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
			inflater.Inflate(Resource.Layout.TextViewCell, this);

			this.FieldLabel.Text = label;
			this.Text.Text = text;
		}

		public TextView FieldLabel => this.FindViewById<TextView>(Resource.TextViewCell.FieldLabel);

		public TextView Text => this.FindViewById<TextView>(Resource.TextViewCell.Text);
	}
}
