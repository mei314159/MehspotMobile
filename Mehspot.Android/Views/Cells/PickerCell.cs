using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Views;
using Android.Widget;

namespace Mehspot.AndroidApp
{

	public class PickerCell<T> : RelativeLayout
	{
		private string placeholder;

		private T Value;
		private Action<T> SetProperty;
		private Func<T, string> GetPropertyString;

		public PickerCell(Context context, T initialValue,
			Action<T> setProperty,
			string label,
			KeyValuePair<T, string>[] rowValues,
		  	string placeholder = null,
			bool isReadOnly = false) : base(context)
		{
			LayoutInflater inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
			inflater.Inflate(Resource.Layout.PickerCell, this);

			this.placeholder = placeholder ?? label;
			this.IsReadOnly = isReadOnly;
			this.FieldLabel.Text = label;
			this.Value = initialValue;

			this.RowValues = rowValues ?? new KeyValuePair<T, string>[] { };
			this.Dropdown.Adapter = new ArrayAdapter(context, Resource.Layout.TextViewItem, this.RowValues.Select(a => a.Value).ToArray());
			this.Dropdown.ItemSelected += Dropdown_ItemSelected;
			this.Dropdown.SetSelection(this.RowValues.Select((a, i) => new { a, i }).FirstOrDefault(a => Equals(a.a.Key, initialValue))?.i ?? 0);

			this.GetPropertyString = (value) => this.RowValues?.FirstOrDefault(a => Equals(a.Key, value)).Value;
			this.SetProperty = setProperty;
		}

		public TextView FieldLabel => this.FindViewById<TextView>(Resource.PickerCell.FieldLabel);

		public Spinner Dropdown => this.FindViewById<Spinner>(Resource.PickerCell.Dropdown);

		public string FieldName { get; set; }

		public KeyValuePair<T, string>[] RowValues { get; set; }

		public bool IsReadOnly
		{
			get
			{
				return this.Dropdown.Enabled;
			}
			set
			{
				this.Dropdown.Enabled = !value;
			}
		}

		void Dropdown_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
		{
			var p = RowValues[e.Position];
			this.Value = p.Key;
			this.SetProperty(p.Key);
		}
	}
}
