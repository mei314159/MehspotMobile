namespace Mehspot.Core.DTO.Search
{
    public class SearchBabysitterDTO : SearchFilterDTOBase
    {
        public int? HourlyRate { get; set; }

        public bool? OwnCar { get; set; }

        public bool? HasCertification { get; set; }

        public int? AgeRange { get; set; }
    }
}
