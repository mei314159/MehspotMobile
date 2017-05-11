using Android.App;
using Android.Content;
using Android.Util;
using Firebase.Iid;

namespace Mehspot.Android
{

    [Service]
    [IntentFilter (new [] { "com.google.firebase.INSTANCE_ID_EVENT" })]
    public class MyFirebaseIIDService : FirebaseInstanceIdService
    {
        const string TAG = "MyFirebaseIIDService";
        public override void OnTokenRefresh ()
        {
            var refreshedToken = FirebaseInstanceId.Instance.Token;
            Log.Debug (TAG, "Refreshed token: " + refreshedToken);
        }
    }

    //[Service (Exported = false)]
    //class RegistrationIntentService : IntentService
    //{
    //    static object locker = new object ();
    //    public RegistrationIntentService () : base ("RegistrationIntentService")
    //    {

    //    }

    //    protected override void OnHandleIntent (Intent intent)
    //    {
    //        try {
    //            Log.Info ("RegistrationIntentService", "Calling InstanceID.GetToken");
    //            lock (locker) {
    //                var instanceID = InstanceID.GetInstance (this);
    //                var token = instanceID.GetToken (Mehspot.Android.Constants.SenderId, GoogleCloudMessaging.InstanceIdScope, null);

    //                Log.Info ("RegistrationIntentService", $"GCM Registration Token: {token}");
    //                SendRegistrationToAppServer (token);
    //                Subscribe (token);
    //            }
    //        } catch (Exception ex) {
    //            Log.Debug ("RegistrationIntentService", "Failed to get a registration token");
    //            return;
    //        }
    //    }

    //    async void SendRegistrationToAppServer (string token)
    //    {
    //        var oldDeviceToken = MehspotAppContext.Instance.DataStorage.PushToken;

    //        if ((string.IsNullOrEmpty (oldDeviceToken) ||
    //             !oldDeviceToken.Equals (token) ||
    //             !MehspotAppContext.Instance.DataStorage.PushDeviceTokenSentToBackend)) {

    //            MehspotAppContext.Instance.DataStorage.PushDeviceTokenSentToBackend = false;
    //            MehspotAppContext.Instance.DataStorage.OldPushToken = oldDeviceToken;
    //            MehspotAppContext.Instance.DataStorage.PushToken = token;
    //            if (MehspotAppContext.Instance.AuthManager.IsAuthenticated ())
    //                await SendPushTokenToServerAsync (oldDeviceToken, token);
    //        }
    //    }

    //    private async System.Threading.Tasks.Task SendPushTokenToServerAsync (string oldToken, string newToken)
    //    {
    //        var pushService = new PushService (MehspotAppContext.Instance.DataStorage);
    //        var result = await pushService.RegisterAsync (oldToken, newToken);
    //        MehspotAppContext.Instance.DataStorage.PushDeviceTokenSentToBackend = result.IsSuccess;
    //    }

    //    void Subscribe (string token)
    //    {
    //        var pubSub = GcmPubSub.GetInstance (this);
    //        pubSub.Subscribe (token, "/topics/global", null);
    //    }
    //}

    //[Service (Exported = false), IntentFilter (new [] { "com.google.android.gms.iid.InstanceID" })]
    //class MyInstanceIDListenerService : InstanceIDListenerService
    //{
    //    public override void OnTokenRefresh ()
    //    {
    //        var intent = new Intent (this, typeof (RegistrationIntentService));
    //        StartService (intent);
    //    }
    //}

    //[Service (Exported = false), IntentFilter (new [] { "com.google.android.c2dm.intent.RECEIVE" })]
    //public class MyGcmListenerService : GcmListenerService
    //{
    //    public MyGcmListenerService (IntPtr javaReference, JniHandleOwnership transfer) : base (javaReference, transfer)
    //    {
    //    }

    //    public MyGcmListenerService ()
    //    {
    //    }

    //    public override void OnMessageReceived (string from, Bundle data)
    //    {
    //        var message = data.GetString ("message");
    //        Log.Debug ("MyGcmListenerService", "From:    " + from);
    //        Log.Debug ("MyGcmListenerService", "Message: " + message);
    //        SendNotification (message);
    //    }

    //    void SendNotification (string message)
    //    {
    //        var intent = new Intent (this, typeof (SplashScreen));
    //        intent.AddFlags (ActivityFlags.ClearTop);
    //        var pendingIntent = PendingIntent.GetActivity (this, 0, intent, PendingIntentFlags.OneShot);

    //        var notificationBuilder = new Notification.Builder (this)
    //            .SetSmallIcon (Resource.Mipmap.ic_launcher)
    //            .SetContentTitle ("GCM Message")
    //            .SetContentText (message)
    //            .SetAutoCancel (true)
    //            .SetContentIntent (pendingIntent);

    //        var notificationManager = (NotificationManager)GetSystemService (Context.NotificationService);
    //        notificationManager.Notify (0, notificationBuilder.Build ());
    //    }
    //}
}