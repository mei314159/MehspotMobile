using System;
using mehspot.Core.Auth;
using mehspot.Core.Contracts;
using Mehspot.Core.DTO;
using Mehspot.Core.Messaging;
using Microsoft.AspNet.SignalR.Client;

namespace Mehspot.Core
{
    public sealed class MehspotAppContext
    {
        private static readonly Lazy<MehspotAppContext> lazy = new Lazy<MehspotAppContext> (() => new MehspotAppContext ());
        private HubConnection hubConnection;

        public static MehspotAppContext Instance => lazy.Value;
        public event Action<MessagingNotificationType, MessageDto> ReceivedNotification;

        private MehspotAppContext ()
        {
        }

        public void Initialize (IApplicationDataStorage dataStorage) {
            AuthManager = new AuthenticationService (dataStorage);
            AuthManager.Authenticated += OnAuthenticated;
            var isAuthenticated = AuthManager.IsAuthenticated ();
            if (isAuthenticated) {
                RunSignalRAsync ();
            }
        }

        public AuthenticationService AuthManager { get; private set; }

        private async void RunSignalRAsync ()
        {
            hubConnection = new HubConnection (Mehspot.Core.Constants.ApiHost);
            hubConnection.Headers.Add ("Authorization", "Bearer " + AuthManager.AuthInfo.AccessToken);
            var messageNotificationHub = hubConnection.CreateHubProxy ("MessageNotificationHub");

            messageNotificationHub.On<MessagingNotificationType, MessageDto> ("OnSendNotification", OnSendNotification);
            // Start the connection
            await hubConnection.Start ();
        }

        private void OnSendNotification (MessagingNotificationType notificationType, MessageDto data)
        {
            if (ReceivedNotification != null) {
                ReceivedNotification (notificationType, data);
            }
        }

        private void OnAuthenticated (AuthenticationInfoDto authInfo)
        {
            if (hubConnection == null) {
                RunSignalRAsync ();
            }
        }
    }
}
