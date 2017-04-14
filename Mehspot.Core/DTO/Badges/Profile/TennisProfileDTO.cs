namespace Mehspot.Core.DTO.Badges
{

    public class TennisProfileDTO : IBadgeProfileValues
    {
        public bool HasCourt { get; set; }

        public bool Played { get; set; }
        public string AgeRange { get; set; }
        public string Gender { get; set; }
        public string SkillLevel { get; set; }
        public string PreferredSide { get; set; }

        public string TennisAgeRange { get; set; }
        public string TennisSubdivision { get; set; }
        public string TennisZip { get; set; }

        public string InfoLabel1
        {
            get { return Played ? "Played" : string.Empty; }
        }

        public string InfoLabel2
        {
            get { return SkillLevel; }
        }
    }
}