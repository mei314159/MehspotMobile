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

        [Cell(Label = "Hired Before", CellType = CellType.Boolean, Order = 999)]
        public bool IsHired { get; set; }

        [Cell(Label = "Add Reference", CellType = CellType.Boolean, Order = 1000)]
        public bool HasReference { get; set; }

        [Cell(Label = "References Count", CellType = CellType.TextView, Order = 1001)]
        public int ReferenceCount { get; set; }
    }
}