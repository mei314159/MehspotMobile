using Mehspot.Core.Services;

namespace Mehspot.Core.DTO.Search
{

    public class SearchFriendshipDTO : SearchFilterDTOBase
    {
        [SearchProperty(Label = "Gender", CellType = CellType.Select, Order = 2, OptionsKey = BadgeService.BadgeKeys.Gender)]
        public string Gender { get; set; }

        public bool? IsTrained { get; set; }

        [SearchProperty(Label = "Age Range", CellType = CellType.Select, Order = 1, OptionsKey = BadgeService.BadgeKeys.HobbyAgeRange)]
        public string AgeRange { get; set; }

        [SearchProperty(Label = "Hobby", CellType = CellType.Select, Order = 0, OptionsKey = BadgeService.BadgeKeys.HobbyType)]
        public string[] HobbyTypes { get; set; }
    }

}