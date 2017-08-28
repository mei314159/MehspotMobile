using System;
using System.Collections.Generic;
using Mehspot.Core.DTO.Badges;
using Mehspot.Core.Builders;
using Mehspot.Core.DTO.Subdivision;

namespace Mehspot.Core.Builders
{
    public abstract class CellBuilder<TCell>
    {
        public abstract IBooleanEditCell GetBooleanCell(bool initialValue, Action<bool> setValue, string placeholder, bool isReadOnly = false);
        public abstract TCell GetTextViewCell(string text, string label);
        public abstract ITextEditCell GetTextEditCell(string initialValue, Action<ITextEditCell, string> setProperty, string label, KeyboardType type = KeyboardType.Default, string placeholder = null, bool isReadOnly = false, string mask = null, string validationRegex = null);
        public abstract TCell GetMultilineTextEditCell(string initialValue, Action<string> setValue, string label, bool isReadOnly = false);
        public abstract TCell GetRangeCell<T>(int? defaultValue, Action<T> setProperty, string placeholder, int minValue, int maxValue, bool isReadOnly = false);
        public abstract TCell GetMaxMinRangeCell<T>(int? defaultMinValue, int? defaultMaxValue, Action<T> setMinProperty, Action<T> setMaxProperty, string placeholder, int minValue, int maxValue, bool isReadOnly = false);
        public abstract TCell GetPickerCell<T>(T initialValue, Action<T> setProperty, string label, KeyValuePair<T, string>[] rowValues, string placeholder = null, bool isReadOnly = false);
        public abstract ISubdivisionPickerCell GetSubdivisionPickerCell(int? selectedId, int? optionId, Action<SubdivisionDTO> setProperty, string label, List<SubdivisionDTO> list, string zipCode, bool isReadOnly = false);
        public abstract TCell GetMultiselectCell<T>(IEnumerable<T> initialValue, Action<IEnumerable<T>> setProperty, string label, KeyValuePair<T, string>[] rowValues, bool isReadOnly = false);
        public abstract TCell GetDatePickerCell<T>(T initialValue, Action<T> setProperty, string label, bool isReadOnly = false);
        public abstract IButtonCell GetButtonCell(string title);
        public abstract TCell GetRecommendationCell(BadgeUserRecommendationDTO item);
    }
}
