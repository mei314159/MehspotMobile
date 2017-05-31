using System;
using Mehspot.Core.Contracts.Wrappers;
using Mehspot.Core.DTO;
using Mehspot.Core.DTO.Badges;

namespace Mehspot.Core
{
    public interface IWriteReviewController
    {
        IViewHelper ViewHelper { get; }
        int BadgeId { get; }
        string UserId { get; }
        event Action<BadgeUserRecommendationDTO> OnSave;
        void NotifySaved(BadgeUserRecommendationDTO data);
    }
}
