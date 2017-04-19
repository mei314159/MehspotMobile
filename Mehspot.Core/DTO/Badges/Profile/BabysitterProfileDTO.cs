namespace Mehspot.Core.DTO.Badges
{

    public class BabysitterProfileDTO : IBadgeProfileValues
    {
        public string FirstName { get; set; }
        public double? HourlyRate { get; set; }
        public string AgeRange { get; set; }
        public bool OwnCar { get; set; }
        public bool CanDrive { get; set; }
        public string BabysitterCertificationInfo { get; set; }
        public string BabysitterOtherCertifications { get; set; }
        public string BabysitterAdditionalInformation { get; set; }

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