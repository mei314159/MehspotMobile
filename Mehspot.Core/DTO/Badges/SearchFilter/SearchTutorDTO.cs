namespace Mehspot.Core.DTO.Search
{
    public class SearchTutorDTO : SearchFilterDTOBase
    {
        public double? HourlyRate { get; set; }

        public string[] Subjects { get; set; }

        public int? CanTravel { get; set; }
    }
}