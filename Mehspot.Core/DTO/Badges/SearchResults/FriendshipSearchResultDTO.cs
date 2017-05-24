using System;
using Mehspot.Core.DTO.Search;
using Mehspot.Models.ViewModels;

namespace Mehspot.Core.DTO.Badges
{
    [SearchResultDto(Constants.BadgeNames.Friendship)]
    public class FriendshipSearchResultDTO : ISearchResultDTO
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