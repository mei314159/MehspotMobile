using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mehspot.Core.DTO.Badges;
using Mehspot.Core.Services;

namespace Mehspot.Core.Filter.Search
{
    public delegate void CellChanged(object obj, string propertyName, object value);
    public abstract class CellsFactoryBase<TCell>
    {
        public event CellChanged CellChanged;

        protected CellsFactoryBase(BadgeService badgeService, int badgeId)
        {
            this.BadgeId = badgeId;
            this.BadgeService = badgeService;
        }

        public int BadgeId { get; private set; }
        public BadgeService BadgeService { get; private set; }

        protected async Task<KeyValuePair<int?, string>[]> GetOptionsAsync(string key, bool skipFirst = false)
        {
            var result = await BadgeService.GetBadgeKeysAsync(this.BadgeId, key);
            if (result.IsSuccess)
            {
                var data = skipFirst ? result.Data.Skip(1) : result.Data;
                return data.Select(a => new KeyValuePair<int?, string>(a.Id, a.Name)).ToArray();
            }

            return null;
        }

        public abstract Task<List<TCell>> CreateCellsForObject(object filter);

        public void OnCellChanged(object obj, string propertyName, object value)
        {
            CellChanged?.Invoke(obj, propertyName, value);
        }

        public abstract TCell CreateButtonCell(string title);

        public abstract TCell CreateRecommendationCell(BadgeUserRecommendationDTO item);
    }

    public abstract class CellsFactoryBase<TCell, TButtonCell, TRecommendationCell>
        : CellsFactoryBase<TCell>
        where TButtonCell : TCell, IButtonCell
        where TRecommendationCell : TCell
    {
        public CellsFactoryBase(BadgeService badgeService, int badgeId) : base(badgeService, badgeId)
        {
        }

        public override TCell CreateButtonCell(string title)
        {
            return CreateButtonCellTyped(title);
        }

        public override TCell CreateRecommendationCell(BadgeUserRecommendationDTO item)
        {
            return CreateRecommendationCellTyped(item);
        }

        public abstract TButtonCell CreateButtonCellTyped(string title);

        public abstract TRecommendationCell CreateRecommendationCellTyped(BadgeUserRecommendationDTO item);
    }

    public interface IButtonCell
    {
        event Action<object> OnButtonTouched;
    }
}
