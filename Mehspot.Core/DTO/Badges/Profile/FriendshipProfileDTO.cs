using Mehspot.Core.DTO.Search;
using Mehspot.Core.Services;

namespace Mehspot.Core.DTO.Badges
{
    [ViewProfileDto(Constants.BadgeNames.Friendship)]
    public class FriendshipProfileDTO : IBadgeProfileValues
    {
        [Cell("First Name", 0, CellType.TextView)]
        public string FirstName { get; set; }

        [Cell("Hobby", 1, CellType.TextView)]
        public string HobbyType { get; set; }

        [Cell("Other Hobby", 2, CellType.TextView)]
        public string HobbyOther { get; set; }

        [Cell("Gender", 3, CellType.TextView)]
        public string Gender { get; set; }

        [Cell("Family Composition", 4, CellType.TextView)]
        public string FriendshipFamilyComposition { get; set; }

        [Cell("Profession", 5, CellType.TextView)]
        public string FriendshipProfession { get; set; }

        [Cell("Looking For", 6, CellType.TextView)]
        public string FriendshipLookingFor { get; set; }

        [Cell("Age Range", 7, CellType.TextView)]
        public string HobbyAgeRange { get; set; }


        [Cell("Additional Information", 8, CellType.TextView)]
        public string HobbyAdditionalInformation { get; set; }

        public string InfoLabel1
        {
            get { return Gender; }
        }

        public string InfoLabel2
        {
            get { return HobbyType; }
        }
    }
}