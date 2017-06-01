﻿using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using Google.Maps;
using Mehspot.Core;
using Mehspot.iOS.Views.CustomPicker;
using Mehspot.Core.DTO.Subdivision;
using UIKit;

namespace Mehspot.iOS.Controllers
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


        public List<SubdivisionDTO> Subdivisions { get; set; }
        public string ZipCode { get; set; }

        public int? SelectedSubdivisionId { get; set; }

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();
            var selectedRow = 0;
            if (Subdivisions != null) {
                for (int i = 0; i < Subdivisions.Count; i++) {
                    var item = Subdivisions [i];
                    if (item.Id == SelectedSubdivisionId) {
                        selectedRow = i;
                        selectedSubdivision = item;
                        break;
                    }
                }
                if (selectedSubdivision == null){
                    selectedSubdivision = Subdivisions[selectedRow];
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

        public override void ViewDidAppear (bool animated)
        {
            base.ViewDidAppear (animated);
            if (this.selectedSubdivision != null)
                RefreshMap (this.selectedSubdivision);
        }

        public void Selected (UIPickerView pickerView, nint row, nint component)
        {
            selectedSubdivision = Subdivisions.ElementAtOrDefault ((int)row);
            RefreshMap (selectedSubdivision);
        }

        private void RefreshMap (SubdivisionDTO subdivision) {
            var camera = CameraPosition.FromCamera (subdivision.Latitude, subdivision.Longitude, 15);
            mapView.Camera = camera;
            marker.Position = camera.Target;
        }

        partial void SaveButtonTouched (UIBarButtonItem sender)
        {
            this.OnDismissed?.Invoke (selectedSubdivision);
            DismissViewController (true, null);
        }

        partial void CloseButtonTouched (UIBarButtonItem sender)
        {
            DismissViewController (true, null);
        }

        partial void AddButtonTouched (UIBarButtonItem sender)
        {
            var controller = new SubdivisionController ();
            controller.AllowEdititng = true;
            controller.ZipCode = this.ZipCode;
            controller.OnDismissed += SubdivisionCreated;
            this.PresentViewController (controller, true, null);
        }

        void SubdivisionOptionsActionSheet_Clicked (object sender, UIButtonEventArgs e)
        {
            if (e.ButtonIndex != 0)
                return;

            if (!selectedSubdivision.IsVerified && !selectedSubdivision.IsVerifiedByCurrentUser) {
                var controller = new VerifySubdivisionController ();
                controller.Subdivision = selectedSubdivision;
                controller.ZipCode = this.ZipCode;
                controller.SubdivisionVerified += SubdivisionVerified;
                this.PresentViewController (controller, true, null);
            } else {
                var controller = new SubdivisionController ();
                controller.Subdivision = this.selectedSubdivision;
                controller.ZipCode = this.ZipCode;
                controller.AllowEdititng = MehspotAppContext.Instance.AuthManager.AuthInfo.IsAdmin;
                controller.OnDismissed += SubdivisionUpdated;
                this.PresentViewController (controller, true, null);
            }
        }

        partial void MoreButtonTouched (UIBarButtonItem sender)
        {
            var subdivisionOptionsActionSheet = new UIActionSheet ("Options");
            if (selectedSubdivision != null && !selectedSubdivision.IsVerified && !selectedSubdivision.IsVerifiedByCurrentUser) {
                subdivisionOptionsActionSheet.AddButton ("Verify or Change Subdivision");
            } else {
                subdivisionOptionsActionSheet.AddButton (MehspotAppContext.Instance.AuthManager.AuthInfo.IsAdmin ? "Edit Subdivision" : "View Details");
            }

            subdivisionOptionsActionSheet.AddButton ("Cancel");
            subdivisionOptionsActionSheet.CancelButtonIndex = 1;
            subdivisionOptionsActionSheet.Clicked += SubdivisionOptionsActionSheet_Clicked;
            subdivisionOptionsActionSheet.ShowInView (View);
        }

        void SubdivisionCreated (EditSubdivisionDTO result)
        {
            var items = ((CustomPickerModel)this.PickerView.Model).Items;

            selectedSubdivision = new SubdivisionDTO ();
            UpdateDTO (selectedSubdivision, result, false);
            this.Subdivisions.Add (selectedSubdivision);
            items.Add (result.Name);
            this.PickerView.ReloadAllComponents ();
            this.PickerView.Select (items.Count - 1, 0, false);
        }

        void SubdivisionUpdated (EditSubdivisionDTO result)
        {
            var items = ((CustomPickerModel)this.PickerView.Model).Items;

            var index = items.IndexOf (selectedSubdivision.DisplayName);
            items.RemoveAt (index);
            items.Insert (index, result.Name);
            UpdateDTO (selectedSubdivision, result, true);
            this.PickerView.ReloadAllComponents ();
            this.PickerView.Select (index, 0, false);
        }

        void SubdivisionVerified (SubdivisionDTO result, bool isNewOption)
        {
            var items = ((CustomPickerModel)this.PickerView.Model).Items;
            selectedSubdivision.IsVerifiedByCurrentUser = true;
            int index;
            if (isNewOption) {
                selectedSubdivision = new SubdivisionDTO ();
                UpdateDTO (selectedSubdivision, result);
                this.Subdivisions.Add (selectedSubdivision);
                index = items.Count - 1;
            } else {
                index = items.IndexOf (selectedSubdivision.DisplayName);
                items.RemoveAt (index);
                items.Insert (index, result.DisplayName);
                UpdateDTO (selectedSubdivision, result);    
            }

            this.PickerView.ReloadAllComponents ();
            this.PickerView.Select (index, 0, false);
        }

        private void UpdateDTO (SubdivisionDTO dto, EditSubdivisionDTO result, bool isVerified) {
            dto.Id = result.Id;
            dto.OptionId = result.OptionId;
            dto.DisplayName = result.Name;
            dto.Latitude = result.Address.Latitude;
            dto.Longitude = result.Address.Longitude;
            dto.FormattedAddress = result.Address.FormattedAddress;
            dto.IsVerified = isVerified;
            dto.IsVerifiedByCurrentUser = false;
            dto.ZipCode = result.ZipCode;
            dto.SubdivisionIdentifier = result.SubdivisionIdentifier;
            dto.AddressId = result.AddressId;
        }

        private void UpdateDTO (SubdivisionDTO dto, SubdivisionDTO result)
        {
            dto.Id = result.Id;
            if (result.IsVerified) {
                dto.DisplayName = result.DisplayName;
            }

            dto.OptionId = result.OptionId;
            dto.Latitude = result.Latitude;
            dto.Longitude = result.Longitude;
            dto.FormattedAddress = result.FormattedAddress;
            dto.IsVerified = result.IsVerified;
            dto.IsVerifiedByCurrentUser = result.IsVerifiedByCurrentUser;
            dto.ZipCode = result.ZipCode;
            dto.SubdivisionIdentifier = result.SubdivisionIdentifier;
            dto.AddressId = result.AddressId;
        }
    }
}

