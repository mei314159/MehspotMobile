
using System;
using Android.Content;
using Android.Views;
using Android.Widget;
using Mehspot.Core.DTO.Badges;
using Mehspot.Core.Filter.Search;

namespace Mehspot.AndroidApp
{

	public class ButtonCell : RelativeLayout, IButtonCell
	{
		public event Action<object> OnButtonTouched;

		public ButtonCell(Context context, string title) : base(context)
		{
			LayoutInflater inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
			inflater.Inflate(Resource.Layout.ButtonCell, this);

			var button = this.FindViewById<Button>(Resource.ButtonCell.Button);
			button.Text = title;
			button.Click += (sender, e) => OnButtonTouched?.Invoke(this);
		}
	}

}
