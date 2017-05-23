using Mehspot.Core.DTO.Search;

namespace Mehspot.Core.DTO.Badges
{

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

        [Cell("Age Range", 4, CellType.TextView)]
        public string HobbyAgeRange { get; set; }

        [Cell("Preferred Date and Time", 5, CellType.TextView)]
        public string HobbyPreferredDateTime { get; set; }

        [Cell("Preferred Location", 6, CellType.TextView)]
        public string HobbyPreferredLocation { get; set; }

        [Cell("Additional Information", 7, CellType.TextView)]
        public string HobbyAdditionalInformation { get; set; }

        public string InfoLabel1
        {
            get { return Gender; }
        }

        public string InfoLabel2
        {
            get { return HobbyType; }
        }

        public string FriendshipFamilyComposition { get; set; }
        public string FriendshipProfession { get; set; }
        public string FriendshipLookingFor { get; set; }
    }
}