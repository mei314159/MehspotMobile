namespace Mehspot.DTO
{
    public class SubdivisionDTO
    {
        public int Id { get; set; }

        public int? OptionId { get; set; }

        public string SubdivisionIdentifier { get; set; }

        public int AddressId { get; set; }

        public string ZipCode { get; set; }

        public bool IsVerified { get; set; }

        public bool IsVerifiedByCurrentUser { get; set; }

        public string FormattedAddress { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string DisplayName { get; set; }

        public string SelectValue { get; set; }
    }
}