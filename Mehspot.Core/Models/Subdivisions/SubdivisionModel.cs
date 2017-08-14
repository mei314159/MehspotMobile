using System;
using System.Linq;
using System.Threading.Tasks;
using Mehspot.Core.Contracts.ViewControllers;
using Mehspot.Core.DTO;
using Mehspot.Core.DTO.Subdivision;
using Mehspot.Core.Services;

namespace Mehspot.Core.Models.Subdivisions
{
    public class SubdivisionModel
    {
        private readonly EditSubdivisionDTO DTO;
        private readonly ISubdivisionController controller;
        private readonly SubdivisionService subdivisionService;

        public bool SkipFieldsUpdate;

        public SubdivisionModel(ISubdivisionController controller, SubdivisionService subdivisionService)
        {
            this.controller = controller;
            this.subdivisionService = subdivisionService;
            DTO = new EditSubdivisionDTO
            {
                Id = controller.Subdivision?.Id ?? default(int),
                Name = controller.Subdivision?.DisplayName,
                OptionId = controller.Subdivision?.OptionId,
                ZipCode = controller.ZipCode,
                Address = new AddressDTO
                {
                    Latitude = controller.Subdivision?.Latitude ?? 0,
                    Longitude = controller.Subdivision?.Longitude ?? 0,
                    FormattedAddress = this.controller.Subdivision?.FormattedAddress,
                    PostalCode = this.controller.ZipCode,
                    GoverningDistrictId = 1
                }
            };
        }

        public void Initialize()
        {
            if (controller.Subdivision == null)
            {
                controller.DetectUserPosition(
                (latitude, longitude) => controller.SetMapLocation(latitude, longitude),
                () => controller.SetMapLocation(Constants.Location.DefaultLatitude, Constants.Location.DefaultLongitude));
            }
            else
            {

                controller.NameFieldText = DTO.Name;
                controller.AddressFieldText = DTO.Address.FormattedAddress;
                controller.LatitudeFieldText = DTO.Address.Latitude.ToString();
                controller.LongitudeFieldText = DTO.Address.Longitude.ToString();
                controller.SetMapLocation(DTO.Address.Latitude, DTO.Address.Longitude, true);
            }

            controller.SaveButtonEnabled =
            controller.NameFieldEnabled =
            controller.AddressFieldEnabled =
            controller.LatitudeFieldEnabled =
            controller.LongitudeFieldEnabled =
            controller.MarkerDraggable = controller.AllowEdititng;
        }

        public void MarkerDraggingStarted()
        {
            this.controller.AddressFieldEnabled = controller.LatitudeFieldEnabled = controller.LongitudeFieldEnabled = false;
        }

        public void MarkerDraggingEnded(double latitude, double longitude)
        {
            controller.ViewHelper.ShowOverlay("Wait...");
            controller.LoadPlaceByCoordinates(latitude, longitude);
        }

        public void SetMarkerByPress(double latitude, double longitude)
        {
            controller.ViewHelper.ShowOverlay("Wait...");
            controller.LoadPlaceByCoordinates(latitude, longitude);
        }

        public void ReverseGeocodeCallback(double latitude, double longitude, string country, string name, params string[] addressLines)
        {
            controller.AddressFieldEnabled =
            controller.LatitudeFieldEnabled =
            controller.LongitudeFieldEnabled = controller.AllowEdititng;

            this.DTO.Address.Latitude = latitude;
            this.DTO.Address.Longitude = longitude;
            this.DTO.Address.FormattedAddress = addressLines != null ? string.Join(", ", addressLines) : string.Empty;
            if (!string.IsNullOrWhiteSpace(country))
            {
                this.DTO.Address.Country = country;
            }

            if (string.IsNullOrWhiteSpace(controller.NameFieldText))
            {
                this.DTO.Name = name;
                controller.NameFieldText = DTO.Name;
            }

            controller.AddressFieldText = DTO.Address.FormattedAddress;
            controller.LatitudeFieldText = DTO.Address.Latitude.ToString();
            controller.LongitudeFieldText = DTO.Address.Longitude.ToString();

            controller.ViewHelper.HideOverlay();
        }

        public async Task SaveAsync()
        {
            DTO.Name = controller.NameFieldText;
            DTO.Address.FormattedAddress = controller.AddressFieldText;
            double latitude, longitude;
            DTO.Address.Latitude = double.TryParse(controller.LatitudeFieldText, out latitude) ? latitude : DTO.Address.Latitude;
            DTO.Address.Longitude = double.TryParse(controller.LongitudeFieldText, out longitude) ? longitude : DTO.Address.Longitude;

            if (string.IsNullOrWhiteSpace(DTO.Name))
            {
                controller.ViewHelper.ShowAlert("Error", "Enter subdivision name");
            }
            else if (string.IsNullOrWhiteSpace(DTO.Address.FormattedAddress))
            {
                controller.ViewHelper.ShowAlert("Error", "Enter subdivision address");
            }
            else
            {
                controller.ViewHelper.ShowOverlay("Saving...");

                Result result;
                if (controller.Subdivision != null)
                {
                    var overrideResult = await this.subdivisionService.OverrideAsync(DTO).ConfigureAwait(false);
                    result = overrideResult;
                    if (overrideResult.IsSuccess)
                    {
                        DTO.Id = overrideResult.Data.Id;
                    }
                }
                else
                {
                    var createResult = await this.subdivisionService.CreateAsync(DTO).ConfigureAwait(false);
                    result = createResult;
                    if (createResult.IsSuccess)
                    {
                        DTO.Id = createResult.Data.SubdivisionId;
                        DTO.OptionId = createResult.Data.OptionId;
                    }

                }

                controller.ViewHelper.HideOverlay();
                if (result.IsSuccess)
                {
                    controller.DismissViewController(DTO);
                }
                else
                {
                    var error = result.ModelState?.ModelState?.Select(a => a.Value?.FirstOrDefault())?.FirstOrDefault();
                    var message = error != null ? error : result.ErrorMessage;
                    var title = error != null ? MehspotResources.ValidationError : result.ErrorMessage;
                    controller.ViewHelper.ShowAlert(title, message);
                }
            }
        }
    }
}










