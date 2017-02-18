using System;
using System.Threading.Tasks;
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
        private HubConnection connection;
        private IHubProxy proxy;
        public static MehspotAppContext Instance => lazy.Value;
        public event Action<MessagingNotificationType, MessageDto> ReceivedNotification;
        public event Action<Exception> HubError;

        private MehspotAppContext ()
        {
        }

        public void Initialize (IApplicationDataStorage dataStorage)
        {
            AuthManager = new AuthenticationService (dataStorage);
            AuthManager.Authenticated += OnAuthenticated;
            var isAuthenticated = AuthManager.IsAuthenticated ();
            if (isAuthenticated) {
                Task.Factory.StartNew (async () => {
                    await RunSignalRAsync (true);
                });
            }
        }

        public AuthenticationService AuthManager { get; private set; }

        private async Task RunSignalRAsync (bool dismissCurrentConnection = false)
        {
            if (connection != null) {
                if (!dismissCurrentConnection) {
                    return;
                }

                connection.StateChanged -= HubConnection_StateChanged;
                connection.Error -= HubConnection_Error;
                connection = null;
                proxy = null;
            }

            connection = new HubConnection (Constants.ApiHost);
            connection.TraceLevel = TraceLevels.All;
            connection.Headers.Add ("Authorization", "Bearer " + AuthManager.AuthInfo.AccessToken);
            connection.StateChanged += HubConnection_StateChanged;
            connection.Error += HubConnection_Error;

            proxy = connection.CreateHubProxy ("MessageNotificationHub");
            proxy.On<MessagingNotificationType, MessageDto> ("OnSendNotification", OnSendNotification);
            await connection.Start ();
        }

        void HubConnection_StateChanged (StateChange obj)
        {
            if (obj.NewState == ConnectionState.Disconnected) { 
                Task.Factory.StartNew (async () => {
                    await RunSignalRAsync (true);
                });
            }
        }

        void HubConnection_Error (Exception ex)
        {
            OnHubError (ex);
        }

        private void OnSendNotification (MessagingNotificationType notificationType, MessageDto data)
        {
            if (ReceivedNotification != null) {
                ReceivedNotification (notificationType, data);
            }
        }

        private void OnHubError (Exception ex)
        {
            if (HubError != null) {
                HubError (ex);
            }
        }

        private void OnAuthenticated (AuthenticationInfoDto authInfo)
        {
            if (connection == null) {
                Task.Factory.StartNew (async () => {
                    await RunSignalRAsync (true);
                });
            }
        }
    }
}
