namespace Mehspot.Core.DTO.Search
{
    public class SearchBabysitterDTO : ISearchFilterDTO
    {
        public BaseFilterDTO Details { get; set; } = new BaseFilterDTO ();

        public int? HourlyRate { get; set; }

        public bool? OwnCar { get; set; }

        public bool? HasCertification { get; set; }

        public int? AgeRange { get; set; }
    }
}
