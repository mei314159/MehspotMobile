using mehspot.Core.Contracts;
using Mehspot.Core.DTO;
using System.Threading.Tasks;
using mehspot.Core;
using MehSpot.Core.DTO.Subdivision;
using System.Collections.Generic;

namespace Mehspot.Core.Services
{

    public class SubdivisionService : BaseDataService
    {
        public SubdivisionService(IApplicationDataStorage applicationDataStorage) : base(applicationDataStorage)
        {
        }

        public Task<Result<StaticDataDTO[]>> ListStatesAsync()
        {
            return this.GetAsync<StaticDataDTO[]>("Subdivision/ListStates");
        }

        public Task<Result<List<SubdivisionDTO>>> ListSubdivisionsAsync(string zip)
        {
            return this.GetAsync<List<SubdivisionDTO>>("Subdivision/List?zipCode=" + zip);
        }

        public Task<Result<List<SubdivisionOptionDTO>>> ListOptionsAsync(int subdivisionId)
        {
            return this.GetAsync<List<SubdivisionOptionDTO>>("Subdivision/ListOptions?subdivisionId=" + subdivisionId);
        }

        public async Task<Result> CreateAsync(EditSubdivisionDTO subdivision)
        {
            return await PostAsync<object>($"Subdivision/Create", subdivision).ConfigureAwait(false);
        }

        public async Task<Result> OverrideAsync(EditSubdivisionDTO subdivision)
        {
            return await PostAsync<object>($"Subdivision/Override", subdivision).ConfigureAwait(false);
        }

        public async Task<Result<SubdivisionDTO>> VerifyOptionAsync(int subdivisionOptionId)
        {
            return await GetAsync<SubdivisionDTO>($"Subdivision/Verify?subdivisionOptionId={subdivisionOptionId}").ConfigureAwait(false);
        }

        public async Task<Result> CreateOptionAsync(SubdivisionOptionDTO subdivisionOption)
        {
            return await PostAsync<object>($"Subdivision/CreateOption", subdivisionOption).ConfigureAwait(false);
        }
    }
}
