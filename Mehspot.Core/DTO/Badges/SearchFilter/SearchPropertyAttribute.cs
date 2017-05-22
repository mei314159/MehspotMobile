using System;

namespace Mehspot.Core.DTO.Search
{

    public class SearchPropertyAttribute : Attribute
    {
        public string Label { get; set; }

        public string Mask { get; set; }

        public float MinValue { get; set; }

        public float MaxValue { get; set; }

        public string OptionsKey { get; set; }

        public object DefaultValue { get; set; }

        public int Order { get; set; }

        public CellType CellType { get; set; }
    }

    public enum CellType
    {
        Text,
        Boolean,
        Range,
        Select,
        Multiselect,
        Complex
    }
}