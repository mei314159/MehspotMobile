using System;
using System.Collections.Generic;

namespace Mehspot.Core.DTO.Badges
{
    public class BadgeRecommendationDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int LikesCount { get; set; }
        public List<BadgeUserRecommendationDTO> Recommendations { get; set; }
    }

    public class BadgeUserRecommendationDTO
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; }
        public string FromUserId { get; set; }
        public string FromUserName { get; set; }
        public bool FromUserIsElite { get; set; }
        public bool IsEnabled { get; set; } = true;
        public string FromUserProfilePicturePath { get; set; }
        public BadgeDescriptionTypeEnum BadgeDescriptionType { get; set; }
    }
}