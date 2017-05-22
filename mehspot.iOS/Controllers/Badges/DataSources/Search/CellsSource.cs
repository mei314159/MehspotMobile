using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using mehspot.iOS.Views;
using mehspot.iOS.Views.Cell;
using Mehspot.Core.DTO.Search;
using Mehspot.Core.Filter.Search;
using Mehspot.Core.Services;
using UIKit;

namespace mehspot.iOS.Controllers.Badges.DataSources.Search
{
	public class CellsSource : FilterTableSource
	{
		public CellsSource(BadgeService badgeService, int badgeId) : base(badgeService, badgeId)
		{
		}

		public async Task<List<UITableViewCell>> CreateCells(object filter)
		{
			return (await CreateCellsInternal(filter)).OrderBy(a => a.Item1).Select(a => a.Item2).ToList();
		}

		private async Task<List<Tuple<int, UITableViewCell>>> CreateCellsInternal(object filter)
		{
			var type = filter.GetType();
			var properties = type.GetProperties()
								 .Select(prop => new { prop = prop, attr = prop.GetCustomAttributes(true).OfType<SearchPropertyAttribute>().FirstOrDefault() })
								 .Where(a => a.attr != null)
								 .OrderBy(a => a.attr.Order);

			var cells = new List<Tuple<int, UITableViewCell>>();
			foreach (var prop in properties)
			{
				var value = prop.prop.GetValue(filter);
				UITableViewCell targetCell = null;
				switch (prop.attr.CellType)
				{
					case CellType.Range:
						targetCell = SliderCell.Create<int?>(prop.attr.DefaultValue as int?, v => prop.prop.SetValue(filter, v), prop.attr.Label, prop.attr.MinValue, prop.attr.MaxValue);
						break;
					case CellType.Boolean:
						targetCell = BooleanEditCell.Create(value as bool? == true, v => prop.prop.SetValue(filter, v == true ? v : (bool?)null), prop.attr.Label);
						break;
					case CellType.Select:
						var options = await this.GetOptionsAsync(prop.attr.OptionsKey);
						targetCell = PickerCell.Create(prop.attr.DefaultValue as int?, (v) => prop.prop.SetValue(filter, v), prop.attr.Label, options);
						break;
					case CellType.Multiselect:
						var multiselectOptions = await this.GetOptionsAsync(prop.attr.OptionsKey);
						targetCell = PickerCell.CreateMultiselect(new int?[] { }, (v) => prop.prop.SetValue(filter, v?.Select(a => a.ToString()).ToArray()), prop.attr.Label, multiselectOptions);
						break;
					case CellType.Complex:
						var childCells = await this.CreateCellsInternal(value);
						cells.AddRange(childCells);
						break;
					default:
						targetCell = TextEditCell.Create(value as string, (v) => prop.prop.SetValue(filter, v), prop.attr.Label, mask: prop.attr.Mask);
						break;
				}
				if (targetCell != null)
				{
					cells.Add(new Tuple<int, UITableViewCell>(prop.attr.Order, targetCell));
				}
			}

			return cells.ToList();
		}
	}
}
