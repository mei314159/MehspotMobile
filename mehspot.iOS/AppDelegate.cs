﻿using System;
using Foundation;
using HockeyApp.iOS;
using mehspot.Core.Auth;
using mehspot.iOS.Core;
using Mehspot.Core.DTO;
using Mehspot.Core.Messaging;
using Microsoft.AspNet.SignalR.Client;
using UIKit;

namespace mehspot.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register ("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        private HubConnection hubConnection;
        public static AuthenticationService AuthManager = new AuthenticationService (new ApplicationDataStorage ());

        // class-level declarations

        public override UIWindow Window {
            get;
            set;
        }

        public static event Action<MessagingNotificationType, MessageDto> ReceivedNotification;

        public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
        {
            AuthManager.Authenticated += OnAuthenticated;
            
            InitializeHockeyApp ();

            var isAuthenticated = AuthManager.IsAuthenticated ();
            if (isAuthenticated) {
                RunSignalRAsync ();
            }

            // Override point for customization after application launch.
            // If not required for your application you can safely delete this method
            this.Window = new UIWindow (UIScreen.MainScreen.Bounds);
            this.Window.RootViewController = GetInitialViewController (isAuthenticated);
            this.Window.MakeKeyAndVisible ();
            this.Window.BackgroundColor = UIColor.White;
            return true;
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

        private void OnAuthenticated (AuthenticationInfoDto authInfo)
        {
            if (hubConnection == null) {
                RunSignalRAsync ();
            }
        }

        private void InitializeHockeyApp ()
        {
            var manager = BITHockeyManager.SharedHockeyManager;
            manager.Configure ("939417b83e9b41b6bbfe772dd8129ac3");
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

        private async void RunSignalRAsync ()
        {
            hubConnection = new HubConnection (Mehspot.Core.Constants.ApiHost);
            hubConnection.Headers.Add ("Authorization", "Bearer " + AuthManager.AuthInfo.AccessToken);
            var messageNotificationHub = hubConnection.CreateHubProxy ("MessageNotificationHub");

            messageNotificationHub.On<MessagingNotificationType, MessageDto> ("OnSendNotification", OnSendNotification);
            // Start the connection
            await hubConnection.Start ();
        }

        void OnSendNotification (MessagingNotificationType notificationType, MessageDto data)
        {
            if (ReceivedNotification != null) {
                ReceivedNotification (notificationType, data);
            }
        }
    }
}

