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

        public static readonly ExcludingKey[] ExcludedKeys = {
            new ExcludingKey(State),
            new ExcludingKey(Zip),
            new ExcludingKey(City),
            new ExcludingKey(Subdivision),
            new ExcludingKey("PetSitterEmployerSearchable", true), 
            new ExcludingKey("PetSitterEmployerSittersNotification", true),
            new ExcludingKey("PetSitterEmployerSittersNotificationDistance") };
    }

    public class ExcludingKey
    {
        public ExcludingKey(string name, bool setDefaultValue = false)
        {
            Name = name;
            UseDefault = setDefaultValue;
        }

        public string Name { get; set; }

        public bool UseDefault { get; set; }
    }
}
