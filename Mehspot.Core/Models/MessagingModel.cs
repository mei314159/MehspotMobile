using System.Threading.Tasks;
using Mehspot.Core.Contracts.ViewControllers;
using Mehspot.Core.Messaging;

namespace Mehspot.Core.Models
{
    public class MessagingModel
    {
        private readonly MessagesService messagesService;
        private readonly IMessagingViewController viewController;

        public MessagingModel (MessagesService messagesService, IMessagingViewController viewController)
        {
            this.viewController = viewController;
            this.messagesService = messagesService;
        }

        public int Page { get; private set; } = 1;

        public async Task LoadMessagesAsync ()
        {
            viewController.ViewHelper.ShowOverlay ("Loading messages...");
            var messagesResult = await messagesService.GetMessages (viewController.ToUserName, Page++);
            if (messagesResult.IsSuccess) {
                viewController.DisplayMessages (messagesResult);
            }
            viewController.ViewHelper.HideOverlay ();
        }

        public async Task SendMessageAsync ()
        {
            var message = viewController.MessageFieldValue;
            if (!string.IsNullOrWhiteSpace (message)) {
                viewController.ToggleMessagingControls (false);
                var result = await this.messagesService.SendMessageAsync (viewController.ToUserName, message);
                if (result.IsSuccess) {
                    this.viewController.AddMessageBubbleToEnd (result.Data);
                    this.viewController.MessageFieldValue = string.Empty;
                }

                viewController.ToggleMessagingControls (true);
            }
        }
    }


}
