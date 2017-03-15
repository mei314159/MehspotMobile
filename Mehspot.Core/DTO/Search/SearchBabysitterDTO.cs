using System;

namespace Mehspot.Core.DTO.Search
{
    public class SearchBabysitterDTO : ISearchFilterDTO
    {
        public BaseFilterDTO Details { get; set; } = new BaseFilterDTO ();

        public float? HourlyRate { get; set; }

        public bool? HasCar { get; set; }

        public bool? HasCertifications { get; set; }

        public int? AgeRange { get; set; }
    }
}
