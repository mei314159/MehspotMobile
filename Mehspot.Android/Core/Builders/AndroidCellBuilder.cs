using System;
using System.Collections.Generic;
using Android.App;
using Android.Views;
using Mehspot.Core.Builders;
using Mehspot.Core.DTO.Badges;
using Mehspot.Core.DTO.Subdivision;

namespace Mehspot.AndroidApp.Core.Builders
{
	public class AndroidCellBuilder : CellBuilder<View>
	{
		private readonly Activity activity;
		public AndroidCellBuilder(Activity activity)
		{
			this.activity = activity;
		}

		public override IBooleanEditCell GetBooleanCell(bool initialValue, Action<bool> setValue, string placeholder, bool isReadOnly = false)
		{
			return new BooleanCell(activity, initialValue as bool? == true, setValue, placeholder, isReadOnly);
		}

		public override IButtonCell GetButtonCell(string title)
		{
			return new ButtonCell(activity, title);
		}

		public override View GetDatePickerCell<T>(T initialValue, Action<T> setProperty, string label, bool isReadOnly = false)
		{
			var cell = new DateTimePickerCell<T>(activity, initialValue, setProperty, label, isReadOnly);
			return cell;
		}

		public override View GetMultilineTextEditCell(string initialValue, Action<string> setValue, string label, bool isReadOnly = false)
		{
			var cell = new TextEditCell(activity, initialValue, (arg1, arg2) => setValue(arg2), label, isReadOnly: isReadOnly);
			cell.Multiline = true;
			return cell;
		}

		public override View GetMultiselectCell<T>(IEnumerable<T> initialValue, Action<IEnumerable<T>> setProperty, string label, KeyValuePair<T, string>[] rowValues, bool isReadOnly = false)
		{
			return new MultiselectCell<T>(activity, initialValue, setProperty, label, rowValues, isReadOnly);
		}

		public override View GetPickerCell<T>(T initialValue, Action<T> setProperty, string label, KeyValuePair<T, string>[] rowValues, string placeholder = null, bool isReadOnly = false)
		{
			return new PickerCell<T>(activity, initialValue, setProperty, label, rowValues, placeholder, isReadOnly);
		}

		public override View GetRangeCell<T>(int? defaultValue, Action<T> setProperty, string placeholder, int minValue, int maxValue, bool isReadOnly = false)
		{
			return new SliderCell<T>(activity, defaultValue, setProperty, placeholder, minValue, maxValue, isReadOnly);
		}

		public override View GetMaxMinRangeCell<T>(int? defaultMinValue, int? defaultMaxValue, Action<T> setMinProperty, Action<T> setMaxProperty, string placeholder, int minValue, int maxValue, bool isReadOnly = false)
		{
			return new RangeSliderCell<T>(activity, defaultMinValue, defaultMaxValue, setMinProperty, setMaxProperty, placeholder, minValue, maxValue, isReadOnly);
		}

		public override View GetRecommendationCell(BadgeUserRecommendationDTO item)
		{
			return new RecommendationCell(activity, item);
		}

		public override ISubdivisionPickerCell GetSubdivisionPickerCell(int? selectedId, int? optionId, Action<SubdivisionDTO> setProperty, string label, List<SubdivisionDTO> list, string zipCode, bool isReadOnly = false)
		{
            return new SubdivisionPickerCell(activity, selectedId, optionId, setProperty, label, list, zipCode, isReadOnly);
		}

		public override ITextEditCell GetTextEditCell(string initialValue, Action<ITextEditCell, string> setProperty, string label, Mehspot.Core.Builders.KeyboardType type = Mehspot.Core.Builders.KeyboardType.Default, string placeholder = null, bool isReadOnly = false, string mask = null, string validationRegex = null)
		{
			return new TextEditCell(activity, initialValue, setProperty, label, type, placeholder, isReadOnly, mask, validationRegex);
		}

		public override View GetTextViewCell(string text, string label)
		{
			return new TextViewCell(activity, text, label);
		}
	}
}
