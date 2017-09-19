using System.Threading.Tasks;
using Mehspot.Core.Contracts.ViewControllers;
using Mehspot.Core.Services;

namespace Mehspot.Core.Models
{
    public class GroupMessagingModel
    {
        private readonly GroupService groupService;
        private readonly IGroupMessagingViewController viewController;

        public GroupMessagingModel(GroupService groupService, IGroupMessagingViewController viewController)
        {
            this.viewController = viewController;
            this.groupService = groupService;
        }

        public int Page { get; set; } = 1;

        public async Task LoadMessagesAsync()
        {
            //viewController.ViewHelper.ShowOverlay("Loading messages...");
            var messagesResult = await groupService.GetMessages(Page++, viewController.GroupId).ConfigureAwait(false);
            if (messagesResult.IsSuccess)
            {
                this.viewController.DisplayMessages(messagesResult);
            }
            //viewController.ViewHelper.HideOverlay();
        }

        public async Task SendMessageAsync()
        {
            var message = viewController.MessageFieldValue;
            if (!string.IsNullOrWhiteSpace(message))
            {
                viewController.ToggleMessagingControls(false);
                var result = await groupService.SendMessageAsync(message, viewController.GroupId).ConfigureAwait(false);

                if (result.IsSuccess)
                {
                    this.viewController.AddMessageBubbleToEnd(result.Data);
                    this.viewController.MessageFieldValue = string.Empty;
                }

                viewController.ToggleMessagingControls(true);
            }
        }
    }
}
