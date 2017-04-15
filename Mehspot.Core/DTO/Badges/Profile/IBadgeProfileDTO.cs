namespace Mehspot.Core.DTO.Badges
{
    public interface IBadgeProfileDTO
    {
        BadgeProfileDetailsDTO Details { get; set; }

        IAdditionalInfo AdditionalInfo { get; }
    }
}