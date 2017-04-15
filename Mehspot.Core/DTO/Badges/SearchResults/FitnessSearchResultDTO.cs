using System;
using MehSpot.Models.ViewModels;

namespace Mehspot.Core.DTO.Badges
{

    public class FitnessSearchResultDTO : ISearchResultDTO
    {
        public BadgeUserDetailsFilterDTO Details { get; set; }
        public string Gender { get; set; }
        public string GenderLabel { get; set; }
        public bool IsTrained { get; set; }
        public string AgeRange { get; set; }
        public string FitnessTypes { get; set; }

        public string InfoLabel1
        {
            get { return GenderLabel; }
        }

        public string InfoLabel2
        {
            get { return string.Empty; }
        }
    }

}