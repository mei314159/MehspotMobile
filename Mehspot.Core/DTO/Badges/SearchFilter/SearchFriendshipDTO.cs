using Mehspot.Core.Services;

namespace Mehspot.Core.DTO.Search
{
    [SearchFilterDto(Constants.BadgeNames.Friendship)]
    public class SearchFriendshipDTO : SearchFilterDTOBase
    {
        [Cell(Label = "Gender", CellType = CellType.Select, Order = 2, OptionsKey = BadgeService.BadgeKeys.Gender)]
        public string Gender { get; set; }

        public bool? IsTrained { get; set; }

        [Cell(Label = "Age Range", CellType = CellType.Select, Order = 1, OptionsKey = BadgeService.BadgeKeys.HobbyAgeRange)]
        public string AgeRange { get; set; }

        [Cell(Label = "Hobby", CellType = CellType.Select, Order = 0, OptionsKey = BadgeService.BadgeKeys.HobbyType)]
        public string[] HobbyTypes { get; set; }
    }

}