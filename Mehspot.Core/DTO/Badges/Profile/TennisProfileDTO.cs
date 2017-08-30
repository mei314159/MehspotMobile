using Mehspot.Core.DTO.Search;
using Mehspot.Core.Services;

namespace Mehspot.Core.DTO.Badges
{
    [ViewProfileDto(Constants.BadgeNames.Tennis)]
    public class TennisProfileDTO : IBadgeProfileValues
    {
        [Cell("First Name", 0, CellType.TextView)]
        public string FirstName { get; set; }

        [Cell("Gender", 1, CellType.TextView)]
        public string Gender { get; set; }

        [Cell("Has Court Access", 2, CellType.Boolean, ReadOnly = true)]
        public bool HasCourtAccess { get; set; }

        [Cell("Skill Level", 3, CellType.TextView)]
        public string SkillLevel { get; set; }

        [Cell("Preferred Side", 4, CellType.TextView)]
        public string PreferredSide { get; set; }

        [Cell("Court Zip", 4, CellType.TextView)]
        public string TennisZip { get; set; }

        [Cell("Court Subdivision", 5, CellType.TextView)]
        public string TennisSubdivision { get; set; }

        [Cell("Age Range", 6, CellType.TextView)]
        public string TennisAgeRange { get; set; }

        [Cell("Additional Information", 7, CellType.TextView)]
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