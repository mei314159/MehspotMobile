using System;
using Foundation;
using UIKit;

namespace Mehspot.iOS.Views.Cell
{
	public partial class SliderCell : UITableViewCell
	{
		public static readonly NSString Key = new NSString("SliderCell");
		public static readonly UINib Nib;

		private int? minValue;
		private int? maxValue;

		static SliderCell()
		{
			Nib = UINib.FromName("SliderCell", NSBundle.MainBundle);
		}

		protected SliderCell(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public static SliderCell Create<TProperty>(int? defaultValue, Action<TProperty> setProperty, string placeholder, int? minValue, int? maxValue, bool isReadOnly = false)
		{
			var cell = (SliderCell)Nib.Instantiate(null, null)[0];
			cell.CellSlider.Enabled = !isReadOnly;
			cell.FieldLabel.Text = placeholder;
			cell.minValue = minValue;
			cell.maxValue = maxValue;
			if (minValue.HasValue)
			{
				cell.CellSlider.MinValue = minValue.Value - 1;
			}

			cell.CellSlider.MinValue = 0;
			cell.CellSlider.MaxValue = 100;

			cell.CellSlider.Value = defaultValue ?? cell.CellSlider.MinValue;
			cell.SetValueLabel(cell.CellSlider.Value == cell.CellSlider.MinValue ? string.Empty : cell.CellSlider.Value.ToString());

			cell.CellSlider.ValueChanged += (sender, e) =>
			{
				SetValue(setProperty, cell);
			};

			SetValue(setProperty, cell);
			return cell;
		}

		static void SetValue<TProperty>(Action<TProperty> setProperty, SliderCell cell)
		{
			var slider = cell.CellSlider;

			var value = cell.ProgressToValue(slider.Value);
			var propertyType = Nullable.GetUnderlyingType(typeof(TProperty)) ?? typeof(TProperty);
			var val = (TProperty)Convert.ChangeType(value, propertyType);
			var clearValue = value == cell.minValue;
			if (clearValue)
			{
				val = default(TProperty);
			}

			cell.SetValueLabel(clearValue ? string.Empty : val?.ToString());
			setProperty(val);
		}

		private void SetValueLabel(string value)
		{
			this.ValueLabel.Text = value;
		}

		private int ProgressToValue(float progress)
		{
			return (int)(minValue + (maxValue - minValue) * progress / 100);
		}
	}
}
