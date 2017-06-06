using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Mehspot.AndroidApp
{

	public class DateTimePickerCell : Button,
	IDialogInterfaceOnCancelListener,
	DatePicker.IOnDateChangedListener
	{
		public class DatePickerSelectedEventArgs : EventArgs
		{
			public DatePickerSelectedEventArgs(DateTime selected)
			{
				Value = selected;
			}

			public DateTime Value { get; private set; }
		}

		public delegate void DateChangedHandler(object sender, DatePickerSelectedEventArgs args);
		public event DateChangedHandler DateChanged;
		public DateTime Value = DateTime.Now;

		public DateTimePickerCell(IntPtr a, JniHandleOwnership b) : base(a, b) { }

		public DateTimePickerCell(Context context) : base(context)
		{
		}

		public DateTimePickerCell(Context context, IAttributeSet attrs)
				: base(context, attrs)
		{
		}

		public DateTimePickerCell(Context context, IAttributeSet attrs, int defStyle)
				: base(context, attrs, defStyle)
		{
		}

		public void OnDateChanged(DatePicker view, int year, int monthOfYear, int dayOfMonth)
		{
			Value = new DateTime(year, monthOfYear, dayOfMonth);
		}

		public void OnCancel(IDialogInterface dialog)
		{
			if (DateChanged != null)
				DateChanged(this, new DatePickerSelectedEventArgs(Value));
		}

		public override bool PerformClick()
		{
			AlertDialog.Builder builder = new AlertDialog.Builder(Context);
			var datePicker = new DatePicker(Context);
			datePicker.Init(Value.Year, Value.Month, Value.Day, this);
			builder.SetView(datePicker);
			builder.SetPositiveButton(Android.Resource.String.Ok,
				delegate (object o, DialogClickEventArgs e)
				{
					(o as AlertDialog).Cancel();
				});
			builder.SetOnCancelListener(this);
			builder.Show();
			return true;
		}
	}
}
