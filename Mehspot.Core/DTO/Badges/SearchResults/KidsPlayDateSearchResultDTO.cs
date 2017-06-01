using System;
using Mehspot.Core.DTO.Search;
using Mehspot.Models.ViewModels;

namespace Mehspot.Core.DTO.Badges
{
    [SearchResultDto(Constants.BadgeNames.KidsPlayDate)]
    public class KidsPlayDateSearchResultDTO : ISearchResultDTO
    {
        public BadgeUserDetailsFilterDTO Details { get; set; }

        public string Gender { get; set; }

        public bool? IsPlayed { get; set; }

        public int? Age { get; set; }

        public string InfoLabel1
        {
            get { return "Age:" + (Age ?? 0); }
        }

        public string InfoLabel2
        {
            get { return string.Empty; }
        }
    }
}