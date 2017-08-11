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
using Mehspot.iOS.Wrappers;

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
			MapWrapperView.AddConstraint(NSLayoutConstraint.Create(mapView, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, MapWrapperView, NSLayoutAttribute.Trailing, 1, 0));
			MapWrapperView.AddConstraint(NSLayoutConstraint.Create(mapView, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, MapWrapperView, NSLayoutAttribute.Trailing, 1, 0));
			MapWrapperView.AddConstraint(NSLayoutConstraint.Create(mapView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, MapWrapperView, NSLayoutAttribute.Top, 1, 0));
			MapWrapperView.AddConstraint(NSLayoutConstraint.Create(mapView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, MapWrapperView, NSLayoutAttribute.Bottom, 1, 0));
			mapView.AutoresizingMask = UIViewAutoresizing.All;
			MapWrapperView.AutoresizingMask = UIViewAutoresizing.All;
			model = new SubdivisionsListModel(this);
			model.Initialize();
			if (Subdivisions?.Count > 0)
			{
				model.RefreshMap();
			}
			else
			{
				var avAlert = new UIAlertView("There is no subdivision yet for this zip code", "Please add your subdivision", (IUIAlertViewDelegate)null, "OK", null);
				avAlert.Clicked += (sender, e) => GoToAddSubdivisionController();
				avAlert.Show();
			}
		}

		public void InitializeList(List<SubdivisionDTO> subdivisions, SubdivisionDTO selectedSubdivision)
		{
			var pickerModel = new CustomPickerModel(subdivisions.Select(a => a.DisplayName).ToList());
			this.PickerView.Model = pickerModel;
			this.PickerView.Select(subdivisions.IndexOf(selectedSubdivision), 0, false);
			pickerModel.ItemSelected += (pickerView, row, component) => { model.SelectItem((int)row); MoreButton.Hidden = false; };
			if (subdivisions?.Count > 0 && selectedSubdivision != null)
			{
				MoreButton.Hidden = false;
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

		public void SetMapLocation(double latitude, double longitude)
		{
			View.LayoutSubviews();
			var camera = CameraPosition.FromCamera(latitude, longitude, 15);
			mapView.Camera = camera;
			marker.Position = camera.Target;
		}

		partial void SaveButtonTouched(UIButton sender)
		{
			if (model.SelectedSubdivision != null)
				this.OnDismissed?.Invoke(model.SelectedSubdivision);
			DismissViewController(true, null);
		}

		partial void CloseButtonTouched(UIBarButtonItem sender)
		{
			DismissViewController(true, null);
		}

		partial void AddButtonTouched(UIButton sender)
		{
			GoToAddSubdivisionController();
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

		partial void MoreButtonTouched(UIButton sender)
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

		void GoToAddSubdivisionController()
		{
			var controller = new SubdivisionController();
			controller.AllowEdititng = true;
			controller.ZipCode = this.ZipCode;
			controller.OnDismissed += model.OnSubdivisionCreated;
			this.PresentViewController(controller, true, null);
		}
	}
}

