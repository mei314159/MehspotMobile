using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using Google.Maps;
using Mehspot.Core;
using Mehspot.iOS.Views.CustomPicker;
using Mehspot.Core.DTO.Subdivision;
using UIKit;
using CoreLocation;
using Mehspot.Core.Contracts.ViewControllers;
using Mehspot.Core.Models.Subdivisions;

namespace Mehspot.iOS.Controllers
{
	public partial class SubdivisionsListController :
	UIViewController, ICLLocationManagerDelegate, ISubdivisionsListController
	{
		private SubdivisionsListModel model;
		private MapView mapView;
		private Marker marker;
		private CLLocationManager locationManager;

		public SubdivisionsListController() : base("SubdivisionsListController", NSBundle.MainBundle)
		{
		}

		public event Action<SubdivisionDTO> OnDismissed;

		public List<SubdivisionDTO> Subdivisions { get; set; }
		public string ZipCode { get; set; }
		public int? SelectedSubdivisionId { get; set; }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			mapView = new MapView(MapWrapperView.Bounds);
			marker = new Marker();
			marker.Map = mapView;
			MapWrapperView.AddSubview(mapView);

			model = new SubdivisionsListModel(this);
			model.Initialize();
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			model.RefreshMap();
		}

		public void InitializeList(List<SubdivisionDTO> subdivisions, SubdivisionDTO selectedSubdivision)
		{
			var pickerModel = new CustomPickerModel(subdivisions.Select(a => a.DisplayName).ToList());
			this.PickerView.Model = pickerModel;
			this.PickerView.Select(subdivisions.IndexOf(selectedSubdivision), 0, false);
			pickerModel.ItemSelected += (pickerView, row, component) => model.SelectItem((int)row);
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

		public void SetMapLocation(double latitude, double longitude)
		{
			var camera = CameraPosition.FromCamera(latitude, longitude, 15);
			mapView.Camera = camera;
			marker.Position = camera.Target;
		}

		partial void SaveButtonTouched(UIBarButtonItem sender)
		{
			this.OnDismissed?.Invoke(model.SelectedSubdivision);
			DismissViewController(true, null);
		}

		partial void CloseButtonTouched(UIBarButtonItem sender)
		{
			DismissViewController(true, null);
		}

		partial void AddButtonTouched(UIBarButtonItem sender)
		{
			var controller = new SubdivisionController();
			controller.AllowEdititng = true;
			controller.ZipCode = this.ZipCode;
			controller.OnDismissed += model.OnSubdivisionCreated;
			this.PresentViewController(controller, true, null);
		}

		void SubdivisionOptionsActionSheet_Clicked(object sender, UIButtonEventArgs e)
		{
			if (e.ButtonIndex != 0)
				return;

			if (!model.SelectedSubdivision.IsVerified && !model.SelectedSubdivision.IsVerifiedByCurrentUser)
			{
				var controller = new VerifySubdivisionController();
				controller.Subdivision = model.SelectedSubdivision;
				controller.ZipCode = this.ZipCode;
				controller.OnDismissed += model.OnSubdivisionVerified;
				this.PresentViewController(controller, true, null);
			}
			else
			{
				var controller = new SubdivisionController();
				controller.Subdivision = this.model.SelectedSubdivision;
				controller.ZipCode = this.ZipCode;
				controller.AllowEdititng = MehspotAppContext.Instance.AuthManager.AuthInfo.IsAdmin;
				controller.OnDismissed += model.OnSubdivisionUpdated;
				this.PresentViewController(controller, true, null);
			}
		}

		partial void MoreButtonTouched(UIBarButtonItem sender)
		{
			if (model.SelectedSubdivision != null)
			{
				var subdivisionOptionsActionSheet = new UIActionSheet("Options");
				if (model.SelectedSubdivision != null && !model.SelectedSubdivision.IsVerified && !model.SelectedSubdivision.IsVerifiedByCurrentUser)
				{
					subdivisionOptionsActionSheet.AddButton("Verify or Change Subdivision");
				}
				else
				{
					subdivisionOptionsActionSheet.AddButton(MehspotAppContext.Instance.AuthManager.AuthInfo.IsAdmin ? "Edit Subdivision" : "View Details");
				}

				subdivisionOptionsActionSheet.AddButton("Cancel");
				subdivisionOptionsActionSheet.CancelButtonIndex = 1;
				subdivisionOptionsActionSheet.Clicked += SubdivisionOptionsActionSheet_Clicked;
				subdivisionOptionsActionSheet.ShowInView(View);
			}
		}
	}
}

