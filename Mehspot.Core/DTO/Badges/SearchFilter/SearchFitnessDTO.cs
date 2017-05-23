using Mehspot.Core.Services;

namespace Mehspot.Core.DTO.Search
{

    public class SearchFitnessDTO : SearchFilterDTOBase
    {

        [Cell(Label = "Fitness Type", CellType = CellType.Multiselect, Order = 0, OptionsKey = BadgeService.BadgeKeys.FitnessType)]
        public string[] FitnessTypes { get; set; }

        [Cell(Label = "Gender", CellType = CellType.Select, Order = 1, OptionsKey = BadgeService.BadgeKeys.Gender)]
        public string Gender { get; set; }

        [Cell(Label = "Age Range", CellType = CellType.Select, Order = 2, OptionsKey = BadgeService.BadgeKeys.FitnessAgeRange)]
        public string AgeRange { get; set; }
    }
}