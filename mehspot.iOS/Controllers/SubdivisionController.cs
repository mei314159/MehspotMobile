using Foundation;
using Google.Maps;
using UIKit;

namespace mehspot.iOS.Controllers
{
    public partial class SubdivisionController : UIViewController
    {
        MapView mapView;

        public static readonly UINib Nib;

        static SubdivisionController ()
        {
            Nib = UINib.FromName ("SubdivisionController", NSBundle.MainBundle);
        }

        public SubdivisionController () : base ("SubdivisionController", null)
        {
        }

        public static SubdivisionController Create ()
        {
            return (SubdivisionController)Nib.Instantiate (null, null) [0];
        }

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();

            // Create a GMSCameraPosition that tells the map to display the
            // coordinate 37.79,-122.40 at zoom level 6.
            var camera = CameraPosition.FromCamera (latitude: 37.79,
                                                    longitude: -122.40,
                                                    zoom: 6);

            mapView = MapView.FromCamera (MapWrapperView.Bounds, camera);
            mapView.MyLocationEnabled = true;
            mapView.CameraPositionChanged += MapView_CameraPositionChanged;

            MapWrapperView.AddSubview (mapView);
        }

        public override void ViewWillAppear (bool animated)
        {
            base.ViewWillAppear (animated);
            mapView.StartRendering ();
        }
        public override void ViewWillDisappear (bool animated)
        {
            mapView.StopRendering ();
            base.ViewWillDisappear (animated);
        }

        public override void DidReceiveMemoryWarning ()
        {
            base.DidReceiveMemoryWarning ();
            // Release any cached data, images, etc that aren't in use.
        }

        partial void SaveButtonTouched (UIBarButtonItem sender)
        {
            
        }

        partial void CloseButtonTouched (UIBarButtonItem sender)
        {
            DismissViewController (true, null);
        }

        void MapView_CameraPositionChanged (object sender, GMSCameraEventArgs e)
        {

        }
    }
}

