using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Location.Places.UI;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Mehspot.AndroidApp.Wrappers;
using Mehspot.Core;
using Mehspot.Core.Contracts.ViewControllers;
using Mehspot.Core.Contracts.Wrappers;
using Mehspot.Core.DTO.Subdivision;
using Mehspot.Core.Models.Subdivisions;
using Mehspot.Core.Services;

namespace Mehspot.AndroidApp.Activities
{
	[Activity(Label = "Subdivision Activity")]
	public class SubdivisionActivity : AppCompatActivity, ISubdivisionController, IOnMapReadyCallback, ILocationListener,
	Android.Support.V7.Widget.Toolbar.IOnMenuItemClickListener, GoogleApiClient.IOnConnectionFailedListener
	{
		private const long MinTime = 0;
		private const float MinDistance = 0;

		private int PLACE_AUTOCOMPLETE_REQUEST_CODE = 1;
		private static readonly string TAG = "X:" + nameof(SubdivisionsListActivity);
		private SubdivisionModel model;
		private Marker marker;
		private GoogleMap map;
		private Geocoder geocoder;
		private CameraPosition camera;
		private LocationManager locationManager;
		private string locationProvider;
		private SetPositionDelegate locationDetected;
		private Action locationDetectionError;

		public Action<EditSubdivisionDTO> OnDismissed => Intent.GetExtra<Action<EditSubdivisionDTO>>("onDismissed");
		public Android.Support.V7.Widget.Toolbar Toolbar => FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.SubdivisionActivity.Menu);

		public IViewHelper ViewHelper { get; set; }
		public SubdivisionDTO Subdivision => Intent.GetExtra<SubdivisionDTO>("subdivision");
		public string ZipCode => Intent.GetStringExtra("zipCode");
		public bool AllowEdititng => Intent.GetBooleanExtra("allowEdititng", false);

		public EditText NameField => FindViewById<EditText>(Resource.SubdivisionActivity.NameField);
		public EditText AddressField => FindViewById<EditText>(Resource.SubdivisionActivity.AddressField);
		public EditText LatitudeField => FindViewById<EditText>(Resource.SubdivisionActivity.LatitudeField);
		public EditText LongitudeField => FindViewById<EditText>(Resource.SubdivisionActivity.LongitudeField);

		#region Properties
		public string NameFieldText
		{
			get
			{
				return this.NameField.Text;
			}

			set
			{
				this.NameField.Text = value;
			}
		}

		public string AddressFieldText
		{
			get
			{
				return this.AddressField.Text;
			}

			set
			{
				this.AddressField.Text = value;
			}
		}

		public string LatitudeFieldText
		{
			get
			{
				return this.LatitudeField.Text;
			}

			set
			{
				this.LatitudeField.Text = value;
			}
		}

		public string LongitudeFieldText
		{
			get
			{
				return this.LongitudeField.Text;
			}

			set
			{
				this.LongitudeField.Text = value;
			}
		}


		public bool NameFieldEnabled
		{
			get
			{
				return this.NameField.Enabled;
			}

			set
			{
				this.NameField.Enabled = value;
			}
		}

		public bool AddressFieldEnabled
		{
			get
			{
				return this.AddressField.Enabled;
			}

			set
			{
				this.AddressField.Enabled = value;
			}
		}

		public bool LatitudeFieldEnabled
		{
			get
			{
				return this.LatitudeField.Enabled;
			}

			set
			{
				this.LatitudeField.Enabled = value;
			}
		}

		public bool LongitudeFieldEnabled
		{
			get
			{
				return this.LongitudeField.Enabled;
			}

			set
			{
				this.LongitudeField.Enabled = value;
			}
		}

		private bool saveButtonEnabled;
		public bool SaveButtonEnabled
		{
			get
			{
				return saveButtonEnabled;
			}

			set
			{
				saveButtonEnabled = value;
				Toolbar.Menu.Clear();
				if (!value)
				{
					return;
				}

				Toolbar.InflateMenu(Resource.Menu.save_button);
			}
		}

