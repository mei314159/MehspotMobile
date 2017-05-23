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

        public float MinValue { get; set; }

        public float MaxValue { get; set; }

        public string OptionsKey { get; set; }

        public bool SkipFirstOption { get; set; }

        public object DefaultValue { get; set; }

        public int Order { get; set; }

        public bool ReadOnly { get; set; }


        public CellType CellType { get; set; }
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