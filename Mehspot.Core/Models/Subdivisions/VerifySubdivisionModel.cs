using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mehspot.Core.Builders;
using Mehspot.Core.Contracts.ViewControllers;
using Mehspot.Core.DTO;
using Mehspot.Core.DTO.Subdivision;
using Mehspot.Core.Extensions;
using Mehspot.Core.Services;
using Mehspot.iOS.Controllers.Subdivisions;

namespace Mehspot.Core.Models.Subdivisions
{
    public class VerifySubdivisionModel<TCell>
    {
        private readonly IVerifySubdivisionController controller;
        private readonly SubdivisionService subdivisionService;
        private readonly CellBuilder<TCell> cellBuilder;

        public VerifySubdivisionResult Result { get; set; }
        public List<SectionModel<TCell>> Sections;

        private TCell namePickerCell;
        private ITextEditCell otherNameCell;
        private TCell addressPickerCell;
        private ITextEditCell otherAddressCell;
		public List<SubdivisionOptionDTO> options;
		public List<KeyValuePair<int?, string>> nameOptions;
		public List<KeyValuePair<int?, string>> addressOptions;
        double Latitude;
        double Longitude;
        string Country;
        string PostalCode;

        public event Action Change;

        public VerifySubdivisionModel(IVerifySubdivisionController controller, SubdivisionService subdivisionService, CellBuilder<TCell> cellBuilder)
        {
            this.cellBuilder = cellBuilder;
            this.controller = controller;
            this.subdivisionService = subdivisionService;
            this.Sections = new List<SectionModel<TCell>>();
        }

        public async Task InitializeAsync()
        {
            controller.ViewHelper.ShowOverlay("Loading");
            var optionsResult = await subdivisionService.ListOptionsAsync(controller.Subdivision.Id);
            if (optionsResult.IsSuccess)
            {
                options = optionsResult.Data;
                this.InitializeInternal();

                controller.DisplayCells();
                this.controller.SaveButtonEnabled = true;
                controller.ViewHelper.HideOverlay();
            }
            else if (!optionsResult.IsNetworkIssue)
            {
                controller.ViewHelper.HideOverlay();
                controller.ViewHelper.ShowAlert("Error", optionsResult.ErrorMessage);
            }
        }

        public void SetMarkerByPress(double latitude, double longitude)
        {
            controller.ViewHelper.ShowOverlay("Wait...");
            controller.LoadPlaceByCoordinates(latitude, longitude);
        }

        public void MarkerDraggingStarted()
        {
            this.otherAddressCell.Editable = false;
        }

        public void MarkerDraggingEnded(double latitude, double longitude)
        {
            controller.ViewHelper.ShowOverlay("Wait...");
            controller.LoadPlaceByCoordinates(latitude, longitude);
        }

        public async Task SaveAsync()
        {
            controller.ViewHelper.ShowOverlay("Saving...");

            Result<SubdivisionDTO> result;
            var verify = Result.NameOptionId.HasValue && Result.AddressOptionId.HasValue;
            SubdivisionOptionDTO dto = null;
            if (verify)
            {
                result = await this.subdivisionService.VerifyOptionAsync(Result.NameOptionId.Value).ConfigureAwait(false);
            }
            else
            {
                var zip = controller.ZipCode ?? this.PostalCode;
                dto = new SubdivisionOptionDTO();
                dto.Name = Result.NameOptionId.HasValue ? options.First(a => a.Id == Result.NameOptionId).Name : Result.NewName;

                if (string.IsNullOrWhiteSpace(dto.Name))
                {
                    controller.ViewHelper.ShowAlert("Validation Error", "Subdivision Name can not be empty");
                    controller.ViewHelper.HideOverlay();
                    return;
                }

                if (dto.Name.Length > 128)
                {
                    if (string.IsNullOrWhiteSpace(dto.Name))
                    {
                        controller.ViewHelper.ShowAlert("Validation Error", "Subdivision Name length can not exceed 128 symbols ");
                        controller.ViewHelper.HideOverlay();
                        return;
                    }
                }

                dto.SubdivisionId = controller.Subdivision.Id;
                if (Result.AddressOptionId.HasValue)

                {
                    dto.AddressId = options.First(a => a.Id == Result.AddressOptionId).AddressId;
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(Result.NewAddress))
                    {
                        controller.ViewHelper.ShowAlert("Validation Error", "Address can not be empty");
                        controller.ViewHelper.HideOverlay();
                        return;
                    }

                    dto.Address = new AddressDTO
                    {
                        Latitude = this.Latitude,
                        Longitude = this.Longitude,
                        Country = this.Country,
                        FormattedAddress = Result.NewAddress,
                        PostalCode = zip,
                        GoverningDistrictId = 1,
                    };
                }

                result = await this.subdivisionService.CreateOptionAsync(dto).ConfigureAwait(false);
            }

