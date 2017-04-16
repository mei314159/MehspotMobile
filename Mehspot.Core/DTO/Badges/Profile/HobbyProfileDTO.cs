namespace Mehspot.Core.DTO.Badges
{
    
    public class HobbyProfileDTO : IBadgeProfileValues
    {
        public string FirstName { get; set; }
        public string HobbyType { get; set; }
        public string HobbyOther { get; set; }
        public string Gender { get; set; }
        public string HobbyAgeRange { get; set; }
        public string HobbyPreferredDateTime { get; set; }
        public string HobbyPreferredLocation { get; set; }
        public string HobbyAdditionalInformation { get; set; }

        public string InfoLabel1
        {
            get { return Gender; }
        }

        public string InfoLabel2
        {
            get { return HobbyType; }
        }
    }
}