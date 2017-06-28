using Mehspot.Core.Services;

namespace Mehspot.Core.DTO.Search
{
    [SearchFilterDto(Constants.BadgeNames.Golf)]
    public class SearchGolfDTO : SearchFilterDTOBase
    {
        [Cell(Label = "Handicap", CellType = CellType.Range, Order = 0, MinValue = 0, MaxValue = 100, DefaultValue = 1, MaxValueProperty = nameof(MaxHandicap))]
        public double? Handicap { get; set; }

        [Cell(Label = "Max Handicap", CellType = CellType.Range, Order = 1, MinValue = 0, MaxValue = 100, DefaultValue = 100, MinValueProperty = nameof(Handicap))]
        public double? MaxHandicap { get; set; }

        [Cell(Label = "Gender", CellType = CellType.Select, Order = 2, OptionsKey = BadgeService.BadgeKeys.Gender)]
        public string Gender { get; set; }

        [Cell(Label = "Age Group", CellType = CellType.Select, Order = 3, OptionsKey = BadgeService.BadgeKeys.GolfAgeGroup)]
        public string AgeRange { get; set; }
    }
}