using Foundation;
using System;
using UIKit;
using System.Linq;
using Mehspot.Core;
using mehspot.Core.Auth;
using mehspot.iOS.Extensions;
using Mehspot.Core.Models;
using mehspot.iOS.Wrappers;
namespace mehspot.iOS
{
    public partial class FacebookAuthController : UIViewController
    {
        private readonly AccountService accountService;
        SignInModel model;
        public FacebookAuthController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad ()
        {
            model = new SignInModel (new AccountService (MehspotAppContext.Instance.DataStorage), new ViewHelper (this.View));
            model.SignedIn += Model_SignedIn;
            WebView.ScalesPageToFit = true;
            WebView.ShouldStartLoad += this.WebView_ShouldStartLoad;
            var url = Constants.ApiHost + "/Account/ExternalLogin?provider=Facebook&ReturnUrl=mobile";
            WebView.LoadRequest (new NSUrlRequest (new NSUrl (url)));
        }

        public override void ViewDidAppear (bool animated)
        {

        }

        bool WebView_ShouldStartLoad (UIWebView webView, NSUrlRequest request, UIWebViewNavigationType navigationType)
        {
            if (request.Url.ToString ().StartsWith (Constants.ApiHost + "/Account/mobile", StringComparison.InvariantCultureIgnoreCase)) {
                var queryParameters = request.Url.Query.Split ('&').Select (a => a.Split ('=')).ToDictionary (a => a [0].ToLower (), a=> a [1]);

                model.SignInExternalAsync (queryParameters ["externalaccesstoken"], queryParameters ["provider"]);
                return false;
            }

            return true;
        }

        void Model_SignedIn (AuthenticationResult obj)
        {
            var targetViewController = UIStoryboard.FromName ("Main", null).InstantiateInitialViewController ();
            this.View.Window.SwapController (targetViewController);
        }
    }
}