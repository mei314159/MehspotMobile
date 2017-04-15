namespace Mehspot.Core.DTO.Badges
{

    public class TennisProfileDTO : IBadgeProfileValues
    {
        public string FirstName { get; set; }
        public string Gender { get; set; }
        public string TennisZip { get; set; }
        public string TennisSubdivision { get; set; }
        public bool HasCourtAccess { get; set; }
        public string TennisAgeRange { get; set; }
        public string SkillLevel { get; set; }
        public string PreferredSide { get; set; }
        public string Description { get; set; }

        public string InfoLabel1
        {
            get { return Gender; }
        }

        public string InfoLabel2
        {
            get { return SkillLevel; }
        }
    }
}