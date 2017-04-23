using System;
using MehSpot.Models.ViewModels;

namespace Mehspot.Core.DTO.Badges
{

    public class TutorSearchResultDTO : ISearchResultDTO
    {
        public BadgeUserDetailsFilterDTO Details { get; set; }
        public double? HourlyRate { get; set; }
        public int? CanTravel { get; set; }
        public string CanTravelLabel { get; set; }

        public string InfoLabel1
        {
            get { return $"${(this.HourlyRate ?? 0)}/hr"; }
        }

        public string InfoLabel2
        {
            get { return "Can Travel: " + (!string.IsNullOrEmpty(CanTravelLabel) ? MehspotResources.ResourceManager.GetString(CanTravelLabel) ?? CanTravelLabel : "N/A"); }
        }
    }
}