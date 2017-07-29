using System;
using System.Threading.Tasks;
using Mehspot.Core.Auth;
using Mehspot.Core.Contracts;
using Mehspot.Core.DTO;
using Mehspot.Core.Messaging;
using Microsoft.AspNet.SignalR.Client;

namespace Mehspot.Core
{
    public sealed class MehspotAppContext
    {
        private static readonly Lazy<MehspotAppContext> lazy = new Lazy<MehspotAppContext>(() => new MehspotAppContext());
        private HubConnection connection;
        private IHubProxy proxy;
        public static MehspotAppContext Instance => lazy.Value;
        public event Action<MessagingNotificationType, MessageDto> ReceivedNotification;

        private MehspotAppContext()
        {
        }

        public void Initialize(IApplicationDataStorage dataStorage)
        {
            DataStorage = dataStorage;
            AuthManager = new AccountService(dataStorage);
            AuthManager.Authenticated += OnAuthenticated;
            var isAuthenticated = AuthManager.IsAuthenticated();
            if (isAuthenticated)
            {
                Task.Factory.StartNew(async () =>
                {
                    await RunSignalRAsync(true);
                });
            }
        }

        public IApplicationDataStorage DataStorage { get; private set; }

        public event Action<Exception> OnException;

        public AccountService AuthManager { get; private set; }

        public void DisconnectSignalR()
        {
            if (connection != null)
            {
                connection.StateChanged -= HubConnection_StateChanged;
                connection.Error -= HubConnection_Error;
                connection = null;
                proxy = null;
            }
        }

        private async Task RunSignalRAsync(bool dismissCurrentConnection = false)
        {
            if (connection != null && !dismissCurrentConnection)
            {
                return;
            }


            try
            {
                DisconnectSignalR();
                connection = new HubConnection(Constants.ApiHost);
                connection.TraceLevel = TraceLevels.All;
                connection.Headers.Add("Authorization", "Bearer " + AuthManager.AuthInfo.AccessToken);
                connection.StateChanged += HubConnection_StateChanged;
                connection.Error += HubConnection_Error;

                proxy = connection.CreateHubProxy("MessageNotificationHub");
                proxy.On<MessagingNotificationType, MessageDto>("OnSendNotification", OnSendNotification);
                await connection.Start();
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }

        public void LogException(Exception ex)
        {
            OnException?.Invoke(ex);
        }

        void HubConnection_StateChanged(StateChange obj)
        {
            if (obj.NewState == ConnectionState.Disconnected)
            {
                Task.Factory.StartNew(async () =>
                {
                    await RunSignalRAsync(true);
                });
            }
        }

        void HubConnection_Error(Exception ex)
        {
            LogException(ex);
        }

        private void OnSendNotification(MessagingNotificationType notificationType, MessageDto data)
        {
            if (ReceivedNotification != null)
            {
                ReceivedNotification(notificationType, data);
            }
        }

        private void OnAuthenticated(AuthenticationInfoDTO authInfo)
        {
            if (connection == null)
            {
                Task.Factory.StartNew(async () =>
                {
                    await RunSignalRAsync(true);
                });
            }
        }
    }
}
