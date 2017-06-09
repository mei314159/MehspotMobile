﻿
using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using Android.OS;
using Android.Util;
using Android.Widget;
using Mehspot.AndroidApp.Adapters;
using Mehspot.Core.Contracts.ViewControllers;
using Mehspot.Core.DTO.Subdivision;
using Mehspot.Core.Models.Subdivisions;

namespace Mehspot.AndroidApp.Activities
{
	[Activity(Label = "SubdivisionsListActivity")]
	public class SubdivisionsListActivity : Activity, ISubdivisionsListController, IOnMapReadyCallback, ILocationListener
	{
		static readonly string TAG = "X:" + nameof(SubdivisionsListActivity);
		SubdivisionsListModel model;
		private MapView mapView;
		private Marker marker;
		private GoogleMap map;

		LocationManager locationManager;
		string locationProvider;

		SetPositionDelegate locationDetected;
		Action locationDetectionError;

		public int? SelectedSubdivisionId => Intent.GetExtra<int?>("selectedSubdivisionId");

		public List<SubdivisionDTO> Subdivisions => Intent.GetExtra<List<SubdivisionDTO>>("subdivisions");

		public string ZipCode => Intent.GetStringExtra("zipCode");
		public Action<SubdivisionDTO> OnDismissed => Intent.GetExtra<Action<SubdivisionDTO>>("onDismissed");
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			this.SetContentView(Resource.Layout.SubdivisionsListActivity);
			model = new SubdivisionsListModel(this);


			this.mapView = FindViewById<MapView>(Resource.SubdivisionListActivity.MapView);
			mapView.GetMapAsync(this);
		}

		public void DetectUserPosition(SetPositionDelegate onSuccess, Action onError)
		{
			this.locationDetected = onSuccess;
			this.locationDetectionError = onError;
			locationManager.RequestLocationUpdates(locationProvider, 0, 0, this);
		}

		public void InitializeList(List<SubdivisionDTO> subdivisions, SubdivisionDTO selectedSubdivision)
		{
			var listView = FindViewById<ListView>(Resource.SubdivisionListActivity.SubdivisionsList);

			if (listView.Adapter == null)
			{
				listView.Adapter = new SubdivisionsListAdapter(this, subdivisions);
			}
			else
			{
				((SubdivisionsListAdapter)listView.Adapter).NotifyDataSetChanged();
			}

		}

		CameraPosition camera;
		public void SetMapLocation(double latitude, double longitude)
		{
			camera = CameraPosition.FromLatLngZoom(new LatLng(latitude, longitude), 15);
			marker.Position = camera.Target;
			map.AnimateCamera(CameraUpdateFactory.NewCameraPosition(camera));
		}

		public void OnMapReady(GoogleMap googleMap)
		{
			map = googleMap;
			marker = map.AddMarker(new MarkerOptions());
			model.Initialize();
		}

		void InitializeLocationManager()
		{
			locationManager = (LocationManager)GetSystemService(LocationService);
			Criteria criteriaForLocationService = new Criteria
			{
				Accuracy = Accuracy.Fine
			};
			IList<string> acceptableLocationProviders = locationManager.GetProviders(criteriaForLocationService, true);

			if (acceptableLocationProviders.Any())
			{
				locationProvider = acceptableLocationProviders.First();
			}
			else
			{
				locationProvider = string.Empty;
			}

			Log.Debug(TAG, "Using " + locationProvider + ".");
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
	}
}
