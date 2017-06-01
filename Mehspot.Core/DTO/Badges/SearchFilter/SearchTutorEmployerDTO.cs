using Mehspot.Core.Services;

namespace Mehspot.Core.DTO.Search
{
    [SearchFilterDto(Constants.BadgeNames.TutorEmployer)]
    public class SearchTutorEmployerDTO : SearchFilterDTOBase
    {
        [Cell(Label = "Max Hourly Rate ($)", CellType = CellType.Range, Order = 0, MinValue = 0, MaxValue = 200)]
        public double? HourlyRate { get; set; }

        [Cell(Label = "Subjects", CellType = CellType.Multiselect, Order = 1, OptionsKey = BadgeService.BadgeKeys.TutorEmployerSubject)]
        public string[] Subjects { get; set; }

        [Cell(Label = "Min Grade", CellType = CellType.Select, Order = 2, OptionsKey = BadgeService.BadgeKeys.TutorEmployerGrade)]
        public int? Grade { get; set; }

        [Cell(Label = "Max Grade", CellType = CellType.Select, Order = 3, OptionsKey = BadgeService.BadgeKeys.TutorEmployerGrade)]
        public int? MaxGrade { get; set; }
    }
}