using Mehspot.Core.DTO.Search;

namespace Mehspot.Core.DTO.Badges
{

    [ViewProfileDto(Constants.BadgeNames.BabysitterEmployer)]
    public class BabysitterEmployerProfileDTO : IBadgeProfileValues
    {
        public string FirstName { get; set; }
        public double? HourlyRate { get; set; }
        public string AgeRange { get; set; }

        [Cell("Own Car", 0, CellType.Boolean, ReadOnly = true)]
        public bool OwnCar { get; set; }

        [Cell("Can Drive", 1, CellType.Boolean, ReadOnly = true)]
        public bool CanDrive { get; set; }

        [Cell("Additional Information", 2, CellType.TextView)]
        public string Description { get; set; }

        public string InfoLabel1
        {
            get { return $"${(this.HourlyRate ?? 0)}/hr"; }
        }

        public string InfoLabel2
        {
            get { return this.AgeRange; }
        }
    }
}