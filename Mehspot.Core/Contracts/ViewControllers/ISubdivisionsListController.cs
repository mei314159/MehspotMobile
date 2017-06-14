using System;
using System.Collections.Generic;
using Mehspot.Core.DTO.Subdivision;

namespace Mehspot.Core.Contracts.ViewControllers
{
    public delegate void SetPositionDelegate(double latitude, double longitude);
    public interface ISubdivisionsListController
    {
        List<SubdivisionDTO> Subdivisions { get; }
        string ZipCode { get; }
        int? SelectedSubdivisionId { get; }

        void SetMapLocation(double latitude, double longitude);

        void InitializeList(List<SubdivisionDTO> subdivisions, SubdivisionDTO selectedSubdivision);

        void DetectUserPosition(SetPositionDelegate onSuccess, Action onError);
    }
}
