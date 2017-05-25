using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Views;
using Android.Widget;

namespace Mehspot.AndroidApp
{

	public class MultiselectCell<T> : RelativeLayout
	{
		private string placeholder;

		private IEnumerable<T> Value;
		private Action<IEnumerable<T>> SetProperty;
		private Func<T, string> GetPropertyString;

		public MultiselectCell(Context context, IEnumerable<T> initialValue,
			Action<IEnumerable<T>> setProperty,
			string label,
			KeyValuePair<T, string>[] rowValues,
					bool isReadOnly = false) : base(context)
		{
			LayoutInflater inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
			inflater.Inflate(Resource.Layout.MultiselectCell, this);

			this.placeholder = placeholder ?? label;
			this.IsReadOnly = isReadOnly;
			this.FieldLabel.Text = label;
			this.Value = initialValue;

			this.RowValues = rowValues;
			this.Dropdown.Adapter = new ArrayAdapter(context, Resource.Layout.TextViewItem, this.RowValues.Select(a => a.Value).ToArray());
			this.Dropdown.ItemsSelected += Dropdown_ItemsSelected;;
			this.Dropdown.SetSelection(this.RowValues.Select((a, i) => new { a, i }).FirstOrDefault(a => Equals(a.a.Key, initialValue))?.i ?? 0);

			this.GetPropertyString = (value) => this.RowValues?.FirstOrDefault(a => Equals(a.Key, value)).Value;
			this.SetProperty = setProperty;
		}

		public TextView FieldLabel => this.FindViewById<TextView>(Resource.MultiselectCell.FieldLabel);

		public MultiSpinner Dropdown => this.FindViewById<MultiSpinner>(Resource.MultiselectCell.Dropdown);

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

		void Dropdown_ItemsSelected(object sender, MultiSpinner.MultiSpinnerSelectionEventArgs e)
		{
			var p = RowValues.Where((a, i) => e.Selected[i]).Select(a => a.Key).ToList();
			this.Value = p;
			this.SetProperty(p);
		}
	}
}
