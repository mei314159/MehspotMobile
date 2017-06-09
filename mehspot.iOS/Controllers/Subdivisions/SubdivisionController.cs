using System;
using CoreGraphics;
using CoreLocation;
using Foundation;
using Google.Maps;
using Mehspot.iOS.Wrappers;
using Mehspot.Core;
using Mehspot.Core.DTO;
using Mehspot.Core.Services;
using Mehspot.Core.DTO.Subdivision;
using UIKit;
using System.Linq;

namespace Mehspot.iOS.Controllers
{
	public partial class SubdivisionController :
			UIViewController, IUITableViewDataSource, IUITableViewDelegate
	{
		private MapView mapView;
		private Marker marker;
		private Geocoder geocoder = new Geocoder();
		private PlacesClient placesClient;

		private CLLocationManager locationManager;
		private SubdivisionService subdivisionService;
		private ViewHelper viewHelper;
		private bool setPlaceOnly;
		private UITableView autocompleteResultsView;

		private string Country;
		private CLLocationCoordinate2D Coordinate;
		private AutocompletePrediction[] autocompleteResults;


		public event Action<EditSubdivisionDTO> OnDismissed;

		public SubdivisionController() : base("SubdivisionController", NSBundle.MainBundle)
		{
		}

		public SubdivisionDTO Subdivision { get; set; }
		public string ZipCode { get; set; }

		public bool AllowEdititng { get; internal set; }


		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			placesClient = PlacesClient.SharedClient();
			viewHelper = new ViewHelper(this.View);
			subdivisionService = new SubdivisionService(MehspotAppContext.Instance.DataStorage);
			LatitudeField.KeyboardType = LongitudeField.KeyboardType = UIKeyboardType.DecimalPad;
			this.AddressField.EditingChanged += AddressField_EditingChanged;
			this.AddressField.TouchDown += (sender, e) => HideAutocompleteResults();

			mapView = new MapView(MapWrapperView.Bounds);
			mapView.DraggingMarkerStarted += MapView_DraggingMarkerStarted;
			mapView.DraggingMarkerEnded += MapView_DraggingMarkerEnded;
			marker = new Marker();
			marker.Map = mapView;
			marker.Draggable = true;


			if (Subdivision == null)
			{
				locationManager = new CLLocationManager();
				if (CLLocationManager.Status == CLAuthorizationStatus.NotDetermined)
				{
					locationManager.RequestWhenInUseAuthorization();
					locationManager.AuthorizationChanged += (sender, e) => DetectUserPosition(e.Status);
				}
				else
				{
					DetectUserPosition(CLLocationManager.Status);
				}
			}
			else
			{
				setPlaceOnly = true;
				this.NameField.Text = Subdivision.DisplayName;
				this.AddressField.Text = Subdivision.FormattedAddress;
				this.LatitudeField.Text = Subdivision.Latitude.ToString();
				this.LongitudeField.Text = Subdivision.Longitude.ToString();

				SetLocation(Subdivision.Latitude, Subdivision.Longitude);
				GetLocationByCoordinates(mapView.Camera.Target);
			}

			if (!AllowEdititng)
				this.NavBarItem.RightBarButtonItems = new UIBarButtonItem[] { };
			this.NameField.Enabled = this.AddressField.Enabled = LatitudeField.Enabled = LongitudeField.Enabled = marker.Draggable = AllowEdititng;

			MapWrapperView.AddSubview(mapView);

			this.NameField.UserInteractionEnabled = true;
			this.NameField.AddGestureRecognizer(new UITapGestureRecognizer(this.HideAutocompleteResults));
		}

