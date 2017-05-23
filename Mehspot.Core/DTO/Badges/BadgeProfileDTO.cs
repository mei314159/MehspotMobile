using System;
using Mehspot.Core.DTO.Profile;
using Mehspot.Core.DTO.Search;

namespace Mehspot.Core.DTO.Badges
{
    public class BadgeProfileDTO<T> : IBadgeProfileDTO where T : IBadgeProfileValues
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int BadgeId { get; set; }
        public string BadgeName { get; set; }
        public bool IsEnabled { get; set; }
        [Cell(CellType = CellType.Complex, Order = -2)]
        public T BadgeValues { get; set; }
        [Cell(CellType = CellType.Complex, Order = -1)]
        public BadgeProfileDetailsDTO Details { get; set; }
        public ProfilePartialDTO Profile { get; set; }

        public IAdditionalInfo AdditionalInfo { get { return BadgeValues; } }
    }
}