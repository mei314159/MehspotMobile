namespace Mehspot.Core.DTO.Search
{

    public class SearchBabysitterEmployerDTO : SearchFilterDTOBase
    {
        public int? HourlyRate { get; set; }

        public bool? OwnCar { get; set; }

        public bool? CanDrive { get; set; }

        public int? AgeRange { get; set; }
    }
}
