using System;
using MehSpot.Models.ViewModels;

namespace Mehspot.Core.DTO.Badges
{

    public class TennisSearchResultDTO : ISearchResultDTO
    {
        public BadgeUserDetailsFilterDTO Details { get; set; }
        public bool HasCourt { get; set; }
        public bool Played { get; set; }
        public string AgeRange { get; set; }
        public string Gender { get; set; }
        public string SkillLevel { get; set; }
        public string SkillLevelLabel { get; set; }
        public string PreferredSide { get; set; }

        public string InfoLabel1
        {
            get { return Played ? "Played" : string.Empty; }
        }

        public string InfoLabel2
        {
            get { return SkillLevelLabel; }
        }
    }
}