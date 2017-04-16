using System;
using MehSpot.Models.ViewModels;

namespace Mehspot.Core.DTO.Badges
{

    public class HobbySearchResultDTO : ISearchResultDTO
    {
        public BadgeUserDetailsFilterDTO Details { get; set; }

        public string Gender { get; set; }

        public bool? IsTrained { get; set; }

        public string AgeRange { get; set; }

        public string HobbyTypes { get; set; }

        public string InfoLabel1
        {
            get { return string.Empty; }
        }

        public string InfoLabel2
        {
            get { return string.Empty; }
        }
    }
    
}