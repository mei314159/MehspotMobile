namespace Mehspot.Core.DTO.Search
{

    public class SearchTutorEmployerDTO : SearchFilterDTOBase
    {
        public double? HourlyRate { get; set; }

        public string[] Subjects { get; set; }

        public int? Grade { get; set; }

        public int? MaxGrade { get; set; }
    }
}