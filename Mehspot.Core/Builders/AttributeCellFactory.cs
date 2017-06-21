using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Mehspot.Core.DTO.Badges;
using Mehspot.Core.DTO.Search;
using Mehspot.Core.Extensions;
using Mehspot.Core.Services;
namespace Mehspot.Core.Builders
{
    public delegate void CellChanged(object obj, string propertyName, object value);

    public class AttributeCellFactory<TCell>
    {
        public event CellChanged CellChanged;
        private readonly CellBuilder<TCell> cellBuilder;

        public BadgeService BadgeService { get; set; }
        public int BadgeId { get; set; }

        public AttributeCellFactory(BadgeService badgeService, int badgeId, CellBuilder<TCell> cellBuilder)
        {
            this.BadgeId = badgeId;
            this.BadgeService = badgeService;
            this.cellBuilder = cellBuilder;
        }

        public virtual async Task<List<TCell>> CreateCellsForObject(object filter)
        {
            return (await CreateCellsInternal(filter)).OrderBy(a => a.Item1).Select(a => a.Item2).ToList();
        }

        public virtual IButtonCell CreateButtonCell(string title)
        {
            var cell = cellBuilder.GetButtonCell(title);
            return cell;
        }

        public virtual TCell CreateRecommendationCell(BadgeUserRecommendationDTO item)
        {
            var cell = cellBuilder.GetRecommendationCell(item);
            return cell;
        }

        protected virtual async Task<KeyValuePair<int?, string>[]> GetOptionsAsync(string key, bool skipFirst = false)
        {
            var result = await BadgeService.GetBadgeKeysAsync(this.BadgeId, key);
            if (result.IsSuccess)
            {
                var data = skipFirst ? result.Data.Skip(1) : result.Data;
                return data.Select(a => new KeyValuePair<int?, string>(a.Id, a.Name)).ToArray();
            }

            return null;
        }

        private async Task<List<Tuple<int, TCell>>> CreateCellsInternal(object filter)
        {
            var type = filter.GetType();
            var properties = type.GetTypeInfo().GetAllProperties()
                                 .Select(prop => new { prop = prop, attr = prop.GetCustomAttributes(true).OfType<CellAttribute>().FirstOrDefault() })
                                 .Where(a => a.attr != null)
                                 .OrderBy(a => a.attr.Order);

            var cells = new List<Tuple<int, TCell>>();
            foreach (var prop in properties)
            {
                var value = prop.prop.GetValue(filter);
                TCell targetCell = default(TCell);
                switch (prop.attr.CellType)
                {
                    case CellType.Range:
                        targetCell = cellBuilder.GetRangeCell<int?>(prop.attr.DefaultValue as int?, v =>
                        {
                            prop.prop.SetValue(filter, v);                      // TODO Object of type 'System.Int32' cannot be converted to type 'System.Nullable`1[System.Double]'.
                            OnCellChanged(filter, prop.prop.Name, v);
                        }, prop.attr.Label, prop.attr.MinValue, prop.attr.MaxValue, isReadOnly: prop.attr.ReadOnly);
                        break;
                    case CellType.Boolean:
                        targetCell = (TCell)cellBuilder.GetBooleanCell(value as bool? == true, v =>
                        {
                            prop.prop.SetValue(filter, v == true ? v : (bool?)null);
                            OnCellChanged(filter, prop.prop.Name, v);
                        }, prop.attr.Label, isReadOnly: prop.attr.ReadOnly);
                        break;
                    case CellType.Select:
                        var options = await this.GetOptionsAsync(prop.attr.OptionsKey, prop.attr.SkipFirstOption);
                        targetCell = cellBuilder.GetPickerCell(prop.attr.DefaultValue as int?, (v) =>
                        {
                            object val = null;
                            if (v != null)
                            {
                                var propertyType = Nullable.GetUnderlyingType(prop.prop.PropertyType) ?? prop.prop.PropertyType;
                                val = Convert.ChangeType(v, propertyType);
                            }

                            prop.prop.SetValue(filter, val);
                            OnCellChanged(filter, prop.prop.Name, v);
                        }, prop.attr.Label, options, isReadOnly: prop.attr.ReadOnly);
                        break;
                    case CellType.Multiselect:
                        var multiselectOptions = await this.GetOptionsAsync(prop.attr.OptionsKey, prop.attr.SkipFirstOption);
                        targetCell = cellBuilder.GetMultiselectCell<int?>(new int?[] { }, (v) =>
                        {
                            prop.prop.SetValue(filter, v?.Select(a => a.ToString()).ToArray());
                            OnCellChanged(filter, prop.prop.Name, v);
                        }, prop.attr.Label, multiselectOptions, isReadOnly: prop.attr.ReadOnly);
                        break;
                    case CellType.Complex:
                        var childCells = await this.CreateCellsInternal(value);
                        cells.AddRange(childCells);
                        break;
                    case CellType.TextView:
                        targetCell = cellBuilder.GetTextViewCell(value as string ?? string.Empty, prop.attr.Label);
                        break;
                    default:
                        targetCell = (TCell)cellBuilder.GetTextEditCell(value as string, (cell, v) => prop.prop.SetValue(filter, v), prop.attr.Label, mask: prop.attr.Mask, isReadOnly: prop.attr.ReadOnly);
                        break;
                }
                if (targetCell != null)
                {
                    cells.Add(new Tuple<int, TCell>(prop.attr.Order, targetCell));
                }
            }

            return cells.ToList();
        }

        private void OnCellChanged(object obj, string propertyName, object value)
        {
            CellChanged?.Invoke(obj, propertyName, value);
        }
    }
}
