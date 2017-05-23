using Mehspot.Core.DTO.Search;

namespace Mehspot.Core.DTO.Badges
{

    public class GolfProfileDTO : IBadgeProfileValues
    {
        [Cell("Player First Name", 0, CellType.TextView)]
        public string FirstName { get; set; }

        [Cell("Gender", 1, CellType.TextView)]
        public string Gender { get; set; }

        [Cell("Age Group", 2, CellType.TextView)]
        public string GolfAgeGroup { get; set; }

        [Cell("Handicap", 3, CellType.TextView)]
        public string GolfHandicap { get; set; }

        [Cell("Home Course Zip", 4, CellType.TextView)]
        public string GolfHomeCourseZip { get; set; }

        [Cell("Home Course Subdivision", 5, CellType.TextView)]
        public string GolfHomeCourseSubdivision { get; set; }

        [Cell("Home Course Type", 6, CellType.TextView)]
        public string GolfHomeCourseType { get; set; }

        [Cell("Green Fee Range", 7, CellType.TextView)]
        public string GolfHomeCourseGreenFeeRange { get; set; }

        [Cell("Green Fee Amount", 8, CellType.TextView)]
        public string GolfGreenFeeRangeWTP { get; set; }

        [Cell("Preference to play at home court", 9, CellType.TextView)]
        public string GolfHomeCourtPreference { get; set; }

        [Cell("Additional Information", 10, CellType.TextView)]
        public string GolfAdditionalInformation { get; set; }

        public string InfoLabel1
        {
            get { return Gender; }
        }

        public string InfoLabel2
        {
            get { return "Handicap: " + (GolfHandicap ?? "0").ToString(); }
        }
    }
}