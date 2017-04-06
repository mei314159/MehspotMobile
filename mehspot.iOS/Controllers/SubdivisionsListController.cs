using System;
using System.Linq;
using Foundation;
using Google.Maps;
using Mehspot.DTO;
using SharpMobileCode.ModalPicker;
using UIKit;

namespace mehspot.iOS.Controllers
{
    public partial class SubdivisionsListController : UIViewController
    {
        MapView mapView;
        Marker marker;
        SubdivisionDTO selectedSubdivision;

        public event Action<SubdivisionDTO> OnDismissed;

        public SubdivisionsListController () : base ("SubdivisionsListController", NSBundle.MainBundle)
        {
        }


        public SubdivisionDTO [] Subdivisions { get; set; }

        public int? SelectedSubdivisionId { get; set; }

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();
            nint selectedRow = 0;

            if (Subdivisions != null) {
                for (int i = 0; i < Subdivisions.Length; i++) {
                    var item = Subdivisions [i];
                    if (item.Id == SelectedSubdivisionId) {
                        selectedRow = i;
                        selectedSubdivision = item;
                        break;
                    }
                }
                var model = new CustomPickerModel (Subdivisions.Select (a => a.DisplayName).ToList ());
                this.PickerView.Model = model;
                this.PickerView.Select (selectedRow, 0, false);
                model.ItemSelected += Selected;
            }

            CameraPosition camera;
            if (selectedSubdivision == null) {
                camera = CameraPosition.FromCamera (37.79, 32.40, 6);
            } else {
                camera = CameraPosition.FromCamera (selectedSubdivision.Latitude, selectedSubdivision.Longitude, 15);
            }
            mapView = MapView.FromCamera (MapWrapperView.Bounds, camera);
            mapView.MyLocationEnabled = true;

            marker = Marker.FromPosition (camera.Target);
            marker.Map = mapView;

            MapWrapperView.AddSubview (mapView);
        }

        public override void ViewWillAppear (bool animated)
        {
            base.ViewWillAppear (animated);
            mapView.StartRendering ();
        }
        public override void ViewWillDisappear (bool animated)
        {
            mapView.StopRendering ();
            base.ViewWillDisappear (animated);
        }

        public void Selected (UIPickerView pickerView, nint row, nint component)
        {
            selectedSubdivision = Subdivisions [row];
            var camera = CameraPosition.FromCamera (selectedSubdivision.Latitude, selectedSubdivision.Longitude, 15);
            mapView.Camera = camera;
            marker.Position = camera.Target;
        }

        public override void DidReceiveMemoryWarning ()
        {
            base.DidReceiveMemoryWarning ();
            // Release any cached data, images, etc that aren't in use.
        }

        partial void SaveButtonTouched (UIBarButtonItem sender)
        {
            this.OnDismissed (selectedSubdivision);
            DismissViewController (true, null);
        }

        partial void CloseButtonTouched (UIBarButtonItem sender)
        {
            DismissViewController (true, null);
        }

        partial void AddButtonTouched (UIBarButtonItem sender)
        {
            var subdivisionOptionsActionSheet = new UIActionSheet ("Options");
            subdivisionOptionsActionSheet.AddButton ("Add Subdivision");
            subdivisionOptionsActionSheet.AddButton ("Verify/Change Subdivision");
            bool userIsAdmin = true; //TODO Temporary. Need to get real user status
            if (userIsAdmin)
                subdivisionOptionsActionSheet.AddButton ("Edit Subdivision");
            subdivisionOptionsActionSheet.AddButton ("Cancel");
            subdivisionOptionsActionSheet.CancelButtonIndex = 3;
            subdivisionOptionsActionSheet.Clicked += SubdivisionOptionsActionSheet_Clicked;
            subdivisionOptionsActionSheet.ShowInView (View);
        }

        void SubdivisionOptionsActionSheet_Clicked (object sender, UIButtonEventArgs e)
        {
            if (e.ButtonIndex == 0) { //Add subdivision
                var controller = new SubdivisionController ();
                this.PresentViewController (controller, true, null);
            } else if (e.ButtonIndex == 1) { //Verify/Change subdivision
                var controller = new SubdivisionController ();
                controller.Subdivision = this.selectedSubdivision;
                this.PresentViewController (controller, true, null);
            } else if (e.ButtonIndex == 2) { //Edit subdivision
                var controller = new SubdivisionController ();
                controller.Subdivision = this.selectedSubdivision;
                this.PresentViewController (controller, true, null);
            } else {
                return;
            }
        }
    }
}

