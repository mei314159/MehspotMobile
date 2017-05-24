using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mehspot.Core.Services;

namespace Mehspot.Core.Filter.Search
{
    public delegate void CellChanged(object obj, string propertyName, object value);
    public abstract class CellsFactoryBase<TCell>
    {
        protected readonly BadgeService badgeService;
        protected readonly int badgeId;
        public event CellChanged CellChanged;

        public CellsFactoryBase(BadgeService badgeService, int badgeId)
        {
            this.badgeId = badgeId;
            this.badgeService = badgeService;
        }

        protected async Task<KeyValuePair<int?, string>[]> GetOptionsAsync(string key, bool skipFirst = false)
        {
            var result = await badgeService.GetBadgeKeysAsync(this.badgeId, key);
            if (result.IsSuccess)
            {
                var data = skipFirst ? result.Data.Skip(1) : result.Data;
                return data.Select(a => new KeyValuePair<int?, string>(a.Id, a.Name)).ToArray();
            }

            return null;
        }

        public abstract Task<List<TCell>> CreateCells(object filter);

        public void OnCellChanged(object obj, string propertyName, object value)
        {
            CellChanged?.Invoke(obj, propertyName, value);
        }
    }
}
