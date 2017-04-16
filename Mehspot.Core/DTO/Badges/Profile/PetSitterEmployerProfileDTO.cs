namespace Mehspot.Core.DTO.Badges
{

    public class PetSitterEmployerProfileDTO : IBadgeProfileValues
    {
        public string FirstName { get; set; }
        public string PetSitterEmployerPetType { get; set; }
        public string PetSitterEmployerPetWeight { get; set; }
        public string PetSitterEmployerAdditionalPetInformation { get; set; }
        public string PetSitterEmployerExpirienceRequired { get; set; }
        public string PetSitterEmployerExpirienceRequiredInformation { get; set; }
        public string PetSitterEmployerSearchable { get; set; }
        public string PetSitterEmployerSittersNotification { get; set; }
        public string PetSitterEmployerSittersNotificationDistance { get; set; }
        public string AdditionalInfo { get; set; }

        public string InfoLabel1
        {
            get { return string.Empty; }
        }

        public string InfoLabel2
        {
            get { return string.Empty; }
        }
    }

}