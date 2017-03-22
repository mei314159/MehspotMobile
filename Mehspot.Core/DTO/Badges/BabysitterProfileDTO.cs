using System.Collections.Generic;
namespace MehSpot.Models.ViewModels
{

    public class BabysitterProfileDTO:IBadgeProfileValues
    {
        public string AgeRange { get; set; }
        public string BabysitterAdditionalInformation { get; set; }
        public string BabysitterCertificationInfo { get; set; }
        public string BabysitterOtherCertifications { get; set; }
        public bool CanDrive { get; set; }
        public string City { get; set; }
        public string FirstName { get; set; }
        public string HourlyRate { get; set; }
        public bool OwnCar { get; set; }
        public string Zip { get; set; }
    }
}