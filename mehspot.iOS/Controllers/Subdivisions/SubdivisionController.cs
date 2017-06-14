using System;
using CoreGraphics;
using CoreLocation;
using Foundation;
using Google.Maps;
using Mehspot.iOS.Wrappers;
using Mehspot.Core;
using Mehspot.Core.Services;
using Mehspot.Core.DTO.Subdivision;
using UIKit;
using System.Linq;
using Mehspot.Core.Contracts.ViewControllers;
using Mehspot.Core.Models.Subdivisions;
using Mehspot.Core.Contracts.Wrappers;

namespace Mehspot.iOS.Controllers
{
	public partial class SubdivisionController :
			UIViewController, IUITableViewDataSource, IUITableViewDelegate,
			ISubdivisionController
	{
		private MapView mapView;
		private Marker marker;
		private Geocoder geocoder = new Geocoder();
		private PlacesClient placesClient;

		private CLLocationManager locationManager;
		private UITableView autocompleteResultsView;
		private SubdivisionModel model;

		private AutocompletePrediction[] autocompleteResults;


		public SubdivisionController() : base("SubdivisionController", NSBundle.MainBundle)
		{
		}

		#region Properties
		public IViewHelper ViewHelper { get; private set; }
		public SubdivisionDTO Subdivision { get; set; }
		public string ZipCode { get; set; }
		public bool AllowEdititng { get; set; }

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

		public bool MarkerDraggable
		{
			get
			{
				return this.marker.Draggable;
			}

			set
			{
				this.marker.Draggable = value;
			}
		}

		public bool SaveButtonEnabled
		{
			get
			{
				return (this.NavBarItem.RightBarButtonItems?.Count() ?? 0) > 0;
			}

			set
			{
				if (!value)
				{
					this.NavBarItem.RightBarButtonItems = new UIBarButtonItem[] { };
				}
			}
		}
		#endregion

		public event Action<EditSubdivisionDTO> OnDismissed;

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			placesClient = PlacesClient.SharedClient();
			ViewHelper = new ViewHelper(this.View);
			LatitudeField.KeyboardType = LongitudeField.KeyboardType = UIKeyboardType.DecimalPad;
			this.AddressField.EditingChanged += AddressField_EditingChanged;
			this.AddressField.TouchDown += (sender, e) => HideAutocompleteResults();

			mapView = new MapView(MapWrapperView.Bounds);
			mapView.DraggingMarkerStarted += MapView_DraggingMarkerStarted;
			mapView.DraggingMarkerEnded += MapView_DraggingMarkerEnded;
			marker = new Marker();
			marker.Map = mapView;
			marker.Draggable = true;
			MapWrapperView.AddSubview(mapView);

			this.NameField.UserInteractionEnabled = true;
			this.NameField.AddGestureRecognizer(new UITapGestureRecognizer(this.HideAutocompleteResults));

			model = new SubdivisionModel(this, new SubdivisionService(MehspotAppContext.Instance.DataStorage));
			model.Initialize();

		}

		void MapView_DraggingMarkerStarted(object sender, GMSMarkerEventEventArgs e)
		{
			model.MarkerDraggingStarted();
		}

		void MapView_DraggingMarkerEnded(object sender, GMSMarkerEventEventArgs e)
		{
			model.MarkerDraggingEnded(e.Marker.Position.Latitude, e.Marker.Position.Longitude);
		}

		async partial void SaveButtonTouched(UIBarButtonItem sender)
		{
			await model.SaveAsync();
		}

		partial void CloseButtonTouched(UIBarButtonItem sender)
		{
			DismissViewController(true, null);
		}

		public void DismissViewController(EditSubdivisionDTO dto)
		{
			DismissViewController(true, null);
			this.OnDismissed?.Invoke(dto);
		}

		public void LoadPlaceByCoordinates(double latitude, double longitude)
		{
			geocoder.ReverseGeocodeCord(new CLLocationCoordinate2D(latitude, longitude), HandleReverseGeocodeCallback);
		}

		private void HandleReverseGeocodeCallback(ReverseGeocodeResponse response, NSError error)
		{
			if (error != null)
				return;

			var address = response.FirstResult;
			model.ReverseGeocodeCallback(address.Coordinate.Latitude,
										address.Coordinate.Longitude,
										address.Country,
										address.SubLocality,
										address.Lines);
		}

