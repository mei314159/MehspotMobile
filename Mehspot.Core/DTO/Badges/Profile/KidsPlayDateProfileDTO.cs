namespace Mehspot.Core.DTO.Badges
{

    public class KidsPlayDateProfileDTO : IBadgeProfileValues
    {
        public string FirstName { get; set; }
        public string KidsPlayDateKidAge { get; set; }
        public string Gender { get; set; }
        public string KidsPlayDateRace { get; set; }
        public string KidsPlayDateParentsInformation { get; set; }
        public string KidsPlayDateDesiredDateAndTime { get; set; }
        public string KidsPlayDateDesiredLocation { get; set; }
        public string KidsPlayDateDesiredGender { get; set; }
        public string KidsPlayDateDesiredAgeRange { get; set; }
        public string KidsPlayDateSpokenLanguage { get; set; }
        public string KidsPlayDatePreferredMethodOfPlay { get; set; }
        public string KidsPlayDateMyChild { get; set; }
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