
using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Content;
using Android.Content.PM;
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
	Android.Support.V7.Widget.Toolbar.IOnMenuItemClickListener
	{
		static readonly string TAG = "X:" + nameof(SubdivisionsListActivity);
		SubdivisionModel model;
		private Marker marker;
		private GoogleMap map;
		CameraPosition camera;
		LocationManager locationManager;
		string locationProvider;

		SetPositionDelegate locationDetected;
		Action locationDetectionError;

		public Action<EditSubdivisionDTO> OnDismissed => Intent.GetExtra<Action<EditSubdivisionDTO>>("onDismissed");

		public Android.Support.V7.Widget.Toolbar Toolbar => FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.SubdivisionActivity.Menu);

		public IViewHelper ViewHelper { get; set; }
		public SubdivisionDTO Subdivision => Intent.GetExtra<SubdivisionDTO>("subdivision");
		public string ZipCode => Intent.GetStringExtra("zipCode");
		public bool AllowEdititng => Intent.GetBooleanExtra("allowEdititng", false);

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

				Toolbar.InflateMenu(Resource.Menu.save_subdivision_menu);
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

		public EditText NameField => FindViewById<EditText>(Resource.SubdivisionActivity.NameField);
		public EditText AddressField => FindViewById<EditText>(Resource.SubdivisionActivity.AddressField);
		public EditText LatitudeField => FindViewById<EditText>(Resource.SubdivisionActivity.LatitudeField);
		public EditText LongitudeField => FindViewById<EditText>(Resource.SubdivisionActivity.LongitudeField);

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			this.SetContentView(Resource.Layout.SubdivisionActivity);
			this.Toolbar.SetOnMenuItemClickListener(this);
			ViewHelper = new ActivityHelper(this);
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
					Android.Manifest.Permission.AccessCoarseLocation }, 1);
				}
			}
		}

		void DetectUserPosition()
		{
			locationManager = (LocationManager)GetSystemService(LocationService);
			Criteria criteriaForLocationService = new Criteria
			{
				Accuracy = Accuracy.Fine
			};
			var acceptableLocationProviders = locationManager.GetProviders(criteriaForLocationService, true);

			if (acceptableLocationProviders.Any())
			{
				locationProvider = acceptableLocationProviders.First();
			}
			else
			{
				locationProvider = string.Empty;
			}

			Log.Debug(TAG, "Using " + locationProvider + ".");

			locationManager.RequestLocationUpdates(locationProvider, 0, 0, this);
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
						break;
					}
				default:
					break;
			}
		}

		public void OnMapReady(GoogleMap googleMap)
		{
			map = googleMap;
			var options = new MarkerOptions();
			options.SetPosition(camera.Target);
			marker = map.AddMarker(options);
			marker.Draggable = MarkerDraggable;
			ApplyLocation();
		}

		void ApplyLocation(bool setMapOnly = false)
		{
			if (camera != null)
			{
				marker.Position = camera.Target;
				map.AnimateCamera(CameraUpdateFactory.NewCameraPosition(camera));
				camera = null;
				if (!setMapOnly)
				{
					LoadPlaceByCoordinates(camera.Target.Latitude, camera.Target.Longitude);
				}
			}
		}

		public void OnLocationChanged(Location location)
		{
			locationManager.RequestLocationUpdates(locationProvider, 0, 0, this);
			this.locationDetected(location.Latitude, location.Longitude);
		}

		public void OnProviderDisabled(string provider)
		{
			locationDetectionError();
		}

		public void OnProviderEnabled(string provider) { }

		public void OnStatusChanged(string provider, Availability status, Bundle extras) { }

		public bool OnMenuItemClick(IMenuItem item)
		{
			return true;
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

		public void LoadPlaceByCoordinates(double latitude, double longitude)
		{
			throw new NotImplementedException();
		}

		public void DismissViewController(EditSubdivisionDTO dto)
		{
			this.Finish();
			OnDismissed(dto);
		}
	}
}
