using System;

namespace Mehspot.Core.DTO.Search
{

    public class CellAttribute : Attribute
    {
        public CellAttribute()
        {
        }

        public CellAttribute(string label, int order, CellType cellType)
        {
            Label = label;
            Order = order;
            CellType = cellType;
        }

        public string Label { get; set; }

        public string Mask { get; set; }

        public int MinValue { get; set; }

        public int MaxValue { get; set; }

        public string OptionsKey { get; set; }

        public bool SkipFirstOption { get; set; }

        public object DefaultValue { get; set; }

        public int Order { get; set; }

        public bool ReadOnly { get; set; }

        public CellType CellType { get; set; }
    }

    public class ViewProfileDtoAttribute : Attribute
    {
        public ViewProfileDtoAttribute(string badgeName)
        {
            BadgeName = badgeName;
        }

        public string BadgeName { get; set; }
    }

    public class SearchResultDtoAttribute : Attribute
    {
        public SearchResultDtoAttribute(string badgeName)
        {
            BadgeName = badgeName;
        }

        public string BadgeName { get; set; }
    }

    public class SearchFilterDtoAttribute : Attribute
    {
        public SearchFilterDtoAttribute(string badgeName)
        {
            BadgeName = badgeName;
        }

        public string BadgeName { get; set; }
    }

    public enum CellType
    {
        TextEdit,
        TextView,
        Boolean,
        Range,
        Select,
        Multiselect,
        Complex
    }
}