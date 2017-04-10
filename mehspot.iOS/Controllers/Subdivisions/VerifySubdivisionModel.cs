using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using mehspot.iOS.Views;
using Mehspot.Core.Extensions;
using MehSpot.Core.DTO.Subdivision;
using UIKit;

namespace mehspot.iOS.Controllers.Subdivisions
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
