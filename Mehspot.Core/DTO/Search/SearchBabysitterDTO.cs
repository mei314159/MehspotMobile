namespace Mehspot.Core.DTO.Search
{
    public class SearchBabysitterDTO : SearchFilterDTO
    {
        public float? HourlyRate { get; set; }

        public bool? HasCar { get; set; }

        public bool? HasCertifications { get; set; }

        public int? AgeRange { get; set; }
    }
}
