namespace Mehspot.Core.DTO.Badges
{
    public static class ProfileKeys
    {
        public const string FirstName = "FirstName";

        public const string Gender = "Gender";

        public const string State = "State";

        public const string Zip = "Zip";

        public const string City = "City";

        public const string Subdivision = "Subdivision";

        public static readonly string[] ExcludedKeys = { State, Zip, City, Subdivision, "PetSitterEmployerSearchable", "PetSitterEmployerSittersNotification", "PetSitterEmployerSittersNotificationDistance" };
    }
}
