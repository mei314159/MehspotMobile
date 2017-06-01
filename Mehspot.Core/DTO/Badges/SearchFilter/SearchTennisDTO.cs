using System;
using Mehspot.Core.Services;

namespace Mehspot.Core.DTO.Search
{
    [SearchFilterDto(Constants.BadgeNames.Tennis)]
    public class SearchTennisDTO : SearchFilterDTOBase
    {
        [Cell(Label = "Has Court Access", CellType = CellType.Boolean, Order = 0)]
        public bool? HasCourt { get; set; }

        [Cell(Label = "Gender", CellType = CellType.Select, Order = 1, OptionsKey = BadgeService.BadgeKeys.Gender)]
        public int? Gender { get; set; }

        [Cell(Label = "Skill Level", CellType = CellType.Select, Order = 2, OptionsKey = BadgeService.BadgeKeys.SkillLevel)]
        public int? SkillLevel { get; set; }

        [Cell(Label = "Age Range", CellType = CellType.Select, Order = 3, OptionsKey = BadgeService.BadgeKeys.TennisAgeRange)]
        public int? AgeRange { get; set; }
    }
}