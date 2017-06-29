﻿using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Mehspot.AndroidApp.Adapters;
using Mehspot.AndroidApp.Resources.layout;
using Mehspot.AndroidApp.Wrappers;
using Mehspot.Core;
using Mehspot.Core.Contracts.ViewControllers;
using Mehspot.Core.Contracts.Wrappers;
using Mehspot.Core.DTO.Subdivision;
using Mehspot.Core.Models.Subdivisions;

namespace Mehspot.AndroidApp.Activities
{
	[Activity(Label = "SubdivisionsListActivity")]
	public class SubdivisionsListActivity : AppCompatActivity, ISubdivisionsListController, IOnMapReadyCallback, ILocationListener,
	Android.Support.V7.Widget.Toolbar.IOnMenuItemClickListener
	{
		private const long MinTime = 0;
		private const float MinDistance = 0;

		private static readonly string TAG = "X:" + nameof(SubdivisionsListActivity);
		private SubdivisionsListModel model;
		private Marker marker;
		private GoogleMap map;
		private LocationManager locationManager;
		private CameraPosition camera;
		private string locationProvider;
		private SetPositionDelegate locationDetected;
		private Action locationDetectionError;

		public IViewHelper ViewHelper { get; set; }
		public int? SelectedSubdivisionId => Intent.GetExtra<int?>("selectedSubdivisionId");
		public List<SubdivisionDTO> Subdivisions => Intent.GetExtra<List<SubdivisionDTO>>("subdivisions");
		public Android.Support.V7.Widget.Toolbar Toolbar => FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.SubdivisionListActivity.Menu);
		public string ZipCode => Intent.GetStringExtra("zipCode");
		public Action<SubdivisionDTO> OnDismissed => Intent.GetExtra<Action<SubdivisionDTO>>("onDismissed");


		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			this.SetContentView(Resource.Layout.SubdivisionsListActivity);
			//var menu = FindViewById<ActionMenuView>(Resource.SubdivisionListActivity.Menu);
			//menu.Menu.Add(new Java.Lang.String("hello"));
			this.Toolbar.SetOnMenuItemClickListener(this);
			ViewHelper = new ActivityHelper(this);
			model = new SubdivisionsListModel(this);
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
				this.DetectUserPosition();
			}
			else
			{
				if (ContextCompat.CheckSelfPermission(this, Android.Manifest.Permission.AccessFineLocation) !=
					Permission.Granted ||
					ContextCompat.CheckSelfPermission(this, Android.Manifest.Permission.AccessCoarseLocation) !=
					Permission.Granted)
				{
					ActivityCompat.RequestPermissions(this, new string[] {
					Android.Manifest.Permission.AccessFineLocation,
					Android.Manifest.Permission.AccessCoarseLocation }, 2);
				}
			}
		}

		public void InitializeList(List<SubdivisionDTO> subdivisions, SubdivisionDTO selectedSubdivision)
		{
			var listView = FindViewById<ListView>(Resource.SubdivisionListActivity.SubdivisionsList);

			if (listView.Adapter == null)
			{
				var subdivisionsListAdapter = new SubdivisionsListAdapter(this, subdivisions, model.SelectedSubdivision);
				listView.Adapter = subdivisionsListAdapter;
				listView.ItemClick += ListView_ItemClick;
				if (model.SelectedSubdivision != null)
				{
					listView.SetSelection(subdivisions.IndexOf(model.SelectedSubdivision));
				}
			}
			else
			{
				((SubdivisionsListAdapter)listView.Adapter).NotifyDataSetChanged();
			}

		}

		public void SetMapLocation(double latitude, double longitude)
		{
			camera = CameraPosition.FromLatLngZoom(new LatLng(latitude, longitude), 15);
			if (map == null)
			{

				var mapOptions = new GoogleMapOptions()
							.InvokeMapType(GoogleMap.MapTypeNormal)
							.InvokeZoomControlsEnabled(true);

				var fragTx = FragmentManager.BeginTransaction();
				var mapFragment = MapFragment.NewInstance(mapOptions);
				fragTx.Add(Resource.SubdivisionListActivity.MapViewWrapper, mapFragment, "map");
				fragTx.Commit();
				mapFragment.GetMapAsync(this);
			}
			else
			{
				ApplyLocation();
			}

			UpdateToolbar(model.SelectedSubdivision);
		}

		public void OnMapReady(GoogleMap googleMap)
		{
			map = googleMap;
			var options = new MarkerOptions();
			options.SetPosition(camera.Target);
			marker = map.AddMarker(options);

			ApplyLocation();
		}

		private void ApplyLocation()
		{
			if (camera != null)
			{
				marker.Position = camera.Target;
				map.AnimateCamera(CameraUpdateFactory.NewCameraPosition(camera));
				camera = null;
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
					InitializeList(Subdivisions, null);
					SetMapLocation(location.Latitude, location.Longitude);
				}
			}
		}

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
		{
			switch (requestCode)
			{
				case 2:
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

		public void OnLocationChanged(Location location)
		{
			locationManager.RemoveUpdates(this);
			this.locationDetected(location.Latitude, location.Longitude);
		}

		public void OnProviderDisabled(string provider)
		{
			locationDetectionError();
		}

		public void OnProviderEnabled(string provider) { }

		public void OnStatusChanged(string provider, Availability status, Bundle extras) { }

		private void SubdivisionsListAdapter_Clicked(SubdivisionDTO dto)
		{
			model.SelectItem(dto);
		}

		private void ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
		{
			var item = e.View as SubdivisionsListItem;
			if (item != null)
			{
				item.Selected = true;
				model.SelectItem(item.SubdivisionDTO);
			}
		}

		private void UpdateToolbar(SubdivisionDTO dto)
		{
			Toolbar.Menu.Clear();
			if (dto == null)
			{
				Toolbar.InflateMenu(Resource.Menu.add_menu);
			}
			else if (dto.IsVerified && !model.SelectedSubdivision.IsVerifiedByCurrentUser)
			{
				Toolbar.InflateMenu(Resource.Menu.view_menu);
			}
			else
			{
				Toolbar.InflateMenu(Resource.Menu.verify_menu);
			}
		}

		public bool OnMenuItemClick(IMenuItem item)
		{
			Intent target;
			switch (item.ItemId)
			{
				case Resource.Id.new_subdivision:
					target = new Intent(this, typeof(SubdivisionActivity));
					target.PutExtra("zipCode", this.ZipCode);
					target.PutExtra("allowEdititng", true);
					target.PutExtra("onDismissed", new Action<EditSubdivisionDTO>(model.OnSubdivisionCreated));
					this.StartActivity(target);
					return true;
				case Resource.Id.view_subdivision:
					target = new Intent(this, typeof(SubdivisionActivity));
					target.PutExtra("subdivision", this.model.SelectedSubdivision);
					target.PutExtra("zipCode", this.ZipCode);
					target.PutExtra("allowEdititng", MehspotAppContext.Instance.AuthManager.AuthInfo.IsAdmin);
					target.PutExtra("onDismissed", new Action<EditSubdivisionDTO>(model.OnSubdivisionUpdated));
					this.StartActivity(target);
					return true;
				case Resource.Id.verify_subdivision:
					target = new Intent(this, typeof(VerifySubdivisionActivity));
					target.PutExtra("subdivision", this.model.SelectedSubdivision);
					target.PutExtra("zipCode", this.ZipCode);
					target.PutExtra("onDismissed", new Action<SubdivisionDTO, bool>(model.OnSubdivisionVerified));
					this.StartActivity(target);
					return true;
				case Resource.Id.save_button:
					DismissViewController(this.model.SelectedSubdivision);
					//Do stuff for item2
					return true;
				default:
					return false;
			}
		}

		public void DismissViewController(SubdivisionDTO dto)
		{
			this.Finish();
			OnDismissed(dto);
		}
	}
}
