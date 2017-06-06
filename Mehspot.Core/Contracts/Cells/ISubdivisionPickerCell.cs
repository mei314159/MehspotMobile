using System.Collections.Generic;
using Mehspot.Core.DTO.Subdivision;

namespace Mehspot.Core.Builders
{

    public interface ISubdivisionPickerCell : IViewCell
    {
        string FieldName { get; set; }

        bool IsReadOnly { get; set; }

        string ZipCode { get; set; }

        List<SubdivisionDTO> Subdivisions { get; set; }
    }
}