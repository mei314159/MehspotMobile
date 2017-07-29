using System;
using System.Collections.Generic;

namespace Mehspot.Core.DTO
{
    public class UserProfileSummaryDTO
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string ProfilePicturePath { get; set; }

        public int RecommendationsCount { get; set; }

        public int ReferencesCount { get; set; }

        public List<BadgeSummaryBaseDTO> RegisteredBadges { get; set; } = new List<BadgeSummaryBaseDTO>();
    }
}
