using System;
using System.Threading.Tasks;
using Mehspot.Core.Services;

namespace Mehspot.Core.Models
{
    public class WriteReviewModel
    {
        private BadgeService badgeService;
        private IWriteReviewController controller;

        public WriteReviewModel(BadgeService badgeService, IWriteReviewController controller)
        {
            this.badgeService = badgeService;
            this.controller = controller;
        }

        public async Task SaveAsync(string text)
        {
            controller.ViewHelper.ShowOverlay("Wait...");
            var result = await this.badgeService.WriteRecommendationAsync(controller.BadgeId, controller.UserId, text);
            if (result.IsSuccess)
            {
                this.controller.NotifySaved(result.Data);
            }
            else
            {
                controller.ViewHelper.ShowAlert("Meh...  Sorry for the trouble.", "Mehspot support has been notified of this issue." + Environment.NewLine + "Please try again in a later time.");
            }

            controller.ViewHelper.HideOverlay();
        }
    }
}
