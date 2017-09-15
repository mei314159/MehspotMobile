using System;
using System.Linq;
using Facebook.CoreKit;
using Foundation;
using Google.Maps;
using HockeyApp.iOS;
using Mehspot.iOS.Core;
using Mehspot.iOS.Extensions;
using Mehspot.Core;
using Mehspot.Core.DTO.Push;
using Mehspot.Core.Push;
using Newtonsoft.Json;
using SDWebImage;
using UIKit;
using CoreGraphics;
using mehspot.iOS.Views;
using mehspot.iOS;

namespace Mehspot.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        private const string HockeyAppId = "939417b83e9b41b6bbfe772dd8129ac3";

        private const string MapsApiKey = "AIzaSyAqIup2dew1z_2_d1uTGcyArOboCWv2rN0";
        private const string PlacesApiKey = "AIzaSyBHCEcMfJQLhi_iTVhv1c_e_k_6wxDUCNY";

        // class-level declarations

        public override UIWindow Window
        {
            get;
            set;
        }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            MehspotAppContext.Instance.Initialize(new ApplicationDataStorage());
            MehspotAppContext.Instance.OnException += OnException;
            MehspotAppContext.Instance.OnNetworkException += Instance_OnNetworkException;
            InitializeHockeyApp();
            MapServices.ProvideAPIKey(MapsApiKey);
            PlacesClient.ProvideApiKey(PlacesApiKey);
            Profile.EnableUpdatesOnAccessTokenChange(true);
            SDWebImageManager.SharedManager.ImageDownloader.MaxConcurrentDownloads = 3;
            SDWebImageManager.SharedManager.ImageCache.ShouldCacheImagesInMemory = false;
            SDImageCache.SharedImageCache.ShouldCacheImagesInMemory = false;

            this.Window = new UIWindow(UIScreen.MainScreen.Bounds);
            this.Window.RootViewController = GetInitialViewController(launchOptions);
            this.Window.MakeKeyAndVisible();
            this.Window.BackgroundColor = UIColor.White;

            if (launchOptions != null)
            {
                var notification = (NSDictionary)launchOptions.ObjectForKey(UIApplication.LaunchOptionsRemoteNotificationKey);
                if (notification != null)
                {
                    ProcessNotification(notification);
                }
            }

            return ApplicationDelegate.SharedInstance.FinishedLaunching(application, launchOptions);
        }

        public override bool ContinueUserActivity(UIApplication application, NSUserActivity userActivity, UIApplicationRestorationHandler completionHandler)
        {
            if (userActivity.ActivityType == NSUserActivityType.BrowsingWeb && !MehspotAppContext.Instance.AuthManager.IsAuthenticated())
            {
                var url = userActivity.WebPageUrl.RelativePath.ToLower();

                if (url.StartsWith("/account/forgotpassword", StringComparison.Ordinal))
                {
                    var storyboard = UIStoryboard.FromName("Login", null);
                    var controller = storyboard.InstantiateViewController("ForgotPasswordViewController");
                    Window.SwapController(controller);
                    return true;
                }
                else if (url.StartsWith("/account/resetpassword", StringComparison.Ordinal))
                {
                    var controller = GetResetPasswordController(userActivity);
                    Window.SwapController(controller);
                    return true;
                }
            }

            return false;
        }

        private ResetPasswordViewController GetResetPasswordController(NSUserActivity userActivity)
        {
            var storyboard = UIStoryboard.FromName("Login", null);
            var controller = (ResetPasswordViewController)storyboard.InstantiateViewController("ResetPasswordViewController");
            var keyValueChunks = userActivity.WebPageUrl.Query.Split('&').Select(a => a.Split('=')).ToDictionary(a => a[0], a => a[1]);
            controller.Email = System.Net.WebUtility.UrlDecode(keyValueChunks["email"]);
            controller.Code = System.Net.WebUtility.UrlDecode(keyValueChunks["code"]);
            return controller;
        }

        public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
        {
            // We need to handle URLs by passing them to their own OpenUrl in order to make the SSO authentication works.
            return ApplicationDelegate.SharedInstance.OpenUrl(application, url, sourceApplication, annotation);
        }

        public override void OnResignActivation(UIApplication application)
        {
            // Invoked when the application is about to move from active to inactive state.
            // This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) 
            // or when the user quits the application and it begins the transition to the background state.
            // Games should use this method to pause the game.
        }

        public override void DidEnterBackground(UIApplication application)
        {
            // Use this method to release shared resources, save user data, invalidate timers and store the application state.
            // If your application supports background exection this method is called instead of WillTerminate when the user quits.
        }

        public override void WillEnterForeground(UIApplication application)
        {
            // Called as part of the transiton from background to active state.
            // Here you can undo many of the changes made on entering the background.
        }

        public override void OnActivated(UIApplication application)
        {
            // Restart any tasks that were paused (or not yet started) while the application was inactive. 
            // If the application was previously in the background, optionally refresh the user interface.
        }

        public override void WillTerminate(UIApplication application)
        {
            // Called when the application is about to terminate. Save data, if needed. See also DidEnterBackground.
        }

        public override async void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            // Get previous device token
            var oldDeviceToken = MehspotAppContext.Instance.DataStorage.PushToken;

            // Get current device token
            var newDeviceToken = deviceToken.Description;
            if (!string.IsNullOrWhiteSpace(newDeviceToken))
            {
                newDeviceToken = newDeviceToken.Trim('<').Trim('>').Replace(" ", string.Empty);

                // Has the token changed?
                if ((string.IsNullOrEmpty(oldDeviceToken) ||
                     !oldDeviceToken.Equals(newDeviceToken) ||
                     !MehspotAppContext.Instance.DataStorage.PushDeviceTokenSentToBackend))
                {

                    MehspotAppContext.Instance.DataStorage.PushDeviceTokenSentToBackend = false;
                    MehspotAppContext.Instance.DataStorage.OldPushToken = oldDeviceToken;
                    MehspotAppContext.Instance.DataStorage.PushToken = newDeviceToken;
                    if (MehspotAppContext.Instance.AuthManager.IsAuthenticated())
                        await SendPushTokenToServerAsync(oldDeviceToken, newDeviceToken);
                }
            }
        }

        public override void ReceivedRemoteNotification(UIApplication application, NSDictionary userInfo)
        {
            ProcessNotification(userInfo);
        }

        public static void CheckPushNotificationsPermissions()
        {
            if (MehspotAppContext.Instance.DataStorage.PushIsEnabled)
            {
                RegisterPushNotifications();
            }
            else
            {
                UIAlertView alert = new UIAlertView(
                                            "Enable push notifications",
                                            "Do you want to get notified about unread messages?",
                                            (IUIAlertViewDelegate)null,
                                            "Cancel",
                                            new string[] { "Yes, I do" });
                alert.Clicked += (object sender, UIButtonEventArgs e) =>
                {
                    MehspotAppContext.Instance.DataStorage.PushIsEnabled = true;
                    if (e.ButtonIndex != alert.CancelButtonIndex)
                    {
                        RegisterPushNotifications();
                    }
                };

                alert.Show();
            }
        }

        void ProcessNotification(NSDictionary userInfo)
        {
            NSError error = new NSError();
            var pushJson = new NSString(NSJsonSerialization.Serialize(userInfo, 0, out error), NSStringEncoding.UTF8).ToString();
            var notification = JsonConvert.DeserializeObject<PushNotification>(pushJson);

            UIApplication.SharedApplication.ApplicationIconBadgeNumber = notification.Data.Badge ?? 1;
            var tabBarController = Window.RootViewController as UITabBarController;
            if (tabBarController == null)
                return;

            foreach (var c in tabBarController.ViewControllers)
            {
                var rootController = c as UINavigationController;
                if (rootController == null)
                {
                    continue;
                }

                if (notification.NotificationType == NotificationTypeEnum.Message)
                {
                    if (OpenMessaging(notification, rootController, tabBarController))
                    {
                        break;
                    }
				}
                else if (notification.NotificationType == NotificationTypeEnum.GroupMessage)
				{
					if (OpenGroupMessaging(notification, rootController, tabBarController))
					{
						break;
					}
				}
            }
        }

        private bool OpenMessaging(PushNotification notification, UINavigationController rootController, UITabBarController tabBarController)
        {
            if (rootController.TopViewController is MessageBoardViewController)
            {
                var messageBoardViewController = ((MessageBoardViewController)rootController.TopViewController);

                if (tabBarController.SelectedViewController != rootController || !messageBoardViewController.IsViewLoaded)
                {
                    tabBarController.SelectedViewController = rootController;
                }

                if (rootController.VisibleViewController is MessagingViewController)
                {
                    var messagingViewController = ((MessagingViewController)rootController.VisibleViewController);
                    messagingViewController.ReloadAsync();
                    return true;
                }
                else
                {
                    messageBoardViewController.ShowMessagesFromUser(notification.FromUserId, notification.FromUserName);
                    return true;
                }
            }

            return false;
        }

		private bool OpenGroupMessaging(PushNotification notification, UINavigationController rootController, UITabBarController tabBarController)
		{
			if (rootController.TopViewController is GroupsListViewController)
			{
				var messageBoardViewController = ((GroupsListViewController)rootController.TopViewController);

				if (tabBarController.SelectedViewController != rootController || !messageBoardViewController.IsViewLoaded)
				{
					tabBarController.SelectedViewController = rootController;
				}

				if (rootController.VisibleViewController is GroupMessagingViewController)
				{
					var messagingViewController = ((GroupMessagingViewController)rootController.VisibleViewController);
					messagingViewController.ReloadAsync();
					return true;
				}
				else
				{
                    messageBoardViewController.ShowMessagesFromGroup(notification.GroupMessage);
					return true;
				}
			}

			return false;
		}

        private static void RegisterPushNotifications()
        {
            var pushSettings = UIUserNotificationSettings
                                        .GetSettingsForTypes(
                                            UIUserNotificationType.Alert |
                                            UIUserNotificationType.Badge |
                                            UIUserNotificationType.Sound, new NSSet());

            UIApplication.SharedApplication.RegisterUserNotificationSettings(pushSettings);
            UIApplication.SharedApplication.RegisterForRemoteNotifications();
        }

        private async System.Threading.Tasks.Task SendPushTokenToServerAsync(string oldToken, string newToken)
        {
            var pushService = new PushService(MehspotAppContext.Instance.DataStorage);
            var result = await pushService.RegisterAsync(oldToken, newToken);
            MehspotAppContext.Instance.DataStorage.PushDeviceTokenSentToBackend = result.IsSuccess;
        }

        private void InitializeHockeyApp()
        {
            var manager = BITHockeyManager.SharedHockeyManager;
            manager.Configure(HockeyAppId);
            manager.CrashManager.EnableAppNotTerminatingCleanlyDetection = true;
            manager.DebugLogEnabled = true;
            manager.StartManager();
            manager.Authenticator.AuthenticateInstallation();
        }

        private UIViewController GetInitialViewController(NSDictionary launchOptions)
        {
            //return UIStoryboard.FromName("Walkthrough", null).InstantiateInitialViewController();
            if (!MehspotAppContext.Instance.AuthManager.IsAuthenticated())
            {
                if (launchOptions != null)
                {
                    var dict = launchOptions.ObjectForKey(UIApplication.LaunchOptionsUserActivityDictionaryKey) as NSDictionary;
                    if (dict != null)
                    {
                        var userActivity = dict.ObjectForKey(new NSString("UIApplicationLaunchOptionsUserActivityKey")) as NSUserActivity;
                        if (userActivity != null)
                        {

                            var controller = GetResetPasswordController(userActivity);
                            return controller;
                        }
                    }
                }

                return UIStoryboard.FromName("Login", null).InstantiateViewController("LoginViewController");
            }

            if (!MehspotAppContext.Instance.DataStorage.WalkthroughPassed)
            {
                return UIStoryboard.FromName("Walkthrough", null).InstantiateInitialViewController();
            }

            return UIStoryboard.FromName("Main", null).InstantiateInitialViewController();
        }

        void OnException(Exception exception)
        {
            Console.WriteLine($"Exception: {exception.Message} + {Environment.NewLine} + {exception.StackTrace}");
        }

        void Instance_OnNetworkException(Exception ex)
        {
            InvokeOnMainThread(() =>
            {
                var topController = UIApplication.SharedApplication.KeyWindow.RootViewController;

                while (topController.PresentedViewController != null)
                {
                    topController = topController.PresentedViewController;
                }

                var y = topController.PrefersStatusBarHidden() ? 0 : 20;
                ErrorView.ShowInView(topController.View, new CGPoint(0, y), "No Internet Connection");
            });
        }
    }
}

