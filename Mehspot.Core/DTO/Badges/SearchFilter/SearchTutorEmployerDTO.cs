using Mehspot.Core.Services;

namespace Mehspot.Core.DTO.Search
{

    public class SearchTutorEmployerDTO : SearchFilterDTOBase
    {
        [SearchProperty(Label = "Max Hourly Rate ($)", CellType = CellType.Range, Order = 0, MinValue = 0, MaxValue = 200)]
        public double? HourlyRate { get; set; }

        [SearchProperty(Label = "Subjects", CellType = CellType.Multiselect, Order = 1, OptionsKey = BadgeService.BadgeKeys.TutorEmployerSubject)]
        public string[] Subjects { get; set; }

        [SearchProperty(Label = "Min Grade", CellType = CellType.Select, Order = 2, OptionsKey = BadgeService.BadgeKeys.TutorEmployerGrade)]
        public int? Grade { get; set; }

        [SearchProperty(Label = "Max Grade", CellType = CellType.Select, Order = 3, OptionsKey = BadgeService.BadgeKeys.TutorEmployerGrade)]
        public int? MaxGrade { get; set; }
    }
}