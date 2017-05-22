using Mehspot.Core.Services;

namespace Mehspot.Core.DTO.Search
{

    public class SearchGolfDTO : SearchFilterDTOBase
    {
        [SearchProperty(Label = "Min Handicap", CellType = CellType.Range, Order = 0, MinValue = 0, MaxValue = 100)]
        public double? Handicap { get; set; }

        [SearchProperty(Label = "Max Handicap", CellType = CellType.Range, Order = 1, MinValue = 0, MaxValue = 100)]
        public double? MaxHandicap { get; set; }

        [SearchProperty(Label = "Gender", CellType = CellType.Select, Order = 2, OptionsKey = BadgeService.BadgeKeys.Gender)]
        public string Gender { get; set; }

        [SearchProperty(Label = "Age Group", CellType = CellType.Select, Order = 3, OptionsKey = BadgeService.BadgeKeys.GolfAgeGroup)]
        public string AgeRange { get; set; }
    }
}