using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Mehspot.iOS.Views;
using Mehspot.iOS.Views.Cell;
using Mehspot.Core.DTO.Search;
using Mehspot.Core.Filter.Search;
using Mehspot.Core.Services;
using UIKit;

namespace Mehspot.iOS.Controllers.Badges.DataSources.Search
{
	public delegate void CellChanged(object obj, string propertyName, object value);

	public class CellsSource : FilterTableSource
	{
		public event CellChanged CellChanged;

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
								 .Select(prop => new { prop = prop, attr = prop.GetCustomAttributes(true).OfType<CellAttribute>().FirstOrDefault() })
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
						targetCell = SliderCell.Create<int?>(prop.attr.DefaultValue as int?, v =>
						{
							prop.prop.SetValue(filter, v);
							CellChanged?.Invoke(filter, prop.prop.Name, v);
						}, prop.attr.Label, prop.attr.MinValue, prop.attr.MaxValue, isReadOnly: prop.attr.ReadOnly);
						break;
					case CellType.Boolean:
						targetCell = BooleanEditCell.Create(value as bool? == true, v =>
						{
							prop.prop.SetValue(filter, v == true ? v : (bool?)null);
							CellChanged?.Invoke(filter, prop.prop.Name, v);
						}, prop.attr.Label, isReadOnly: prop.attr.ReadOnly);
						break;
					case CellType.Select:
						var options = await this.GetOptionsAsync(prop.attr.OptionsKey, prop.attr.SkipFirstOption);
						targetCell = PickerCell.Create(prop.attr.DefaultValue as int?, (v) =>
						{
							prop.prop.SetValue(filter, v);
							CellChanged?.Invoke(filter, prop.prop.Name, v);
						}, prop.attr.Label, options, isReadOnly: prop.attr.ReadOnly);
						break;
					case CellType.Multiselect:
						var multiselectOptions = await this.GetOptionsAsync(prop.attr.OptionsKey, prop.attr.SkipFirstOption);
						targetCell = PickerCell.CreateMultiselect(new int?[] { }, (v) =>
						{
							prop.prop.SetValue(filter, v?.Select(a => a.ToString()).ToArray());
							CellChanged?.Invoke(filter, prop.prop.Name, v);
						}, prop.attr.Label, multiselectOptions, isReadOnly: prop.attr.ReadOnly);
						break;
					case CellType.Complex:
						var childCells = await this.CreateCellsInternal(value);
						cells.AddRange(childCells);
						break;
					case CellType.TextView:
						targetCell = TextViewCell.Create(value as string ?? string.Empty, prop.attr.Label);
						break;
					default:
						targetCell = TextEditCell.Create(value as string, (v) => prop.prop.SetValue(filter, v), prop.attr.Label, mask: prop.attr.Mask, isReadOnly: prop.attr.ReadOnly);
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
