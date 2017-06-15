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

	public class DateTimePickerCell<T> : RelativeLayout,
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
		public Button ChooseDate => this.FindViewById<Button>(Resource.DateViewCell.ChooseDate);
		public TextView FieldLabel => this.FindViewById<TextView>(Resource.DateViewCell.FieldLabel);

		public bool IsReadOnly
		{
			get
			{
				return !ChooseDate.Enabled;
			}
			set
			{
				ChooseDate.Enabled = !value;
			}
		}


		public DateTimePickerCell(IntPtr a, JniHandleOwnership b) : base(a, b) { }

		public DateTimePickerCell(Context context, T initialValue, Action<T> setProperty, string label, bool isReadOnly = false) : base(context)
		{
			LayoutInflater inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
			inflater.Inflate(Resource.Layout.DateViewCell, this);
			Value = initialValue is DateTime ? (DateTime)(object)initialValue : initialValue is DateTime? ? (DateTime?)(object)initialValue ?? DateTime.Now : DateTime.Now;

			FieldLabel.Text = label;
			ChooseDate.Text = GetDateString(Value);
			this.IsReadOnly = isReadOnly;
			ChooseDate.Click += (sender, e) =>
			{
				AlertDialog.Builder builder = new AlertDialog.Builder(Context);
				var datePicker = new DatePicker(Context);
				datePicker.Init(Value.Year, Value.Month, Value.Day, this);
				builder.SetView(datePicker);
				builder.SetPositiveButton(Android.Resource.String.Ok, (s, ev) =>
						{
							var typedValue = typeof(T) == typeof(string) ? (T)(object)Value.ToString() : (T)(object)Value;
							setProperty(typedValue);
							(s as AlertDialog)?.Cancel();
						});

				builder.SetOnCancelListener(this);
				builder.Show();
			};
		}

		string GetDateString(DateTime value)
		{
			return value.Day.ToString() + "/" + value.Month.ToString() + "/" + value.Year.ToString();
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
			Value = new DateTime(year, monthOfYear + 1, dayOfMonth);
			ChooseDate.Text = GetDateString(Value);
		}

		public void OnCancel(IDialogInterface dialog)
		{
			//if (DateChanged != null)
			//	DateChanged(this, new DatePickerSelectedEventArgs(Value));
			//ChooseDate.Text = GetDateString(Value);
		}
	}
}
