using System;
using System.Threading.Tasks;
using Facebook.CoreKit;
using Foundation;
using Google.Maps;
using HockeyApp.iOS;
using mehspot.iOS.Core;
using mehspot.iOS.Core.DTO;
using Mehspot.Core;
using Mehspot.Core.Push;
using Newtonsoft.Json;
using SDWebImage;
using UIKit;

namespace mehspot.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register ("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        private const string HockeyAppId = "939417b83e9b41b6bbfe772dd8129ac3";

        private const string MapsApiKey = "AIzaSyAqIup2dew1z_2_d1uTGcyArOboCWv2rN0";

        // class-level declarations

        public override UIWindow Window {
            get;
            set;
        }

        public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
        {
            MehspotAppContext.Instance.Initialize (new ApplicationDataStorage ());
            MehspotAppContext.Instance.OnException += OnException;
            InitializeHockeyApp ();
            MapServices.ProvideAPIKey (MapsApiKey);

            Profile.EnableUpdatesOnAccessTokenChange (true);


            // Override point for customization after application launch.
            // If not required for your application you can safely delete this method
            this.Window = new UIWindow (UIScreen.MainScreen.Bounds);
            this.Window.RootViewController = GetInitialViewController (MehspotAppContext.Instance.AuthManager.IsAuthenticated ());
            this.Window.MakeKeyAndVisible ();
            this.Window.BackgroundColor = UIColor.White;

            SDWebImageManager.SharedManager.ImageDownloader.MaxConcurrentDownloads = 3;
            SDWebImageManager.SharedManager.ImageCache.ShouldCacheImagesInMemory = false;
            SDImageCache.SharedImageCache.ShouldCacheImagesInMemory = false;
            if (launchOptions != null) {
                var notification = (NSDictionary)launchOptions.ObjectForKey (UIApplication.LaunchOptionsRemoteNotificationKey);
                if (notification != null) {
                    ProcessNotification (notification);
                }
            }

            return ApplicationDelegate.SharedInstance.FinishedLaunching (application, launchOptions);
        }

        public override bool OpenUrl (UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
        {
            // We need to handle URLs by passing them to their own OpenUrl in order to make the SSO authentication works.
            return ApplicationDelegate.SharedInstance.OpenUrl (application, url, sourceApplication, annotation);
        }


        public override void OnResignActivation (UIApplication application)
        {
            // Invoked when the application is about to move from active to inactive state.
            // This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) 
            // or when the user quits the application and it begins the transition to the background state.
            // Games should use this method to pause the game.
        }

        public override void DidEnterBackground (UIApplication application)
        {
            // Use this method to release shared resources, save user data, invalidate timers and store the application state.
            // If your application supports background exection this method is called instead of WillTerminate when the user quits.
        }

        public override void WillEnterForeground (UIApplication application)
        {
            // Called as part of the transiton from background to active state.
            // Here you can undo many of the changes made on entering the background.
        }

        public override void OnActivated (UIApplication application)
        {
            // Restart any tasks that were paused (or not yet started) while the application was inactive. 
            // If the application was previously in the background, optionally refresh the user interface.
        }

        public override void WillTerminate (UIApplication application)
        {
            // Called when the application is about to terminate. Save data, if needed. See also DidEnterBackground.
        }

        public override async void RegisteredForRemoteNotifications (UIApplication application, NSData deviceToken)
        {
            // Get previous device token
            var oldDeviceToken = MehspotAppContext.Instance.DataStorage.PushToken;

            // Get current device token
            var newDeviceToken = deviceToken.Description;
            if (!string.IsNullOrWhiteSpace (newDeviceToken)) {
                newDeviceToken = newDeviceToken.Trim ('<').Trim ('>').Replace (" ", string.Empty);

                // Has the token changed?
                if ((string.IsNullOrEmpty (oldDeviceToken) ||
                     !oldDeviceToken.Equals (newDeviceToken) ||
                     !MehspotAppContext.Instance.DataStorage.PushDeviceTokenSentToBackend)) {

                    MehspotAppContext.Instance.DataStorage.PushDeviceTokenSentToBackend = false;
                    MehspotAppContext.Instance.DataStorage.OldPushToken = oldDeviceToken;
                    MehspotAppContext.Instance.DataStorage.PushToken = newDeviceToken;
                    if (MehspotAppContext.Instance.AuthManager.IsAuthenticated ())
                        await SendPushTokenToServerAsync (oldDeviceToken, newDeviceToken);
                }
            }
        }

        public override void ReceivedRemoteNotification (UIApplication application, NSDictionary userInfo)
        {
            ProcessNotification (userInfo);
        }

        public static void CheckPushNotificationsPermissions ()
        {
            if (MehspotAppContext.Instance.DataStorage.PushIsEnabled) {
                RegisterPushNotifications ();
            } else {
                UIAlertView alert = new UIAlertView (
                                            "Enable push notifications",
                                            "Do you want to get notified about unread messages?",
                                            null,
                                            "Cancel",
                                            new string [] { "Yes, I do" });
                alert.Clicked += (object sender, UIButtonEventArgs e) => {
                    MehspotAppContext.Instance.DataStorage.PushIsEnabled = true;
                    if (e.ButtonIndex != alert.CancelButtonIndex) {
                        RegisterPushNotifications ();
                    }
                };

                alert.Show ();
            }
        }

        void ProcessNotification (NSDictionary userInfo)
        {
            NSError error = new NSError ();
            var pushJson = new NSString (NSJsonSerialization.Serialize (userInfo, 0, out error), NSStringEncoding.UTF8).ToString ();
            var notification = JsonConvert.DeserializeObject<IosNotification> (pushJson);

            UIApplication.SharedApplication.ApplicationIconBadgeNumber = notification.Data.Badge ?? 1;
            var controller = Window.RootViewController as UITabBarController;
            if (controller == null)
                return;

            foreach (var c in controller.ViewControllers) {
                var rootController = c as UINavigationController;
                if (rootController == null) {
                    continue;
                }

                if (rootController.TopViewController is MessageBoardViewController) {
                    var messageBoardViewController = ((MessageBoardViewController)rootController.TopViewController);

                    if (controller.SelectedViewController != rootController || !messageBoardViewController.IsViewLoaded) {
                        controller.SelectedViewController = rootController;

                        messageBoardViewController.ShowMessagesFromUser (notification.FromUserId, notification.FromUserName);
                        break;
                    }
                }
            }
        }

        private static void RegisterPushNotifications ()
        {
            var pushSettings = UIUserNotificationSettings
                                        .GetSettingsForTypes (
                                            UIUserNotificationType.Alert |
                                            UIUserNotificationType.Badge |
                                            UIUserNotificationType.Sound, new NSSet ());

            UIApplication.SharedApplication.RegisterUserNotificationSettings (pushSettings);
            UIApplication.SharedApplication.RegisterForRemoteNotifications ();
        }

        private async System.Threading.Tasks.Task SendPushTokenToServerAsync (string oldToken, string newToken)
        {
            var pushService = new PushService (MehspotAppContext.Instance.DataStorage);
            var result = await pushService.RegisterAsync (oldToken, newToken);
            MehspotAppContext.Instance.DataStorage.PushDeviceTokenSentToBackend = result.IsSuccess;
        }

        private void InitializeHockeyApp ()
        {
            var manager = BITHockeyManager.SharedHockeyManager;
            manager.Configure (HockeyAppId);
            manager.CrashManager.EnableAppNotTerminatingCleanlyDetection = true;
            manager.DebugLogEnabled = true;
            manager.StartManager ();
            manager.Authenticator.AuthenticateInstallation ();
        }

        private UIViewController GetInitialViewController (bool isAuthenticated)
        {
            var storyboard = UIStoryboard.FromName ("Main", null);
            if (!isAuthenticated) {
                return storyboard.InstantiateViewController ("LoginViewController");
            }

            return storyboard.InstantiateInitialViewController ();
        }

        void OnException (Exception exception)
        {
            Console.WriteLine ($"Exception: {exception.Message} + {Environment.NewLine} + {exception.StackTrace}");
        }
    }
}

