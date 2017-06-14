using System;
using Mehspot.Core.Contracts.Wrappers;
using Mehspot.Core.DTO.Subdivision;

namespace Mehspot.Core.Contracts.ViewControllers
{
    public interface ISubdivisionController
    {
        IViewHelper ViewHelper { get; }
        SubdivisionDTO Subdivision { get; }
        string ZipCode { get; }
        bool AllowEdititng { get; }

        string NameFieldText { get; set; }
        string AddressFieldText { get; set; }
        string LatitudeFieldText { get; set; }
        string LongitudeFieldText { get; set; }

        bool NameFieldEnabled { get; set; }
        bool AddressFieldEnabled { get; set; }
        bool LatitudeFieldEnabled { get; set; }
        bool LongitudeFieldEnabled { get; set; }
        bool SaveButtonEnabled { get; set; }
        bool MarkerDraggable { get; set; }

        void SetMapLocation(double latitude, double longitude, bool setMapOnly = false);

        void DetectUserPosition(SetPositionDelegate onSuccess, Action onError);

        void LoadPlaceByCoordinates(double latitude, double longitude);
        void DismissViewController(EditSubdivisionDTO dto);
    }
}