		bool markerDraggable;
		public bool MarkerDraggable
		{
			get
			{
				return markerDraggable;
			}

			set
			{
				markerDraggable = value;
				if (marker != null)
				{
					marker.Draggable = value;
				}
			}
		}
		#endregion


		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			this.SetContentView(Resource.Layout.SubdivisionActivity);
			this.Toolbar.SetOnMenuItemClickListener(this);
			ViewHelper = new ActivityHelper(this);
			AddressField.FocusChange += AddressField_FocusChange; ;
			geocoder = new Geocoder(this);
			model = new SubdivisionModel(this, new SubdivisionService(MehspotAppContext.Instance.DataStorage));
			model.Initialize();
		}

		public void DetectUserPosition(SetPositionDelegate onSuccess, Action onError)
		{
			this.locationDetected = onSuccess;
			this.locationDetectionError = onError;

			if (ContextCompat.CheckSelfPermission(this, Android.Manifest.Permission.AccessFineLocation) ==
				Permission.Granted &&
				ContextCompat.CheckSelfPermission(this, Android.Manifest.Permission.AccessCoarseLocation) ==
				Permission.Granted &&
				ContextCompat.CheckSelfPermission(this, Android.Manifest.Permission.AccessMockLocation) ==
				Permission.Granted)
			{
				DetectUserPosition();
			}
			else
			{
				if (ContextCompat.CheckSelfPermission(this, Android.Manifest.Permission.AccessFineLocation) != Permission.Granted || ContextCompat.CheckSelfPermission(this, Android.Manifest.Permission.AccessCoarseLocation) != Permission.Granted)
				{
					ActivityCompat.RequestPermissions(this, new string[] {
					Android.Manifest.Permission.AccessFineLocation,
					Android.Manifest.Permission.AccessCoarseLocation,
						Android.Manifest.Permission.AccessMockLocation}, 1);
				}

				DetectUserPosition();
			}
		}

		private void DetectUserPosition()
		{
			try
			{
				locationManager = (LocationManager)GetSystemService(LocationService);

				bool isGPSEnabled = locationManager.IsProviderEnabled(LocationManager.GpsProvider);
				bool isNetworkEnabled = locationManager.IsProviderEnabled(LocationManager.NetworkProvider);

				if (!isGPSEnabled && !isNetworkEnabled)
				{
					ViewHelper.ShowAlert("Connection Error", "Sorry, no Internet connectivity detected. Please reconnect and try again.");
				}
				else
				{
					if (isGPSEnabled)
					{
						SpotLocation(LocationManager.GpsProvider);
					}
					else if (isNetworkEnabled)
					{
						SpotLocation(LocationManager.NetworkProvider);
					}
				}
			}
			catch (Exception e)
			{ }
		}

		private void SpotLocation(string provider)
		{
			locationManager.RequestLocationUpdates(provider, MinTime, MinDistance, this);
			if (locationManager != null)
			{
				Location location = locationManager.GetLastKnownLocation(provider);
				if (location != null)
				{
					SetMapLocation(location.Latitude, location.Longitude);
				}
			}
		}

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
		{
			switch (requestCode)
			{
				case 1:
					{
						if (grantResults.FirstOrDefault() == Permission.Granted)
						{
							DetectUserPosition();
						}
						else
						{
							this.locationDetected(Mehspot.Core.Constants.Location.DefaultLatitude, Mehspot.Core.Constants.Location.DefaultLongitude);
						}
						break;
					}
				default:
					break;
			}
		}

		public void OnMapReady(GoogleMap googleMap)
		{
			map = googleMap;
			map.MarkerDragStart += Map_MarkerDragStart;
			map.MarkerDragEnd += Map_MarkerDragEnd;
			var options = new MarkerOptions();
			options.SetPosition(camera.Target);
			marker = map.AddMarker(options);
			marker.Draggable = MarkerDraggable;
			ApplyLocation();
		}

		private void Map_MarkerDragStart(object sender, GoogleMap.MarkerDragStartEventArgs e)
		{
			model.MarkerDraggingStarted();
		}

		private void Map_MarkerDragEnd(object sender, GoogleMap.MarkerDragEndEventArgs e)
		{
			model.MarkerDraggingEnded(e.Marker.Position.Latitude, e.Marker.Position.Longitude);
		}

		private void ApplyLocation(bool setMapOnly = false)
		{
			if (camera != null)
			{
				marker.Position = camera.Target;
				map.AnimateCamera(CameraUpdateFactory.NewCameraPosition(camera));
				if (!setMapOnly)
				{
					LoadPlaceByCoordinates(camera.Target.Latitude, camera.Target.Longitude);
				}

				camera = null;
			}
		}

		public void OnLocationChanged(Location location)
		{
			locationManager.RemoveUpdates(this);
			this.locationDetected(location.Latitude, location.Longitude);
		}

		public void OnProviderDisabled(string provider)
		{
			locationDetectionError();
		}

		public void OnProviderEnabled(string provider)
		{

		}

		public void OnStatusChanged(string provider, Availability status, Bundle extras)
		{

		}

		public bool OnMenuItemClick(IMenuItem item)
		{
			if (item.ItemId == Resource.Id.save_button)
			{
				model.SaveAsync();
				return true;
			}

			return false;
		}

		public void SetMapLocation(double latitude, double longitude, bool setMapOnly = false)
		{
			camera = CameraPosition.FromLatLngZoom(new LatLng(latitude, longitude), 15);
			if (map == null)
			{

				var mapOptions = new GoogleMapOptions()
							.InvokeMapType(GoogleMap.MapTypeNormal)
							.InvokeZoomControlsEnabled(true);

				var fragTx = FragmentManager.BeginTransaction();
				var mapFragment = MapFragment.NewInstance(mapOptions);
				fragTx.Add(Resource.SubdivisionActivity.MapViewWrapper, mapFragment, "map");
				fragTx.Commit();
				mapFragment.GetMapAsync(this);
			}
			else
			{
				ApplyLocation();
			}
		}

		public async void LoadPlaceByCoordinates(double latitude, double longitude)
		{
			var addresses = await geocoder.GetFromLocationAsync(latitude, longitude, 1);
			Address address = addresses.FirstOrDefault();

			List<string> addressLines = new List<string>();
			for (int i = 0; i < address.MaxAddressLineIndex; i++)
			{
				addressLines.Add(address.GetAddressLine(i));
			}

			model.ReverseGeocodeCallback(address.Latitude,
										address.Longitude,
										 address.CountryCode,
										address.SubLocality,
										 addressLines.ToArray());
		}

		public void DismissViewController(EditSubdivisionDTO dto)
		{
			this.Finish();
			OnDismissed(dto);
		}


		private void AddressField_FocusChange(object sender, View.FocusChangeEventArgs e)
		{
			if (!e.HasFocus)
				return;
			try
			{
				LatLngBounds bounds;
				if (map?.CameraPosition != null)
				{
					bounds = new LatLngBounds(this.map.CameraPosition.Target, this.map.CameraPosition.Target);
				}
				else
				{
					var coords = new LatLng(Mehspot.Core.Constants.Location.DefaultLatitude, Mehspot.Core.Constants.Location.DefaultLongitude);
					bounds = new LatLngBounds(coords, coords);
				}

				var intent = new PlaceAutocomplete.IntentBuilder(PlaceAutocomplete.ModeFullscreen)
													 .SetBoundsBias(bounds).Build(this);

				StartActivityForResult(intent, PLACE_AUTOCOMPLETE_REQUEST_CODE);
			}
			catch (GooglePlayServicesRepairableException ex)
			{
				// TODO: Handle the error.
			}
			catch (GooglePlayServicesNotAvailableException ex)
			{
				// TODO: Handle the error.
			}
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			if (requestCode == PLACE_AUTOCOMPLETE_REQUEST_CODE && resultCode == Result.Ok)
			{
				var place = PlaceAutocomplete.GetPlace(this, data);
				string country = null;
				//foreach (var item in place.)
				//{
				//	if (item.Type == "country")
				//	{
				//		country = item.Name;
				//	}
				//}

				model.ReverseGeocodeCallback(
						place.LatLng.Latitude,
						place.LatLng.Longitude,
						country, place.NameFormatted.ToString(), place.AddressFormatted.ToString());
				SetMapLocation(place.LatLng.Latitude, place.LatLng.Longitude, true);
			}
		}

		public void OnConnectionFailed(ConnectionResult result)
		{
			Log.Error(TAG, result.ErrorMessage);
		}
	}
}
