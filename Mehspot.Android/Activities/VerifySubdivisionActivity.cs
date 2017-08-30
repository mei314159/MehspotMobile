
using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Mehspot.AndroidApp.Core.Builders;
using Mehspot.AndroidApp.Wrappers;
using Mehspot.Core;
using Mehspot.Core.Contracts.ViewControllers;
using Mehspot.Core.Contracts.Wrappers;
using Mehspot.Core.DTO.Subdivision;
using Mehspot.Core.Models.Subdivisions;
using Mehspot.Core.Services;

namespace Mehspot.AndroidApp.Activities
{
	[Activity(Label = "Verify Subdivision Activity", ScreenOrientation = ScreenOrientation.Portrait)]
	public class VerifySubdivisionActivity : AppCompatActivity, IVerifySubdivisionController, IOnMapReadyCallback,
	Android.Support.V7.Widget.Toolbar.IOnMenuItemClickListener, GoogleApiClient.IOnConnectionFailedListener
	{
		private static readonly string TAG = "X:" + nameof(VerifySubdivisionActivity);
		private VerifySubdivisionModel<View> model;
		private Marker marker;
		private GoogleMap map;
		private Geocoder geocoder;
		private CameraPosition camera;

		#region Properties

		public IViewHelper ViewHelper { get; set; }

		SubdivisionDTO subdivision;
		public SubdivisionDTO Subdivision
		{
			get
			{
				return subdivision ?? (subdivision = Intent.GetExtra<SubdivisionDTO>("subdivision"));
			}

			set
			{
				subdivision = value;
			}
		}

		public string ZipCode => Intent.GetStringExtra("zipCode");
		public Action<SubdivisionDTO, bool> OnDismissed => Intent.GetExtra<Action<SubdivisionDTO, bool>>("onDismissed");


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

		public LinearLayout NameSection => FindViewById<LinearLayout>(Resource.VerifySubdivisionActivity.NameSection);
		public LinearLayout AddressSection => FindViewById<LinearLayout>(Resource.VerifySubdivisionActivity.AddressSection);
		public Android.Support.V7.Widget.Toolbar Toolbar => FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.VerifySubdivisionActivity.Menu);

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			this.SetContentView(Resource.Layout.VerifySubdivisionActivity);
			this.Toolbar.SetOnMenuItemClickListener(this);
			ViewHelper = new ActivityHelper(this);
			geocoder = new Geocoder(this);
			model = new VerifySubdivisionModel<View>(this, new SubdivisionService(MehspotAppContext.Instance.DataStorage), new AndroidCellBuilder(this));
		}

		protected override void OnStart()
		{
			base.OnStart();
			model.InitializeAsync();
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

		void Map_MarkerDragStart(object sender, GoogleMap.MarkerDragStartEventArgs e)
		{
			model.MarkerDraggingStarted();
		}

		void Map_MarkerDragEnd(object sender, GoogleMap.MarkerDragEndEventArgs e)
		{
			model.MarkerDraggingEnded(e.Marker.Position.Latitude, e.Marker.Position.Longitude);
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
										 address.PostalCode,
										 addressLines.ToArray());
		}

		public void OnConnectionFailed(ConnectionResult result)
		{
			Log.Error(TAG, result.ErrorMessage);
		}

		public void DisplayCells()
		{
			model.Sections[0].Rows.ForEach(NameSection.AddView);
			model.Sections[1].Rows.ForEach(AddressSection.AddView);
		}

		public void ShowLocation(double latitude, double longitude)
		{
			camera = CameraPosition.FromLatLngZoom(new LatLng(latitude, longitude), 15);
			if (map == null)
			{
				var mapOptions = new GoogleMapOptions()
							.InvokeMapType(GoogleMap.MapTypeNormal)
							.InvokeZoomControlsEnabled(true);

				var fragTx = FragmentManager.BeginTransaction();
				var mapFragment = MapFragment.NewInstance(mapOptions);
				fragTx.Add(Resource.VerifySubdivisionActivity.MapViewWrapper, mapFragment, "map");
				fragTx.Commit();
				mapFragment.GetMapAsync(this);
			}
			else
			{
				ApplyLocation();
			}
		}

		public void OnSubdivisionVerified(SubdivisionDTO dto, bool isNewOption)
		{
			this.Finish();
			OnDismissed(dto, isNewOption);
		}

		void ApplyLocation(bool setMapOnly = false)
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
	}
}
