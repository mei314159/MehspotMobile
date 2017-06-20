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

    public class SectionModel<TView>
    {
        public SectionModel(string name)
        {
            Name = name;
            Rows = new List<TView>();
        }

        public string Name { get; set; }

        public List<TView> Rows { get; set; }
    }
}
