using System;
using CoreLocation;
using Foundation;
using Google.Maps;
using mehspot.iOS.Wrappers;
using Mehspot.Core;
using Mehspot.Core.DTO;
using Mehspot.Core.Services;
using MehSpot.Core.DTO.Subdivision;
using UIKit;

namespace mehspot.iOS.Controllers
{
    public partial class SubdivisionController : UIViewController
    {
        MapView mapView;
        Marker marker;
        Address Place;
        Geocoder geocoder = new Geocoder ();
        SubdivisionService subdivisionService;
        ViewHelper viewHelper;
        private bool setPlaceOnly;

        public event Action<EditSubdivisionDTO> OnDismissed;

        public SubdivisionController () : base ("SubdivisionController", NSBundle.MainBundle)
        {
        }

        public SubdivisionDTO Subdivision { get; set; }
        public string ZipCode { get; set; }

        public bool AllowEdititng { get; internal set; }


        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();
            viewHelper = new ViewHelper (this.View);
            subdivisionService = new SubdivisionService (MehspotAppContext.Instance.DataStorage);
            LatitudeField.KeyboardType = LongitudeField.KeyboardType = UIKeyboardType.DecimalPad;

            CameraPosition camera;
            if (Subdivision == null) {
                camera = CameraPosition.FromCamera (33.7489954, -84.3879824, 15);
            } else {
                setPlaceOnly = true;
                camera = CameraPosition.FromCamera (Subdivision.Latitude, Subdivision.Longitude, 15);
                this.NameField.Text = Subdivision.DisplayName;
                this.AddressField.Text = Subdivision.FormattedAddress;
                this.LatitudeField.Text = Subdivision.Latitude.ToString ();
                this.LongitudeField.Text = Subdivision.Longitude.ToString ();
            }

            GetLocationByCoordinates (camera.Target);

            mapView = MapView.FromCamera (MapWrapperView.Bounds, camera);
            mapView.MyLocationEnabled = true;
            mapView.DraggingMarkerStarted += MapView_DraggingMarkerStarted;;
            mapView.DraggingMarkerEnded += MapView_DraggingMarkerEnded ;

            marker = Marker.FromPosition (camera.Target);
            marker.Map = mapView;
            marker.Draggable = true;

            if (!AllowEdititng)
                this.NavBarItem.RightBarButtonItems = new UIBarButtonItem [] { };
            this.NameField.Enabled = this.AddressField.Enabled = LatitudeField.Enabled = LongitudeField.Enabled = marker.Draggable = AllowEdititng;

            MapWrapperView.AddSubview (mapView);
        }

        void MapView_DraggingMarkerStarted (object sender, GMSMarkerEventEventArgs e)
        {
            this.AddressField.Enabled = LatitudeField.Enabled = LongitudeField.Enabled = false;
        }

        void MapView_DraggingMarkerEnded (object sender, GMSMarkerEventEventArgs e)
        {
            viewHelper.ShowOverlay ("Wait...");
            GetLocationByCoordinates (e.Marker.Position);
        }

        void HandleReverseGeocodeCallback (ReverseGeocodeResponse response, NSError error)
        {
            this.AddressField.Enabled = LatitudeField.Enabled = LongitudeField.Enabled = AllowEdititng;

            if (error != null)
                return;

            this.Place = response.FirstResult;
            if (!setPlaceOnly) {
                this.NameField.Text = this.Place.SubLocality;
                this.AddressField.Text = Place.Lines != null ? string.Join (", ", response.FirstResult.Lines) : response.FirstResult.Thoroughfare;
                this.LatitudeField.Text = response.FirstResult.Coordinate.Latitude.ToString ();
                this.LongitudeField.Text = response.FirstResult.Coordinate.Longitude.ToString ();
            }

            setPlaceOnly = false;
            viewHelper.HideOverlay ();
        }


        async partial void SaveButtonTouched (UIBarButtonItem sender)
        {
            viewHelper.ShowOverlay ("Saving...");

            var zip = this.ZipCode ?? Place.PostalCode;
            var dto = new EditSubdivisionDTO {
                Id = Subdivision?.Id ?? default(int),
                Name = this.NameField.Text,
                OptionId = Subdivision?.OptionId,
                ZipCode = zip,
                Address = new AddressDTO {
                    Latitude = Place.Coordinate.Latitude,
                    Longitude = Place.Coordinate.Longitude,
                    Country = Place.Country,
                    FormattedAddress = this.AddressField.Text,
                    PostalCode = zip,
                    GoverningDistrictId = 1,
                }
            };

            Result result;
            if (Subdivision != null) {
                var overrideResult = await this.subdivisionService.OverrideAsync (dto);
                dto.Id = overrideResult.Data.Id;
                result = overrideResult;
            } else {
                var createResult = await this.subdivisionService.CreateAsync (dto);
                dto.Id = createResult.Data.SubdivisionId;
                dto.OptionId = createResult.Data.OptionId;
                result = createResult;
            }

            viewHelper.HideOverlay ();
            if (result.IsSuccess) {
                DismissViewController (true, null);
                this.OnDismissed?.Invoke (dto);
            } else {
                viewHelper.ShowAlert ("Error", result.ErrorMessage);
            }
        }

        partial void CloseButtonTouched (UIBarButtonItem sender)
        {
            DismissViewController (true, null);
        }

        void GetLocationByCoordinates (CLLocationCoordinate2D position)
        {
            geocoder.ReverseGeocodeCord (position, HandleReverseGeocodeCallback);
        }
    }
}