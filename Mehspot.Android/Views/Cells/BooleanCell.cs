
using System;
using Android.Content;
using Android.Views;
using Android.Widget;
using Mehspot.Core.Filter.Search;

namespace Mehspot.AndroidApp
{
	public class BooleanCell : RelativeLayout
	{
		public BooleanCell(Context context, bool initialValue, Action<bool> setValue, string placeholder, bool isReadOnly = false) :
							base(context)
		{
			LayoutInflater inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
			inflater.Inflate(Resource.Layout.BooleanCell, this);

			this.Switch.Enabled = !isReadOnly;
			this.Switch.Text = placeholder;
			this.Switch.Checked = initialValue;
			this.Switch.CheckedChange += (sender, e) => setValue(((Switch)sender).Checked);
		}

		public Switch Switch => this.FindViewById<Switch>(Resource.BooleanCell.Switch);
	}

	public class ButtonCell : RelativeLayout, IButtonCell
	{
		public event Action<object> OnButtonTouched;

		public ButtonCell(Context context, string title) : base(context)
		{
			LayoutInflater inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
			//inflater.Inflate(Resource.Layout.ButtonCell, this);

		}

		public Switch Switch => this.FindViewById<Switch>(Resource.BooleanCell.Switch);
	}

	public class RecommendationCell : RelativeLayout
	{
		public event Action<object> OnButtonTouched;

		public RecommendationCell(Context context, string title) : base(context)
		{
			LayoutInflater inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
			//inflater.Inflate(Resource.Layout.RecommendationCell, this);

		}

		public Switch Switch => this.FindViewById<Switch>(Resource.BooleanCell.Switch);
	}
}
