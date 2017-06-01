namespace Mehspot.Core.DTO.Subdivision
{
    public class AddressDTO
    {
        public int Id { get; set; }

        public string FormattedAddress { get; set; }

        public string StreetNumber { get; set; }

        public string Route { get; set; }

        public string Sublocality { get; set; }

        public string Locality { get; set; }

        public string AdministrativeAreaLevel1 { get; set; }

        public string AdministrativeAreaLevel2 { get; set; }

        public string AdministrativeAreaLevel3 { get; set; }

        public string AdministrativeAreaLevel4 { get; set; }

        public string AdministrativeAreaLevel5 { get; set; }

        public int GoverningDistrictId { get; set; }

        public string Country { get; set; }

        public int? CountryId { get; set; }

        public string PostalCode { get; set; }

        public string PostalCodeSuffix { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public virtual CountryDTO CountryModel { get; set; }

        public virtual GoverningDistrictDTO GoverningDistrict { get; set; }
    }
}
