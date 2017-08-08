using Mehspot.Core.DTO.Search;

namespace Mehspot.Core.DTO.Badges
{
    public class BadgeProfileDetailsDTO
    {
        public string UserId { get; set; }

        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string SubdivisionName { get; set; }

        public int LikesCount { get; set; }

        public bool AlreadyLiked { get; set; }

        public bool Played { get; set; }

        public bool HasPicture { get; set; }

        public string ProfilePicturePath { get; set; }

        public bool IsFavorite { get; set; }

        public int RecommendationsCount { get; set; }

        public double? Distance { get; set; }

        public bool IsHired { get; set; }

        public bool HasReference { get; set; }

        public int ReferenceCount { get; set; }
    }
}