		private void AddressField_EditingChanged(object sender, EventArgs e)
		{
			var coordinateBounds = new CoordinateBounds(mapView.Camera.Target, mapView.Camera.Target);
			placesClient.AutocompleteQuery(AddressField.Text, coordinateBounds,
										   new AutocompleteFilter { }, HandleAutocompletePredictions);
		}

		private void HandleAutocompletePredictions(AutocompletePrediction[] results, NSError error)
		{
			autocompleteResults = results;
			if (autocompleteResultsView == null)
			{
				const int rowHeight = 44;
				const int resultsCount = 5;
				autocompleteResultsView = new UITableView(new CGRect(0, AddressField.Frame.Y + AddressField.Frame.Height, this.View.Frame.Width, resultsCount * rowHeight));
				autocompleteResultsView.RegisterClassForCellReuse(typeof(UITableViewCell), "autocompleteRow");
				autocompleteResultsView.WeakDataSource = this;
				autocompleteResultsView.Delegate = this;
			}

			if (autocompleteResultsView.Superview == null)
			{
				this.View.AddSubview(autocompleteResultsView);
			}

			autocompleteResultsView.ReloadData();
		}

		public nint RowsInSection(UITableView tableView, nint section)
		{
			return autocompleteResults?.Length ?? 0;
		}

		public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell("autocompleteRow");
			var item = autocompleteResults[indexPath.Row];
			cell.TextLabel.Text = item.AttributedFullText.Value;
			return cell;
		}

		[Export("tableView:didSelectRowAtIndexPath:")]
		public void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			HideAutocompleteResults();
			placesClient.LookUpPlaceID(autocompleteResults[indexPath.Row].PlaceID, HandlePlaceResultHandler);
		}

		void HandlePlaceResultHandler(Place result, NSError error)
		{
			string country = null;
			foreach (var item in result.AddressComponents)
			{
				if (item.Type == "country")
				{
					country = item.Name;
				}
			}

			model.ReverseGeocodeCallback(
				result.Coordinate.Latitude,
				result.Coordinate.Longitude,
				country, result.Name, result.FormattedAddress);
			SetMapLocation(result.Coordinate.Latitude, result.Coordinate.Longitude, true);
		}

		public void SetMapLocation(double latitude, double longitude, bool setMapOnly = false)
		{
			var camera = CameraPosition.FromCamera(latitude, longitude, 15);
			mapView.Camera = camera;
			marker.Position = camera.Target;
			if (!setMapOnly)
			{
				LoadPlaceByCoordinates(latitude, longitude);
			}
		}

		public void DetectUserPosition(SetPositionDelegate onSuccess, Action onError)
		{
			locationManager = new CLLocationManager();
			if (CLLocationManager.Status == CLAuthorizationStatus.NotDetermined)
			{
				locationManager.RequestWhenInUseAuthorization();
				locationManager.AuthorizationChanged += (sender, e) => DetectUserPosition(e.Status, onSuccess, onError);
			}
			else
			{
				DetectUserPosition(CLLocationManager.Status, onSuccess, onError);
			}
		}

		void DetectUserPosition(CLAuthorizationStatus status, SetPositionDelegate onSuccess, Action onError)
		{
			if (status == CLAuthorizationStatus.NotDetermined)
			{
				return;
			}

			if (status == CLAuthorizationStatus.Denied || status == CLAuthorizationStatus.Restricted)
			{
				onError();
			}
			else
			{
				locationManager.DistanceFilter = 100;
				locationManager.LocationsUpdated += (sender, e) =>
				{
					var location = e.Locations?.FirstOrDefault();
					if (location != null)
					{
						locationManager.StopUpdatingLocation();
						onSuccess(location.Coordinate.Latitude, location.Coordinate.Longitude);
					}
				};
				locationManager.Failed += (sender, e) => onError();
				locationManager.StartUpdatingLocation();
			}
		}

		void HideAutocompleteResults()
		{
			if (autocompleteResultsView != null)
			{
				autocompleteResultsView.RemoveFromSuperview();
			}
		}
	}
}