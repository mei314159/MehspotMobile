using System;
using System.Collections.Generic;
using CoreLocation;
using Foundation;
using Google.Maps;
using Mehspot.iOS.Controllers.Subdivisions;
using Mehspot.iOS.Views;
using Mehspot.iOS.Wrappers;
using Mehspot.Core;
using Mehspot.Core.Services;
using Mehspot.Core.DTO.Subdivision;
using UIKit;
using Mehspot.Core.Extensions;
using System.Linq;
using Mehspot.Core.DTO;
using Mehspot.iOS.Extensions;

namespace Mehspot.iOS.Controllers
{
	public partial class VerifySubdivisionController : UIViewController, IUITableViewDataSource, IUITableViewDelegate
	{
		MapView mapView;
		Marker marker;
		Address Place;
		Geocoder geocoder = new Geocoder();
		SubdivisionService subdivisionService;
		ViewHelper viewHelper;

		List<SubdivisionOptionDTO> options;
		private List<Section> sections;
		private PickerCell namePickerCell;
		private TextEditCell otherNameCell;
		private PickerCell addressPickerCell;
		private TextEditCell otherAddressCell;

		public event Action<SubdivisionDTO, bool> SubdivisionVerified;

		public VerifySubdivisionController() : base("VerifySubdivisionController", NSBundle.MainBundle)
		{
		}

		public SubdivisionDTO Subdivision { get; set; }
		public string ZipCode { get; set; }

		public VerifySubdivisionModel Model { get; set; }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			this.View.AddGestureRecognizer(new UITapGestureRecognizer(this.HideKeyboard));
			this.MainTable.TableFooterView = new UIView();
			viewHelper = new ViewHelper(this.View);
			subdivisionService = new SubdivisionService(MehspotAppContext.Instance.DataStorage);

		}

