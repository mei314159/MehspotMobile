using System;
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
            //TODO: toUserName value is a temporary solution that was used unit messagebord implemented. Should be removed when android version will be updated;
            var messagesResult = await messagesService.GetMessages (Page++, viewController.ToUserId, viewController.ToUserName); 
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
                //TODO: toUserName value is a temporary solution that was used unit messagebord implemented. Should be removed when android version will be updated;
                var result = await messagesService.SendMessageAsync (message, viewController.ToUserId, viewController.ToUserName);
                                  
                if (result.IsSuccess) {
                    this.viewController.AddMessageBubbleToEnd (result.Data);
                    this.viewController.MessageFieldValue = string.Empty;
                }

                viewController.ToggleMessagingControls (true);
            }
        }

        public async Task MarkMessagesReadAsync ()
        {
            await messagesService.MarkMessagesReadAsync (viewController.ToUserId);
        }
    }


}
