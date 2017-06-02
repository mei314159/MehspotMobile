using System;
using System.Collections.Generic;
using Mehspot.Core.Builders;
using Mehspot.Core.DTO.Badges;
using Mehspot.Core.DTO.Subdivision;
using Mehspot.iOS.Views;
using Mehspot.iOS.Views.Cell;
using UIKit;

namespace Mehspot.iOS.Core.Builders
{
	public class IosCellBuilder : CellBuilder<UITableViewCell>
	{
		public override IBooleanEditCell GetBooleanCell(bool initialValue, Action<bool> setValue, string placeholder, bool isReadOnly = false)
		{
			return BooleanEditCell.Create(initialValue as bool? == true, setValue, placeholder, isReadOnly);
		}

		public override IButtonCell GetButtonCell(string title)
		{
			return ButtonCell.Create(title);
		}

		public override UITableViewCell GetDatePickerCell<T>(T initialValue, Action<T> setProperty, string label, bool isReadOnly = false)
		{
			return PickerCell.CreateDatePicker(initialValue, setProperty, label, isReadOnly);
		}

		public override UITableViewCell GetMultilineTextEditCell(string initialValue, Action<string> setValue, string label, bool isReadOnly = false)
		{
			return MultilineTextEditCell.Create(initialValue, setValue, label, isReadOnly);
		}

		public override UITableViewCell GetMultiselectCell<T>(IEnumerable<T> initialValue, Action<IEnumerable<T>> setProperty, string label, KeyValuePair<T, string>[] rowValues, bool isReadOnly = false)
		{
			return PickerCell.CreateMultiselect(initialValue, setProperty, label, rowValues, isReadOnly);
		}

		public override UITableViewCell GetPickerCell<T>(T initialValue, Action<T> setProperty, string label, KeyValuePair<T, string>[] rowValues, string placeholder = null, bool isReadOnly = false)
		{
			return PickerCell.Create(initialValue, setProperty, label, rowValues, placeholder, isReadOnly);
		}

		public override UITableViewCell GetRangeCell<T>(int? defaultValue, Action<T> setProperty, string placeholder, float? minValue, float? maxValue, bool isReadOnly = false)
		{
			return SliderCell.Create(defaultValue, setProperty, placeholder, minValue, maxValue, isReadOnly);
		}

		public override UITableViewCell GetRecommendationCell(BadgeUserRecommendationDTO item)
		{
			return RecommendationCell.Create(item);
		}

		public override ISubdivisionPickerCell GetSubdivisionPickerCell(int? selectedId, Action<SubdivisionDTO> setProperty, string label, List<SubdivisionDTO> list, string zipCode, bool isReadOnly = false)
		{
			return SubdivisionPickerCell.Create(selectedId, setProperty, label, list, zipCode, isReadOnly);
		}

		public override ITextEditCell GetTextEditCell(string initialValue, Action<ITextEditCell, string> setProperty, string label, string placeholder = null, bool isReadOnly = false, string mask = null, string validationRegex = null)
		{
			return TextEditCell.Create(initialValue, setProperty, label, placeholder, isReadOnly, mask, validationRegex);
		}

		public override UITableViewCell GetTextViewCell(string text, string label)
		{
			return TextViewCell.Create(text, label);
		}
	}
}
