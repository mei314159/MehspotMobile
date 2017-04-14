using System;
using System.Collections.Generic;

namespace Mehspot.Core.DTO.Badges
{

    public class EditBadgeProfileDTO : Dictionary<string, BadgeItemValueDTO>, IBadgeProfileValues
    {
        public string InfoLabel1 { get; }

        public string InfoLabel2 { get; }
    }
}