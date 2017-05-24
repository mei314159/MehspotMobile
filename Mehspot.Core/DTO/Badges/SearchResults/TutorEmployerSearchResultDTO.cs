using System;
using Mehspot.Core.DTO.Search;
using Mehspot.Models.ViewModels;

namespace Mehspot.Core.DTO.Badges
{
    [SearchResultDto(Constants.BadgeNames.TutorEmployer)]
    public class TutorEmployerSearchResultDTO : ISearchResultDTO
    {
        public BadgeUserDetailsFilterDTO Details { get; set; }
        public double? HourlyRate { get; set; }
        public int? Grade { get; set; }
        public string GradeLabel { get; set; }

        public string InfoLabel1
        {
            get { return $"${(this.HourlyRate ?? 0)}/hr"; }
        }

        public string InfoLabel2
        {
            get { return "Grade" + (MehspotResources.ResourceManager.GetString(GradeLabel) ?? GradeLabel); }
        }
    }
}