using Mehspot.Core.Contracts;
using Mehspot.Core.DTO;
using System.Threading.Tasks;
using Mehspot.Core.DTO.Subdivision;
using System.Collections.Generic;

namespace Mehspot.Core.Services
{

    public class SubdivisionService : BaseDataService
    {
        private readonly Dictionary<string, Result<List<SubdivisionDTO>>> cache;
        public SubdivisionService(IApplicationDataStorage applicationDataStorage) : base(applicationDataStorage)
        {
            cache = new Dictionary<string, Result<List<SubdivisionDTO>>>();
        }

        public async Task<Result<List<SubdivisionDTO>>> ListSubdivisionsAsync(string zip)
        {
            Result<List<SubdivisionDTO>> result;
            if (cache.ContainsKey(zip))
            {
                result = cache[zip];
            }
            else
            {
                result = await this.GetAsync<List<SubdivisionDTO>>("Subdivision/List?zipCode=" + zip);
                if (result.IsSuccess)
                {
                    cache.Add(zip, result);
                }
            }

            return result;
        }

        public Task<Result<List<SubdivisionOptionDTO>>> ListOptionsAsync(int subdivisionId)
        {
            return this.GetAsync<List<SubdivisionOptionDTO>>("Subdivision/ListOptions?subdivisionId=" + subdivisionId);
        }

        public async Task<Result<CreateSubdivisionResultDTO>> CreateAsync(EditSubdivisionDTO subdivision)
        {
            var result = await PostAsync<CreateSubdivisionResultDTO>($"Subdivision/Create", subdivision).ConfigureAwait(false);
            if (result.IsSuccess && cache.ContainsKey(subdivision.ZipCode))
            {
                cache.Remove(subdivision.ZipCode);
            }

            return result;
        }

        public async Task<Result<EditSubdivisionDTO>> OverrideAsync(EditSubdivisionDTO subdivision)
        {
            var result = await PostAsync<EditSubdivisionDTO>($"Subdivision/Override", subdivision).ConfigureAwait(false);
            if (result.IsSuccess && cache.ContainsKey(subdivision.ZipCode))
            {
                cache.Remove(subdivision.ZipCode);
            }
            return result;
        }

        public async Task<Result<SubdivisionDTO>> VerifyOptionAsync(int subdivisionOptionId)
        {
            var result = await GetAsync<SubdivisionDTO>($"Subdivision/Verify?subdivisionOptionId={subdivisionOptionId}").ConfigureAwait(false);
            if (result.IsSuccess && cache.ContainsKey(result.Data.ZipCode))
            {
                cache.Remove(result.Data.ZipCode);
            }

            return result;
        }

        public async Task<Result> CreateOptionAsync(SubdivisionOptionDTO subdivisionOption)
        {
            return await PostAsync<object>($"Subdivision/CreateOption", subdivisionOption).ConfigureAwait(false);
        }
    }
}