		public override async void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);

			viewHelper.ShowOverlay("Loading");
			var optionsResult = await subdivisionService.ListOptionsAsync(Subdivision.Id);
			if (optionsResult.IsSuccess)
			{
				options = optionsResult.Data;
				Initialize();
				MainTable.DataSource = this;
				MainTable.Delegate = this;
				MainTable.ReloadData();
				viewHelper.HideOverlay();
			}
			else
			{
				viewHelper.HideOverlay();
				viewHelper.ShowAlert("Error", optionsResult.ErrorMessage);
			}
		}

		void MapView_DraggingMarkerStarted(object sender, GMSMarkerEventEventArgs e)
		{
			this.otherAddressCell.TextInput.Enabled = false;
		}

		void MapView_DraggingMarkerEnded(object sender, GMSMarkerEventEventArgs e)
		{
			viewHelper.ShowOverlay("Wait...");
			GetLocationByCoordinates(e.Marker.Position);
		}

		void HandleReverseGeocodeCallback(ReverseGeocodeResponse response, NSError error)
		{
			this.otherAddressCell.TextInput.Enabled = true;

			if (error != null)
				return;

			this.Place = response.FirstResult;
			otherAddressCell.TextInput.Text = Place.Lines != null ? string.Join(", ", response.FirstResult.Lines) : response.FirstResult.Thoroughfare;
			viewHelper.HideOverlay();
		}


		async partial void SaveButtonTouched(UIBarButtonItem sender)
		{
			viewHelper.ShowOverlay("Saving...");

			Result result;
			var verify = Model.NameOptionId.HasValue && Model.AddressOptionId.HasValue;
			if (verify)
			{
				var verifyResult = await this.subdivisionService.VerifyOptionAsync(Model.NameOptionId.Value);
				if (verifyResult.IsSuccess)
				{
					Subdivision = verifyResult.Data;
				}

				result = verifyResult;
			}
			else
			{
				var zip = this.ZipCode ?? Place.PostalCode;
				var dto = new SubdivisionOptionDTO();
				dto.Name = Model.NameOptionId.HasValue ? options.First(a => a.Id == Model.NameOptionId).Name : Model.NewName;

				if (string.IsNullOrWhiteSpace(dto.Name))
				{
					viewHelper.ShowAlert("Validation Error", "Subdivision Name can not be empty");
					return;
				}
				else if (dto.Name.Length > 128)
				{
					if (string.IsNullOrWhiteSpace(dto.Name))
					{
						viewHelper.ShowAlert("Validation Error", "Subdivision Name length can not exceed 128 symbols ");
						return;
					}
				}

				dto.SubdivisionId = this.Subdivision.Id;
				if (Model.AddressOptionId.HasValue)
				{
					dto.AddressId = options.First(a => a.Id == Model.AddressOptionId).AddressId;
				}
				else
				{
					dto.Address = new AddressDTO
					{
						Latitude = Place.Coordinate.Latitude,
						Longitude = Place.Coordinate.Longitude,
						Country = Place.Country,
						FormattedAddress = this.otherAddressCell.TextInput.Text,
						PostalCode = zip,
						GoverningDistrictId = 1,
					};
				}

				result = await this.subdivisionService.CreateOptionAsync(dto);
			}

			viewHelper.HideOverlay();
			if (result.IsSuccess)
			{
				DismissViewController(true, null);
				this.SubdivisionVerified?.Invoke(Subdivision, !verify);

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

		public nint RowsInSection(UITableView tableView, nint section)
		{
			return sections[(int)section].Rows.Count;
		}

		public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			return sections[(int)indexPath.Section].Rows[indexPath.Row];
		}

		[Export("tableView:titleForHeaderInSection:")]
		public string TitleForHeader(UITableView tableView, nint section)
		{
			return sections[(int)section].Name;
		}

		[Export("numberOfSectionsInTableView:")]
		public nint NumberOfSections(UITableView tableView)
		{
			return sections.Count;
		}

		private void Initialize()
		{
			var nameOptions = options.DistinctBy(a => a.Name).Select(a => new KeyValuePair<int?, string>(a.Id, a.Name)).ToList();
			var addressOptions = options.DistinctBy(a => a.Address.FormattedAddress).Select(a => new KeyValuePair<int?, string>(a.Id, a.Address.FormattedAddress)).ToList();

			var nameOption = nameOptions.First(a => a.Key == Subdivision.OptionId);
			Model = new VerifySubdivisionModel(nameOption.Key, addressOptions[0].Key);

			var nameSection = new Section("Verify Subdivision Name");
			nameOptions.Add(new KeyValuePair<int?, string>(null, "Other"));
			namePickerCell = PickerCell.Create(Model.NameOptionId, NameOptionChanged, "Subdivision Name", nameOptions, "Other");
			nameSection.Rows.Add(namePickerCell);

			otherNameCell = TextEditCell.Create(Model.NewName, (c, a) => Model.NewName = a, "Other", "New Subdivision Name");
			otherNameCell.Hidden = Model.NameOptionId.HasValue;
			nameSection.Rows.Add(otherNameCell);

			var adressSection = new Section("Verify Subdivision Address");
			addressOptions.Add(new KeyValuePair<int?, string>(null, "Other"));
			addressPickerCell = PickerCell.Create(Model.AddressOptionId, AddressOptionChanged, "Subdivision Location", addressOptions, "Other");
			adressSection.Rows.Add(addressPickerCell);

			otherAddressCell = TextEditCell.Create(Model.NewName, (c, a) => Model.NewName = a, "Other", "New Subdivision Name");
			otherAddressCell.Hidden = Model.AddressOptionId.HasValue;
			adressSection.Rows.Add(otherAddressCell);


			this.sections = new List<Section>();
			sections.Add(nameSection);
			sections.Add(adressSection);

			mapView = new MapView(MapWrapperView.Bounds);
			mapView.MyLocationEnabled = true;
			mapView.DraggingMarkerStarted += MapView_DraggingMarkerStarted;
			mapView.DraggingMarkerEnded += MapView_DraggingMarkerEnded;

			marker = Marker.FromPosition(mapView.Camera.Target);
			marker.Map = mapView;
			marker.Draggable = true;

			MapWrapperView.AddSubview(mapView);
			SetCamera(addressOptions[0].Key.Value);
		}

		void NameOptionChanged(int? value)
		{
			Model.NameOptionId = value;
			otherNameCell.Hidden = value.HasValue;
		}

		void AddressOptionChanged(int? value)
		{
			Model.AddressOptionId = value;
			if (value.HasValue)
			{
				otherAddressCell.Hidden = true;
				this.SetCamera(value.Value);
				marker.Draggable = false;

			}
			else
			{
				otherAddressCell.Hidden = false;
				marker.Draggable = true;
				GetLocationByCoordinates(marker.Position);
			}
		}

		void SetCamera(int optionId)
		{
			var option = options.First(a => a.Id == optionId);
			var camera = CameraPosition.FromCamera(option.Address.Latitude, option.Address.Longitude, 15);
			mapView.Camera = camera;
			marker.Position = camera.Target;
		}

		class Section
		{
			public Section(string name)
			{
				Name = name;
				Rows = new List<UITableViewCell>();
			}

			public string Name { get; set; }

			public List<UITableViewCell> Rows { get; set; }
		}
	}
}