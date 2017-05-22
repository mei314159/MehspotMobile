using System;
using Mehspot.Core.Services;

namespace Mehspot.Core.DTO.Search
{
    public class SearchTennisDTO : SearchFilterDTOBase
    {
        [SearchProperty(Label = "Has Court Access", CellType = CellType.Boolean, Order = 0)]
        public bool? HasCourt { get; set; }

        [SearchProperty(Label = "Gender", CellType = CellType.Select, Order = 1, OptionsKey = BadgeService.BadgeKeys.Gender)]
        public int? Gender { get; set; }

        [SearchProperty(Label = "Skill Level", CellType = CellType.Select, Order = 2, OptionsKey = BadgeService.BadgeKeys.SkillLevel)]
        public int? SkillLevel { get; set; }

        [SearchProperty(Label = "Age Range", CellType = CellType.Select, Order = 3, OptionsKey = BadgeService.BadgeKeys.TennisAgeRange)]
        public int? AgeRange { get; set; }
    }
}