            controller.ViewHelper.HideOverlay();
            if (result.IsSuccess)
            {
                this.controller.OnSubdivisionVerified(result.Data, !verify);

            }
            else if (!result.IsNetworkIssue)
            {
                controller.ViewHelper.ShowAlert("Error", result.ErrorMessage);
            }
        }

        public void ReverseGeocodeCallback(double latitude, double longitude, string country, string postalCode, params string[] addressLines)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
            this.Country = country;
            this.PostalCode = postalCode;
            this.otherAddressCell.Editable = true;
            otherAddressCell.Text = Result.NewAddress = addressLines != null ? string.Join(", ", addressLines) : string.Empty;

            controller.ViewHelper.HideOverlay();
        }

        private void InitializeInternal()
        {
            this.nameOptions = options.DistinctBy(a => a.Name).Select(a => new KeyValuePair<int?, string>(a.Id, a.Name)).ToList();
            this.addressOptions = options.DistinctBy(a => a.Address.FormattedAddress).Select(a => new KeyValuePair<int?, string>(a.Id, a.Address.FormattedAddress)).ToList();

            var nameOption = nameOptions.First(a => a.Value == controller.Subdivision.DisplayName);
            Result = new VerifySubdivisionResult(nameOption.Key, addressOptions[0].Key);

            var nameSection = new SectionModel<TCell>("Verify Subdivision Name");
            nameOptions.Add(new KeyValuePair<int?, string>(null, "Other"));
            namePickerCell = cellBuilder.GetPickerCell(Result.NameOptionId, NameOptionChanged, "Subdivision Name", nameOptions.ToArray(), "Other");
            nameSection.Rows.Add(namePickerCell);

            otherNameCell = cellBuilder.GetTextEditCell(Result.NewName, (c, a) => { Result.NewName = a; Change?.Invoke(); }, "Other", KeyboardType.Default, "New name");
            otherNameCell.Hidden = Result.NameOptionId.HasValue;
            otherNameCell.Multiline = false;
            nameSection.Rows.Add((TCell)otherNameCell);

            var adressSection = new SectionModel<TCell>("Verify Subdivision Address");
            addressOptions.Add(new KeyValuePair<int?, string>(null, "Other"));
            addressPickerCell = cellBuilder.GetPickerCell(Result.AddressOptionId, AddressOptionChanged, "Subdivision Location", addressOptions.ToArray(), "Other");
            adressSection.Rows.Add(addressPickerCell);

            otherAddressCell = cellBuilder.GetTextEditCell(Result.NewAddress, (c, a) => { Result.NewAddress = a; Change?.Invoke(); }, "Other", KeyboardType.Default, "New address");
            otherAddressCell.Hidden = Result.AddressOptionId.HasValue;
            otherAddressCell.Multiline = false;
            adressSection.Rows.Add((TCell)otherAddressCell);

            Sections.Add(nameSection);
            Sections.Add(adressSection);
        }

        void NameOptionChanged(int? value)
        {
            Result.NameOptionId = value;
            otherNameCell.Hidden = value.HasValue;
            Change?.Invoke();
        }

        void AddressOptionChanged(int? value)
        {
            Result.AddressOptionId = value;
            if (value.HasValue)
            {
                otherAddressCell.Hidden = true;
                var option = options.First(a => a.Id == value.Value);
                this.controller.ShowLocation(option.Address.Latitude, option.Address.Longitude);
                controller.MarkerDraggable = false;
            }
            else
            {
                otherAddressCell.Hidden = false;
                controller.MarkerDraggable = true;
                controller.LoadPlaceByCoordinates(Constants.Location.DefaultLatitude, Constants.Location.DefaultLongitude);
            }
            Change?.Invoke();
        }
    }
}