		void DetectUserPosition(CLAuthorizationStatus status)
		{
			if (status == CLAuthorizationStatus.NotDetermined)
				return;

			if (status == CLAuthorizationStatus.Denied || status == CLAuthorizationStatus.Restricted)
			{
				SetLocation(Mehspot.Core.Constants.Location.DefaultLatitude, Mehspot.Core.Constants.Location.DefaultLongitude);
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
						SetLocation(location.Coordinate.Latitude, location.Coordinate.Longitude);
						GetLocationByCoordinates(mapView.Camera.Target);
					}
				};
				locationManager.Failed += (sender, e) =>
				{
					SetLocation(Mehspot.Core.Constants.Location.DefaultLatitude, Mehspot.Core.Constants.Location.DefaultLongitude);
					GetLocationByCoordinates(mapView.Camera.Target);
				};
				locationManager.StartUpdatingLocation();
			}
		}

		void MapView_DraggingMarkerStarted(object sender, GMSMarkerEventEventArgs e)
		{
			this.AddressField.Enabled = LatitudeField.Enabled = LongitudeField.Enabled = false;
		}

		void MapView_DraggingMarkerEnded(object sender, GMSMarkerEventEventArgs e)
		{
			viewHelper.ShowOverlay("Wait...");
			GetLocationByCoordinates(e.Marker.Position);
		}

		void HandleReverseGeocodeCallback(ReverseGeocodeResponse response, NSError error)
		{
			this.AddressField.Enabled = LatitudeField.Enabled = LongitudeField.Enabled = AllowEdititng;

			if (error != null)
				return;
			var address = response.FirstResult;
			this.Coordinate = address.Coordinate;
			this.Country = address.Country;
			this.ZipCode = this.ZipCode ?? address.PostalCode;

			if (!setPlaceOnly)
			{
				this.NameField.Text = address.SubLocality;
				this.AddressField.Text = address.Lines != null ? string.Join(", ", address.Lines) : address.Thoroughfare;
				this.LatitudeField.Text = address.Coordinate.Latitude.ToString();
				this.LongitudeField.Text = address.Coordinate.Longitude.ToString();
			}

			setPlaceOnly = false;
			viewHelper.HideOverlay();
		}

		async partial void SaveButtonTouched(UIBarButtonItem sender)
		{
			viewHelper.ShowOverlay("Saving...");
			var dto = new EditSubdivisionDTO
			{
				Id = Subdivision?.Id ?? default(int),
				Name = this.NameField.Text,
				OptionId = Subdivision?.OptionId,
				ZipCode = this.ZipCode,
				Address = new AddressDTO
				{
					Latitude = this.Coordinate.Latitude,
					Longitude = this.Coordinate.Longitude,
					Country = this.Country,
					FormattedAddress = this.AddressField.Text,
					PostalCode = this.ZipCode,
					GoverningDistrictId = 1,
				}
			};

			Result result;
			if (Subdivision != null)
			{
				var overrideResult = await this.subdivisionService.OverrideAsync(dto);
				dto.Id = overrideResult.Data.Id;
				result = overrideResult;
			}
			else
			{
				var createResult = await this.subdivisionService.CreateAsync(dto);
				dto.Id = createResult.Data.SubdivisionId;
				dto.OptionId = createResult.Data.OptionId;
				result = createResult;
			}

			viewHelper.HideOverlay();
			if (result.IsSuccess)
			{
				DismissViewController(true, null);
				this.OnDismissed?.Invoke(dto);
			}
			else
			{
				viewHelper.ShowAlert("Error", result.ErrorMessage);
			}
		}

		partial void CloseButtonTouched(UIBarButtonItem sender)
		{
			DismissViewController(true, null);
		}

		void GetLocationByCoordinates(CLLocationCoordinate2D position)
		{
			geocoder.ReverseGeocodeCord(position, HandleReverseGeocodeCallback);
		}

		void AddressField_EditingChanged(object sender, EventArgs e)
		{
			var coordinateBounds = new CoordinateBounds(mapView.Camera.Target, mapView.Camera.Target);
			placesClient.AutocompleteQuery(AddressField.Text, coordinateBounds, new AutocompleteFilter { }, HandleAutocompletePredictionsHandler);
		}


		void HandleAutocompletePredictionsHandler(AutocompletePrediction[] results, NSError error)
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
			if (string.IsNullOrWhiteSpace(this.NameField.Text))
			{
				this.NameField.Text = result.Name;
			}

			this.AddressField.Text = result.FormattedAddress;
			this.LatitudeField.Text = result.Coordinate.Latitude.ToString();
			this.LongitudeField.Text = result.Coordinate.Longitude.ToString();
			this.Coordinate = result.Coordinate;
			foreach (var item in result.AddressComponents)
			{
				if (item.Type == "postal_code")
				{
					this.ZipCode = item.Name;
				}
				else if (item.Type == "country")
				{
					this.Country = item.Name;
				}
			}

			SetLocation(this.Coordinate.Latitude, this.Coordinate.Longitude);
		}

		void HideAutocompleteResults()
		{
			if (autocompleteResultsView != null)
			{
				autocompleteResultsView.RemoveFromSuperview();
			}
		}

		void SetLocation(double latitude, double longitude)
		{
			var camera = CameraPosition.FromCamera(latitude, longitude, 15);
			mapView.Camera = camera;
			marker.Position = camera.Target;
		}
	}
}