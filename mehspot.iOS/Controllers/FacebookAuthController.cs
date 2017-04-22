using Foundation;
using System;
using UIKit;
using System.Linq;
using Mehspot.Core;
using mehspot.Core.Auth;
using mehspot.iOS.Extensions;
using Mehspot.Core.Models;
using mehspot.iOS.Wrappers;
using Mehspot.Core.Contracts.ViewControllers;
using Mehspot.Core.Contracts.Wrappers;

namespace mehspot.iOS
{
    public partial class FacebookAuthController : UIViewController, IExternalSignInController
    {
        private ExternalSignInModel model;

        public IViewHelper ViewHelper { get; private set; }

        public FacebookAuthController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad ()
        {
            this.ViewHelper = new ViewHelper (this.WebView);
            model = new ExternalSignInModel (new AccountService (MehspotAppContext.Instance.DataStorage), this);
            model.SignedIn += Model_SignedIn;
            WebView.ScalesPageToFit = true;
            WebView.ShouldStartLoad += this.WebView_ShouldStartLoad;
            WebView.LoadStarted += WebView_LoadStarted;
            WebView.LoadFinished += WebView_LoadFinished;
        }

        public override void ViewDidAppear (bool animated)
        {
            model.BeginExternalSignIn ();
        }

        public void LoadExternalSignInPage ()
        {
            foreach (var cookie in NSHttpCookieStorage.SharedStorage.Cookies) {
                if (cookie.Domain == ".mehspot.com") {
                    NSHttpCookieStorage.SharedStorage.DeleteCookie (cookie);
                }
            }
            var url = Constants.ApiHost + "/Account/ExternalLogin?provider=Facebook&ReturnUrl=mobile";
            WebView.LoadRequest (new NSUrlRequest (new NSUrl (url)));
        }

        bool WebView_ShouldStartLoad (UIWebView webView, NSUrlRequest request, UIWebViewNavigationType navigationType)
        {
            if (request.Url.ToString ().StartsWith (Constants.ApiHost + "/Account/mobile", StringComparison.InvariantCultureIgnoreCase)) {
                var queryParameters = request.Url.Query.Split ('&').Select (a => a.Split ('=')).ToDictionary (a => a [0].ToLower (), a => a [1]);

                model.SignInExternalAsync (queryParameters ["externalaccesstoken"], queryParameters ["provider"]);
                return false;
            }

            return true;
        }

        void WebView_LoadStarted (object sender, EventArgs e)
        {
            ViewHelper.ShowOverlay ("Wait...");
        }

        void WebView_LoadFinished (object sender, EventArgs e)
        {
            ViewHelper.HideOverlay ();
        }

        void Model_SignedIn (AuthenticationResult obj)
        {
            var targetViewController = UIStoryboard.FromName ("Main", null).InstantiateInitialViewController ();
            this.View.Window.SwapController (targetViewController);
        }
    }
}