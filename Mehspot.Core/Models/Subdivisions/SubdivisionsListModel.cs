using System;
using System.Linq;
using Mehspot.Core.Contracts.ViewControllers;
using Mehspot.Core.DTO.Subdivision;

namespace Mehspot.Core.Models.Subdivisions
{
    public class SubdivisionsListModel
    {
        public SubdivisionDTO SelectedSubdivision;
        private readonly ISubdivisionsListController controller;

        public SubdivisionsListModel(ISubdivisionsListController controller)
        {
            this.controller = controller;
        }

        public void Initialize()
        {
            if (this.controller.Subdivisions != null && this.controller.Subdivisions.Count > 0)
            {
                SelectedSubdivision =
                    this.controller.Subdivisions?
                        .FirstOrDefault(a => 
                                        a.Id == this.controller.SelectedSubdivisionId &&
                                        (this.controller.SelectedSubdivisionOptionId == null ||
                                         a.OptionId == this.controller.SelectedSubdivisionOptionId)) ??
                    this.controller.Subdivisions?.FirstOrDefault();

                if (SelectedSubdivision != null)
                {
                    controller.InitializeList(this.controller.Subdivisions, SelectedSubdivision);
                    controller.SetMapLocation(SelectedSubdivision.Latitude, SelectedSubdivision.Longitude);
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
            if (SelectedSubdivision != null)
            {
                controller.SetMapLocation(SelectedSubdivision.Latitude, SelectedSubdivision.Longitude);
            }
        }

        public void SelectItem(int row)
        {
            SelectedSubdivision = controller.Subdivisions.ElementAtOrDefault(row);
            this.RefreshMap();
        }

        public void SelectItem(SubdivisionDTO dto)
        {
            var index = controller.Subdivisions.IndexOf(dto);
            this.SelectItem(index);
        }

        public void OnSubdivisionCreated(EditSubdivisionDTO result)
        {
            SelectedSubdivision = new SubdivisionDTO();
            UpdateDTO(SelectedSubdivision, result, false);
            this.controller.Subdivisions.Add(SelectedSubdivision);

            controller.InitializeList(this.controller.Subdivisions, SelectedSubdivision);
        }

        public void OnSubdivisionUpdated(EditSubdivisionDTO result)
        {
            UpdateDTO(SelectedSubdivision, result, true);
            controller.InitializeList(this.controller.Subdivisions, SelectedSubdivision);
        }

        public void OnSubdivisionVerified(SubdivisionDTO result, bool isNewOption)
        {
            if (result == null)
                return;
            
            SelectedSubdivision.IsVerifiedByCurrentUser = true;
            if (isNewOption && this.controller.Subdivisions.All(a => a.DisplayName != result.DisplayName))
            {
                SelectedSubdivision = new SubdivisionDTO();
                this.controller.Subdivisions.Add(SelectedSubdivision);
            }

            UpdateDTO(SelectedSubdivision, result);
            controller.InitializeList(this.controller.Subdivisions, SelectedSubdivision);
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
            if (result.IsVerified || dto.Id == default(int))
            {
                dto.DisplayName = result.DisplayName;
            }

            dto.Id = result.Id;
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



