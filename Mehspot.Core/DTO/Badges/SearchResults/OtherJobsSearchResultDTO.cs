using System;
using Mehspot.Core.DTO.Search;
using Mehspot.Models.ViewModels;

namespace Mehspot.Core.DTO.Badges
{
    [SearchResultDto(Constants.BadgeNames.OtherJobs)]
    public class OtherJobsSearchResultDTO : ISearchResultDTO
    {
        public BadgeUserDetailsFilterDTO Details { get; set; }

        public double? HourlyRate { get; set; }

        public string Gender { get; set; }

        public string Jobs { get; set; }

        public bool? IsHired { get; set; }

        public string AgeRange { get; set; }

        public string InfoLabel1
        {
            get { return $"${(this.HourlyRate ?? 0)}/hr"; }
        }

        public string InfoLabel2
        {
            get { return string.Empty; }
        }
    }
}