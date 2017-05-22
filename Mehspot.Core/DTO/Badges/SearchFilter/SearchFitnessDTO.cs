using Mehspot.Core.Services;

namespace Mehspot.Core.DTO.Search
{

    public class SearchFitnessDTO : SearchFilterDTOBase
    {

        [SearchProperty(Label = "Fitness Type", CellType = CellType.Multiselect, Order = 0, OptionsKey = BadgeService.BadgeKeys.FitnessType)]
        public string[] FitnessTypes { get; set; }

        [SearchProperty(Label = "Gender", CellType = CellType.Select, Order = 1, OptionsKey = BadgeService.BadgeKeys.Gender)]
        public string Gender { get; set; }

        [SearchProperty(Label = "Age Range", CellType = CellType.Select, Order = 2, OptionsKey = BadgeService.BadgeKeys.FitnessAgeRange)]
        public string AgeRange { get; set; }
    }
}