using System;
using Mehspot.Core.Contracts.Wrappers;
using Mehspot.Core.DTO.Subdivision;

namespace Mehspot.Core.Contracts.ViewControllers
{

    public interface IVerifySubdivisionController
    {
        IViewHelper ViewHelper { get; }
        SubdivisionDTO Subdivision { get; set; }
        string ZipCode { get; }
        bool MarkerDraggable { get; set; }
        bool SaveButtonEnabled { get; set; }
        Action<SubdivisionDTO, bool> OnDismissed { get; }

        void DisplayCells();
        void LoadPlaceByCoordinates(double latitude, double longitude);
        void ShowLocation(double latitude, double longitude);
        void OnSubdivisionVerified(SubdivisionDTO dto, bool isNewOption);
    }
}
