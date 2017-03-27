using System.Collections.Generic;

namespace Mehspot.Core.DTO.Badges
{

    public class BadgeItemDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ValueType { get; set; }

        public bool Required { get; set; }

        public string DefaultValue { get; set; }

        public virtual List<BadgeItemOptionDTO> BadgeItemOptions { get; set; }
    }
    
}