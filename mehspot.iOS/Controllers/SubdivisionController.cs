using System;
using Foundation;
using Google.Maps;
using Mehspot.DTO;
using UIKit;

namespace mehspot.iOS.Controllers
{
    public partial class SubdivisionController : UIViewController
    {
        MapView mapView;
        Marker marker;


        public event Action<SubdivisionDTO> OnDismissed;

        public SubdivisionController () : base ("SubdivisionController", NSBundle.MainBundle)
        {
        }

        public SubdivisionDTO Subdivision { get; set; }

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();
            LatitudeField.KeyboardType = LongitudeField.KeyboardType = UIKeyboardType.DecimalPad;

            this.NameField.Text = Subdivision.DisplayName;
            this.AddressField.Text = Subdivision.FormattedAddress;
            this.LatitudeField.Text = Subdivision.Latitude.ToString();
            this.LongitudeField.Text = Subdivision.Longitude.ToString ();

            CameraPosition camera;
            if (Subdivision == null) {
                camera = CameraPosition.FromCamera (37.79, 32.40, 6);
            } else {
                camera = CameraPosition.FromCamera (Subdivision.Latitude, Subdivision.Longitude, 15);
            }
            mapView = MapView.FromCamera (MapWrapperView.Bounds, camera);
            mapView.MyLocationEnabled = true;
            mapView.DraggingMarkerEnded += MapView_DraggingMarkerEnded;;

            marker = Marker.FromPosition (camera.Target);
            marker.Map = mapView;
            marker.Draggable = true;
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

        void MapView_DraggingMarkerEnded (object sender, GMSMarkerEventEventArgs e)
        {
            new Geocoder ().ReverseGeocodeCord (e.Marker.Position, HandleReverseGeocodeCallback);
        }

        void HandleReverseGeocodeCallback (ReverseGeocodeResponse response, NSError error)
        {
            if (error != null)
                return;

            this.AddressField.Text = response.FirstResult.Thoroughfare;
            this.LatitudeField.Text = response.FirstResult.Coordinate.Latitude.ToString();
            this.LongitudeField.Text = response.FirstResult.Coordinate.Longitude.ToString ();
        }


        partial void SaveButtonTouched (UIBarButtonItem sender)
        {
            Subdivision.DisplayName = this.NameField.Text;
            Subdivision.FormattedAddress = this.AddressField.Text;
            Subdivision.Latitude = double.Parse (this.LatitudeField.Text);
            Subdivision.Longitude = double.Parse (this.LongitudeField.Text);

            this.OnDismissed (Subdivision);
            DismissViewController (true, null);
        }

        partial void CloseButtonTouched (UIBarButtonItem sender)
        {
            DismissViewController (true, null);
        }
    }
}

