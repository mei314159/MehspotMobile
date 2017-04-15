namespace Mehspot.Core.DTO.Badges
{

    public class GolfProfileDTO : IBadgeProfileValues
    {
        public string FirstName { get; set; }
        public string GolfHomeCourseZip { get; set; }
        public string GolfHomeCourseSubdivision { get; set; }
        public string GolfHomeCourseType { get; set; }
        public string GolfHomeCourseGreenFeeRange { get; set; }
        public string GolfGreenFeeRangeWTP { get; set; }
        public string GolfHomeCourtPreference { get; set; }
        public string GolfHandicap { get; set; }
        public string Gender { get; set; }
        public string GolfAgeGroup { get; set; }
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