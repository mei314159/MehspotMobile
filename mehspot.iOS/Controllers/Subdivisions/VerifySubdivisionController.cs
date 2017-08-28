﻿using System;
using CoreLocation;
using Foundation;
using Google.Maps;
using Mehspot.iOS.Wrappers;
using Mehspot.Core;
using Mehspot.Core.Services;
using Mehspot.Core.DTO.Subdivision;
using UIKit;
using Mehspot.iOS.Extensions;
using Mehspot.Core.Contracts.ViewControllers;
using Mehspot.Core.Contracts.Wrappers;
using Mehspot.Core.Models.Subdivisions;
using Mehspot.iOS.Core.Builders;
using Mehspot.Core.Builders;
using System.Linq;
using System.Threading.Tasks;

namespace Mehspot.iOS.Controllers
{
    public partial class VerifySubdivisionController : UIViewController, IUITableViewDataSource, IUITableViewDelegate,
    IVerifySubdivisionController
    {
        private VerifySubdivisionModel<UITableViewCell> model;

        MapView mapView;
        Marker marker;
        Geocoder geocoder = new Geocoder();

        public Action<SubdivisionDTO, bool> OnDismissed { get; set; }

        public VerifySubdivisionController() : base("VerifySubdivisionController", NSBundle.MainBundle)
        {
        }

        public IViewHelper ViewHelper { get; private set; }
        public SubdivisionDTO Subdivision { get; set; }
        public string ZipCode { get; set; }

        public bool MarkerDraggable
        {
            get
            {
                return marker.Draggable;
            }

            set
            {
                marker.Draggable = value;
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

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.View.AddGestureRecognizer(new UITapGestureRecognizer(this.HideKeyboard));
            this.MainTable.TableFooterView = new UIView();
            MainTable.DataSource = this;
            MainTable.Delegate = this;
            MainTable.RowHeight = UITableView.AutomaticDimension;
            MainTable.EstimatedRowHeight = 44;
            MainTable.ClipsToBounds = true;
            MainTable.AddObserver(this, (NSString) "contentSize", NSKeyValueObservingOptions.Old | NSKeyValueObservingOptions.New, (IntPtr)1);
            ViewHelper = new ViewHelper(this.View);

            mapView = new MapView(MapWrapperView.Bounds);
            mapView.DraggingMarkerStarted += MapView_DraggingMarkerStarted;
            mapView.DraggingMarkerEnded += MapView_DraggingMarkerEnded;

            marker = new Marker();
            marker.Map = mapView;
            marker.Draggable = false;
            MapWrapperView.AddSubview(mapView);
            MapWrapperView.AddConstraint(NSLayoutConstraint.Create(mapView, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, MapWrapperView, NSLayoutAttribute.Trailing, 1, 0));
            MapWrapperView.AddConstraint(NSLayoutConstraint.Create(mapView, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, MapWrapperView, NSLayoutAttribute.Trailing, 1, 0));
            MapWrapperView.AddConstraint(NSLayoutConstraint.Create(mapView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, MapWrapperView, NSLayoutAttribute.Top, 1, 0));
            MapWrapperView.AddConstraint(NSLayoutConstraint.Create(mapView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, MapWrapperView, NSLayoutAttribute.Bottom, 1, 0));
            mapView.AutoresizingMask = UIViewAutoresizing.All;
            MapWrapperView.AutoresizingMask = UIViewAutoresizing.All;
            model = new VerifySubdivisionModel<UITableViewCell>(this, new SubdivisionService(MehspotAppContext.Instance.DataStorage), new IosCellBuilder());
            model.Change += UpdateTableSize;
        }

        public override async void ViewDidAppear(bool animated)
        {
            await model.InitializeAsync();
        }

        public void DisplayCells()
        {
            UIView.Animate(0, MainTable.ReloadData, UpdateTableSize);
			var option = model.options?.FirstOrDefault(a => a.Id == model.addressOptions[0].Key.Value);
            if (option != null)
		        this.ShowLocation(option.Address.Latitude, option.Address.Longitude);
        }

        public override void ObserveValue(NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
        {
            if ((int)context == 1)
            {
                UpdateTableSize();
            }
            else
            {
                base.ObserveValue(keyPath, ofObject, change, context);
            }
        }

        private void UpdateTableSize()
        {
            MainTableHeight.Constant = MainTable.ContentSize.Height;
            View.LayoutIfNeeded();
        }

        void MapView_DraggingMarkerStarted(object sender, GMSMarkerEventEventArgs e)
        {
            model.MarkerDraggingStarted();
        }

        void MapView_DraggingMarkerEnded(object sender, GMSMarkerEventEventArgs e)
        {
            model.MarkerDraggingEnded(e.Marker.Position.Latitude, e.Marker.Position.Longitude);
        }

        void HandleReverseGeocodeCallback(ReverseGeocodeResponse response, NSError error)
        {
            if (error != null )
                return;

			var firstResult = response?.FirstResult;
            if (firstResult != null)
            {
                model.ReverseGeocodeCallback(firstResult.Coordinate.Latitude, firstResult.Coordinate.Longitude, firstResult.Country, firstResult.PostalCode, firstResult?.Lines);
            }

			ViewHelper.HideOverlay();
        }

        async partial void SaveButtonTouched(UIBarButtonItem sender)
        {
            await model.SaveAsync();
        }

        partial void CloseButtonTouched(UIBarButtonItem sender)
        {
            DismissViewController(true, null);
        }

        public void LoadPlaceByCoordinates(double latitude, double longitude)
        {
            geocoder.ReverseGeocodeCord(new CLLocationCoordinate2D(latitude, longitude), HandleReverseGeocodeCallback);
        }

        public nint RowsInSection(UITableView tableView, nint section)
        {
            return model.Sections[(int)section].Rows.Count;
        }

        public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            UITableViewCell uITableViewCell = model.Sections[indexPath.Section].Rows[indexPath.Row];
            return uITableViewCell;
        }

        [Export("tableView:titleForHeaderInSection:")]
        public string TitleForHeader(UITableView tableView, nint section)
        {
            return model.Sections[(int)section].Name;
        }

        [Export("numberOfSectionsInTableView:")]
        public nint NumberOfSections(UITableView tableView)
        {
            return model?.Sections?.Count ?? 0;
        }

        public void ShowLocation(double latitude, double longitude)
        {
            var camera = CameraPosition.FromCamera(latitude, longitude, 15);
            mapView.Camera = camera;
            marker.Position = camera.Target;
        }

        public void OnSubdivisionVerified(SubdivisionDTO dto, bool verified)
        {
            InvokeOnMainThread(() =>
            {
                DismissViewController(true, null);
                OnDismissed?.Invoke(dto, verified);
            });
        }
    }
}
