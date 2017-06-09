using System;
using System.Linq;
using Mehspot.Core.Contracts.ViewControllers;
using Mehspot.Core.DTO.Subdivision;

namespace Mehspot.Core.Models.Subdivisions
{
    public class SubdivisionsListModel
    {
        public SubdivisionDTO selectedSubdivision;
        private readonly ISubdivisionsListController controller;

        public SubdivisionsListModel(ISubdivisionsListController controller)
        {
            this.controller = controller;
        }

        public void Initialize()
        {
            if (this.controller.Subdivisions != null && this.controller.Subdivisions.Count > 0)
            {
                selectedSubdivision =
                    this.controller.Subdivisions?.FirstOrDefault(a => a.Id == this.controller.SelectedSubdivisionId) ??
                    this.controller.Subdivisions?.FirstOrDefault();

                if (selectedSubdivision != null)
                {
                    controller.InitializeList(this.controller.Subdivisions, selectedSubdivision);
                    controller.SetMapLocation(selectedSubdivision.Latitude, selectedSubdivision.Longitude);
                }
            }
            else
            {
                controller.DetectUserPosition(
                    controller.SetMapLocation,
                    () => controller.SetMapLocation(Constants.Location.DefaultLatitude, Constants.Location.DefaultLongitude));
            }
        }

        public void RefreshMap()
        {
            if (selectedSubdivision != null)
            {
                controller.SetMapLocation(selectedSubdivision.Latitude, selectedSubdivision.Longitude);
            }
        }

        public void SelectItem(int row)
        {
            selectedSubdivision = controller.Subdivisions.ElementAtOrDefault(row);
            this.RefreshMap();
        }

        public void OnSubdivisionCreated(EditSubdivisionDTO result)
        {
            selectedSubdivision = new SubdivisionDTO();
            UpdateDTO(selectedSubdivision, result, false);
            this.controller.Subdivisions.Add(selectedSubdivision);

            controller.InitializeList(this.controller.Subdivisions, selectedSubdivision);
        }

        public void OnSubdivisionUpdated(EditSubdivisionDTO result)
        {
            UpdateDTO(selectedSubdivision, result, true);
            controller.InitializeList(this.controller.Subdivisions, selectedSubdivision);
        }

        public void OnSubdivisionVerified(SubdivisionDTO result, bool isNewOption)
        {
            selectedSubdivision.IsVerifiedByCurrentUser = true;
            if (isNewOption)
            {
                selectedSubdivision = new SubdivisionDTO();
                this.controller.Subdivisions.Add(selectedSubdivision);
            }

            UpdateDTO(selectedSubdivision, result);
            controller.InitializeList(this.controller.Subdivisions, selectedSubdivision);
        }

        private void UpdateDTO(SubdivisionDTO dto, EditSubdivisionDTO result, bool isVerified)
        {
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

        private void UpdateDTO(SubdivisionDTO dto, SubdivisionDTO result)
        {
            dto.Id = result.Id;
            if (result.IsVerified)
            {
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



