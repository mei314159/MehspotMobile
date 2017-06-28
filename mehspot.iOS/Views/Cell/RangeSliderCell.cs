using System;
using Foundation;
using UIKit;

namespace Mehspot.iOS.Views.Cell
{
	public partial class RangeSliderCell : UITableViewCell
	{
		public static readonly NSString Key = new NSString("RangeSliderCell");
		public static readonly UINib Nib;

		private const int SliderMaxSize = 100;
		private int minValue;
		private int maxValue;

		static RangeSliderCell()
		{
			Nib = UINib.FromName("RangeSliderCell", NSBundle.MainBundle);
		}

		protected RangeSliderCell(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public static RangeSliderCell Create<TProperty>(int? defaultMinValue, int? defaultMaxValue, Action<TProperty> setMinProperty, Action<TProperty> setMaxProperty, string placeholder, int minValue, int maxValue, bool isReadOnly = false)
		{
			var cell = (RangeSliderCell)Nib.Instantiate(null, null)[0];
			var slider = cell.RangeSlider;
			cell.minValue = minValue;
			cell.maxValue = maxValue;
			slider.Enabled = !isReadOnly;
			cell.FieldLabel.Text = placeholder;
			slider.LowerValue = cell.ValueToProgress(defaultMinValue ?? cell.minValue);
			slider.UpperValue = cell.ValueToProgress(defaultMaxValue ?? cell.maxValue);

			var min = cell.ProgressToValue(slider.LowerValue);
			var max = cell.ProgressToValue(slider.UpperValue);

			slider.MinimumValue = cell.minValue;
			slider.MaximumValue = SliderMaxSize;
			cell.SetValueLabel(min.ToString(), max.ToString(), cell);

			slider.LowerValueChanged += (sender, e) =>
			{
				var value = slider.LowerValue / 100 * maxValue;
				var propertyType = Nullable.GetUnderlyingType(typeof(TProperty)) ?? typeof(TProperty);
				var val = (TProperty)Convert.ChangeType(value, propertyType);
				var clearValue = value == cell.minValue;
				if (clearValue)
				{
					val = default(TProperty);
				}

				cell.SetValueLabel(clearValue ? cell.minValue.ToString() : val?.ToString(), cell.MaxValueLabel.Text, cell);
				setMinProperty(val);
			};

			slider.UpperValueChanged += (sender, e) =>
			{
				var value = slider.UpperValue / 100 * maxValue;
				var propertyType = Nullable.GetUnderlyingType(typeof(TProperty)) ?? typeof(TProperty);
				var val = (TProperty)Convert.ChangeType(value, propertyType);
				var clearValue = value == cell.maxValue;
				if (clearValue)
				{
					val = default(TProperty);
				}

				cell.SetValueLabel(cell.MinValueLabel.Text, clearValue ? cell.maxValue.ToString() : val?.ToString(), cell);
				setMaxProperty(val);
			};

			return cell;
		}

		private int ValueToProgress(float value)
		{
			return (int)((value - minValue) / (float)(maxValue - minValue) * 100);
		}

		private int ProgressToValue(float progress)
		{
			return (int)(minValue + (maxValue - minValue) * progress / 100);
		}

		private void SetValueLabel(string minVal, string maxVal, RangeSliderCell cell)
		{
			cell.MinValueLabel.Text = minVal;
			cell.MaxValueLabel.Text = maxVal;
		}
	}
}
