using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using Mehspot.iOS.Views;
using Mehspot.Core.Extensions;
using Mehspot.Core.DTO.Subdivision;
using UIKit;

namespace Mehspot.iOS.Controllers.Subdivisions
{

    public class VerifySubdivisionModel
    {
        public VerifySubdivisionModel (int? nameOptionId, int? locationOptionId)
        {
            NameOptionId = nameOptionId;
            AddressOptionId = locationOptionId;
        }

        public int? NameOptionId { get; set; }

        public int? AddressOptionId { get; set; }

        public string NewName { get; set; }
    }
}
