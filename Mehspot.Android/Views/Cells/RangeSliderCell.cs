using System;
using Android.Content;
using Android.Views;
using Android.Widget;
using Xamarin.RangeSlider;

namespace Mehspot.AndroidApp
{
	public class RangeSliderCell<TProperty> : RelativeLayout
	{
		public RangeSliderControl CellSlider => this.FindViewById<RangeSliderControl>(Resource.RangeSliderCell.RangeSlider);
		public TextView FieldLabel => this.FindViewById<TextView>(Resource.RangeSliderCell.FieldLabel);
		public TextView MinValueLabel => this.FindViewById<TextView>(Resource.RangeSliderCell.MinValueLabel);
		public TextView MaxValueLabel => this.FindViewById<TextView>(Resource.RangeSliderCell.MaxValueLabel);

		readonly int minValue;
		readonly int maxValue;

		public RangeSliderCell(Context context, int? defaultMinValue, int? defaultMaxValue, Action<TProperty> setMinProperty, Action<TProperty> setMaxProperty, string placeholder, int minValue, int maxValue, bool isReadOnly = false) :
									base(context)
		{
			this.minValue = minValue;
			this.maxValue = maxValue;
			LayoutInflater inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
			inflater.Inflate(Resource.Layout.RangeSliderCell, this);
			this.CellSlider.Enabled = !isReadOnly; 
			this.FieldLabel.Text = placeholder;
			this.CellSlider.SetSelectedMinValue(ValueToProgress(defaultMinValue ?? this.minValue));
			this.CellSlider.SetSelectedMaxValue(ValueToProgress(defaultMaxValue ?? this.maxValue));

			var min = ProgressToValue(this.CellSlider.GetSelectedMinValue());
			var max = ProgressToValue(this.CellSlider.GetSelectedMaxValue());

			this.SetValueLabel(min.ToString(), max.ToString());
			this.CellSlider.LowerValueChanged += (sender, e) =>
			{
				var value = (((RangeSliderControl)sender).GetSelectedMinValue()) / 100 * maxValue;
				var propertyType = Nullable.GetUnderlyingType(typeof(TProperty)) ?? typeof(TProperty);
				var val = (TProperty)Convert.ChangeType(value, propertyType);
				var clearValue = value == this.minValue;
				if (clearValue)
				{
					val = default(TProperty);
				}

				this.SetValueLabel(clearValue ? this.minValue.ToString() : val?.ToString(), this.MaxValueLabel.Text);
				setMinProperty(val);
			};

			this.CellSlider.UpperValueChanged += (sender, e) =>
			{
				var value = (((RangeSliderControl)sender).GetSelectedMaxValue()) / 100 * maxValue;
				var propertyType = Nullable.GetUnderlyingType(typeof(TProperty)) ?? typeof(TProperty);
				var val = (TProperty)Convert.ChangeType(value, propertyType);
				var clearValue = value == this.maxValue;
				if (clearValue)
				{
					val = default(TProperty);
				}

				this.SetValueLabel(this.MinValueLabel.Text, clearValue ? this.maxValue.ToString() : val?.ToString());
				setMaxProperty(val);
			};
		}

		private int ValueToProgress(float value)
		{
			return (int)((value - minValue) / (float)(maxValue - minValue) * 100);
		}

		private int ProgressToValue(float progress)
		{
			return (int)(minValue + (maxValue - minValue) * progress / 100);
		}

		private void SetValueLabel(string minVal, string maxVal)
		{
			this.MinValueLabel.Text = minVal;
			this.MaxValueLabel.Text = maxVal;
		}
	}
}
