using System;
using Android.Content;
using Android.Views;
using Android.Widget;

namespace Mehspot.AndroidApp
{

	public class SliderCell<TProperty> : RelativeLayout
	{
		public SeekBar CellSlider => this.FindViewById<SeekBar>(Resource.SliderCell.Slider);
		public TextView FieldLabel => this.FindViewById<TextView>(Resource.SliderCell.FieldLabel);
		public TextView ValueLabel => this.FindViewById<TextView>(Resource.SliderCell.ValueLabel);

		int minValue;
		int maxValue;

		public SliderCell(Context context, int? defaultValue, Action<TProperty> setProperty, string placeholder, int minValue, int maxValue, bool isReadOnly = false) :
								base(context)
		{
			this.minValue = minValue;
			this.maxValue = maxValue;
			LayoutInflater inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
			inflater.Inflate(Resource.Layout.SliderCell, this);
			this.CellSlider.Enabled = !isReadOnly;
			this.FieldLabel.Text = placeholder;

			this.CellSlider.Progress = ValueToProgress(defaultValue ?? this.minValue);
			var value = ProgressToValue(this.CellSlider.Progress);
			this.SetValueLabel(value == this.minValue ? string.Empty : value.ToString());

			this.CellSlider.ProgressChanged += (sender, e) => SetValue(setProperty);
			this.SetValue(setProperty);
		}

		private int ValueToProgress(float value)
		{
			return (int)((value - minValue) / (float)(maxValue - minValue) * 100);
		}

		private int ProgressToValue(float progress)
		{
			return (int)(minValue + (maxValue - minValue) * progress / 100);
		}

		void SetValue(Action<TProperty> setProperty)
		{
			var value = ProgressToValue(this.CellSlider.Progress);
			var propertyType = Nullable.GetUnderlyingType(typeof(TProperty)) ?? typeof(TProperty);
			var val = (TProperty)Convert.ChangeType(value, propertyType);
			var clearValue = value == this.minValue;
			if (clearValue)
			{
				val = default(TProperty);
			}

			this.SetValueLabel(clearValue ? string.Empty : val?.ToString());
			setProperty(val);
		}

		private void SetValueLabel(string value)
		{
			this.ValueLabel.Text = value;
		}
	}
}
