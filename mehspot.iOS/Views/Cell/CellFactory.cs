using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mehspot.Core.DTO.Badges;
using Mehspot.Core.DTO.Search;
using Mehspot.Core.Filter.Search;
using Mehspot.Core.Services;
using UIKit;

namespace Mehspot.iOS.Views.Cell
{
	public class CellFactory : CellsFactoryBase<UITableViewCell, ButtonCell, RecommendationCell>
	{

		public CellFactory(BadgeService badgeService, int badgeId) : base(badgeService, badgeId)
		{
		}

		public override ButtonCell CreateButtonCellTyped(string title)
		{
			return ButtonCell.Create(title);
		}

		public override async Task<List<UITableViewCell>> CreateCellsForObject(object filter)
		{
			return (await CreateCellsInternal(filter)).OrderBy(a => a.Item1).Select(a => a.Item2).ToList();
		}

		public override RecommendationCell CreateRecommendationCellTyped(BadgeUserRecommendationDTO item)
		{
			return RecommendationCell.Create(item);
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
							OnCellChanged(filter, prop.prop.Name, v);
						}, prop.attr.Label, prop.attr.MinValue, prop.attr.MaxValue, isReadOnly: prop.attr.ReadOnly);
						break;
					case CellType.Boolean:
						targetCell = BooleanEditCell.Create(value as bool? == true, v =>
						{
							prop.prop.SetValue(filter, v == true ? v : (bool?)null);
							OnCellChanged(filter, prop.prop.Name, v);
						}, prop.attr.Label, isReadOnly: prop.attr.ReadOnly);
						break;
					case CellType.Select:
						var options = await this.GetOptionsAsync(prop.attr.OptionsKey, prop.attr.SkipFirstOption);
						targetCell = PickerCell.Create(prop.attr.DefaultValue as int?, (v) =>
						{
							prop.prop.SetValue(filter, v);
							OnCellChanged(filter, prop.prop.Name, v);
						}, prop.attr.Label, options, isReadOnly: prop.attr.ReadOnly);
						break;
					case CellType.Multiselect:
						var multiselectOptions = await this.GetOptionsAsync(prop.attr.OptionsKey, prop.attr.SkipFirstOption);
						targetCell = PickerCell.CreateMultiselect(new int?[] { }, (v) =>
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
