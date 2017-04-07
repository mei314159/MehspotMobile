using mehspot.Core.Contracts;
using Mehspot.Core.DTO;
using System.Threading.Tasks;
using mehspot.Core;
using MehSpot.Core.DTO.Subdivision;
using Mehspot.DTO;
using System.Collections.Generic;

namespace Mehspot.Core.Services
{

    public class SubdivisionService : BaseDataService
    {
        public SubdivisionService(IApplicationDataStorage applicationDataStorage) : base(applicationDataStorage)
        {
        }

        public Task<Result<StaticDataDto[]>> GetStatesAsync()
        {
            return this.GetAsync<StaticDataDto[]>("Profile/GetStates");
        }

        public Task<Result<List<SubdivisionDTO>>> GetSubdivisionsAsync(string zip)
        {
            return this.GetAsync<List<SubdivisionDTO>>("Profile/GetSubdivisions?zipCode=" + zip);
        }

        public async Task<Result> CreateAsync(EditSubdivisionDTO subdivision)
        {
            return await PostAsync<object>($"Subdivision/Create", subdivision).ConfigureAwait(false);
        }

        public async Task<Result> OverrideAsync(EditSubdivisionDTO subdivision)
        {
            return await PostAsync<object>($"Subdivision/Override", subdivision).ConfigureAwait(false);
        }
    }
}
