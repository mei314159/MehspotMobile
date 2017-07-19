using System.Threading.Tasks;
using Mehspot.Core.Contracts.ViewControllers;
using Mehspot.Core.Services;

namespace Mehspot.Core.Models
{
    public class MessagingModel
    {
        private readonly MessagesService messagesService;
        private readonly IMessagingViewController viewController;

        public MessagingModel(MessagesService messagesService, IMessagingViewController viewController)
        {
            this.viewController = viewController;
            this.messagesService = messagesService;
        }

        public int Page { get; set; } = 1;

        public async Task LoadMessagesAsync()
        {
            //viewController.ViewHelper.ShowOverlay("Loading messages...");
            var messagesResult = await messagesService.GetMessages(Page++, viewController.ToUserId);
            if (messagesResult.IsSuccess)
            {
                this.viewController.DisplayMessages(messagesResult);
                this.viewController.ScrollingDown();
            }
            //viewController.ViewHelper.HideOverlay();
        }

        public async Task SendMessageAsync()
        {
            var message = viewController.MessageFieldValue;
            if (!string.IsNullOrWhiteSpace(message))
            {
                viewController.ToggleMessagingControls(false);
                var result = await messagesService.SendMessageAsync(message, viewController.ToUserId);

                if (result.IsSuccess)
                {
                    this.viewController.AddMessageBubbleToEnd(result.Data);
                    this.viewController.MessageFieldValue = string.Empty;
                    this.viewController.ScrollingDown();
                }

                viewController.ToggleMessagingControls(true);
            }
        }

        public async Task MarkMessagesReadAsync()
        {
            await messagesService.MarkMessagesReadAsync(viewController.ToUserId);
        }
    }
}
