using System;
using MehSpot.Models.ViewModels;

namespace Mehspot.Core.DTO.Badges
{

    public class GolfSearchResultDTO : ISearchResultDTO
    {
        public BadgeUserDetailsFilterDTO Details { get; set; }
        public bool Played { get; set; }
        public string AgeRange { get; set; }
        public string Gender { get; set; }
        public string GenderLabel { get; set; }
        public double? Handicap { get; set; }

        public string InfoLabel1
        {
            get { return GenderLabel; }
        }

        public string InfoLabel2
        {
            get { return "Handicap: " + (Handicap ?? 0).ToString(); }
        }
    }
}