using Mehspot.Core.Services;

namespace Mehspot.Core.DTO.Search
{
    [SearchFilterDto(Constants.BadgeNames.PetSitter)]
    public class SearchPetSitterDTO : SearchFilterDTOBase
    {
        public double? HourlyRate { get; set; }

        public string Gender { get; set; }

        [Cell(Label = "Pet Types", CellType = CellType.Multiselect, Order = 0, OptionsKey = BadgeService.BadgeKeys.PetSitterPetType)]
        public string[] PetType { get; set; }

        public bool? CanTravel { get; set; }

        public bool IsHired { get; set; }
    }

}