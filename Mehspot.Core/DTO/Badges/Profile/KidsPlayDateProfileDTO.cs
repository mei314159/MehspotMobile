using Mehspot.Core.DTO.Search;
using Mehspot.Core.Services;

namespace Mehspot.Core.DTO.Badges
{

    [ViewProfileDto(Constants.BadgeNames.KidsPlayDate)]
    public class KidsPlayDateProfileDTO : IBadgeProfileValues
    {
        [Cell("First Name", 0, CellType.TextView)]
        public string FirstName { get; set; }

        [Cell("Kid Age", 1, CellType.TextView)]
        public string KidsPlayDateKidAge { get; set; }

        [Cell("Gender", 2, CellType.TextView)]
        public string Gender { get; set; }

        [Cell("Race", 3, CellType.TextView)]
        public string KidsPlayDateRace { get; set; }

        [Cell("Parents Information", 4, CellType.TextView)]
        public string KidsPlayDateParentsInformation { get; set; }

        [Cell("Desired Date and Time", 5, CellType.TextView)]
        public string KidsPlayDateDesiredDateAndTime { get; set; }

        [Cell("Desired Location", 6, CellType.TextView)]
        public string KidsPlayDateDesiredLocation { get; set; }

        [Cell("Desired Gender", 7, CellType.TextView)]
        public string KidsPlayDateDesiredGender { get; set; }

        [Cell("Desired Age Range", 8, CellType.TextView)]
        public string KidsPlayDateDesiredAgeRange { get; set; }

        [Cell("Desired Spoken Language", 9, CellType.TextView)]
        public string KidsPlayDateSpokenLanguage { get; set; }

        [Cell("Preferred Method of Play", 10, CellType.TextView)]
        public string KidsPlayDatePreferredMethodOfPlay { get; set; }

        [Cell("My Child", 11, CellType.TextView)]
        public string KidsPlayDateMyChild { get; set; }

        [Cell("Additional Information", 12, CellType.TextView)]
        public string KidsPlayDateAdditionalInformation { get; set; }

        public string InfoLabel1
        {
            get { return Gender; }
        }

        public string InfoLabel2
        {
            get { return "Kid Age: " + KidsPlayDateKidAge; }
        }
    }
}