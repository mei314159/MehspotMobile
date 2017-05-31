using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mehspot.Core.DTO.Search;
using Mehspot.Core.Filter.Search;
using Mehspot.Core.Services;
using Android.Views;
using Mehspot.AndroidApp;
using Android.Content;
using Android.App;

namespace Mehspot.iOS.Views.Cell
{
	public class CellFactory : CellsFactoryBase<View, ButtonCell, RecommendationCell>
	{
		readonly Activity activity;

		public CellFactory(Activity context, BadgeService badgeService, int badgeId) : base(badgeService, badgeId)
		{
			this.activity = context;
		}

		public override async Task<List<View>> CreateCellsForObject(object filter)
		{
			return (await CreateCellsInternal(filter)).OrderBy(a => a.Item1).Select(a => a.Item2).ToList();
		}

		private async Task<List<Tuple<int, View>>> CreateCellsInternal(object filter)
		{
			var type = filter.GetType();
			var properties = type.GetProperties()
								 .Select(prop => new { prop = prop, attr = prop.GetCustomAttributes(true).OfType<CellAttribute>().FirstOrDefault() })
								 .Where(a => a.attr != null)
								 .OrderBy(a => a.attr.Order);

			var cells = new List<Tuple<int, View>>();
			foreach (var prop in properties)
			{
				var value = prop.prop.GetValue(filter);
				View targetCell = null;
				switch (prop.attr.CellType)
				{
					case CellType.Range:
						targetCell = new SliderCell<int?>(activity, prop.attr.DefaultValue as int?, v =>
						{
							prop.prop.SetValue(filter, v);
							OnCellChanged(filter, prop.prop.Name, v);
						}, prop.attr.Label, prop.attr.MinValue, prop.attr.MaxValue, isReadOnly: prop.attr.ReadOnly);
						break;
					case CellType.Boolean:
						targetCell = new BooleanCell(activity, value as bool? == true, v =>
						{
							prop.prop.SetValue(filter, v == true ? v : (bool?)null);
							OnCellChanged(filter, prop.prop.Name, v);
						}, prop.attr.Label, isReadOnly: prop.attr.ReadOnly);
						break;
					case CellType.Select:
						var options = await this.GetOptionsAsync(prop.attr.OptionsKey, prop.attr.SkipFirstOption);
						targetCell = new PickerCell<int?>(activity, prop.attr.DefaultValue as int?, (v) =>
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
						targetCell = new MultiselectCell<int?>(activity, new int?[] { }, (v) =>
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
						targetCell = new TextViewCell(activity, value as string ?? string.Empty, prop.attr.Label);
						break;
					default:
						targetCell = new TextEditCell(activity, value as string, (v) => prop.prop.SetValue(filter, v), prop.attr.Label, mask: prop.attr.Mask, isReadOnly: prop.attr.ReadOnly);
						break;
				}
				if (targetCell != null)
				{
					cells.Add(new Tuple<int, View>(prop.attr.Order, targetCell));
				}
			}

			return cells.ToList();
		}

		public override ButtonCell CreateButtonCellTyped(string title)
		{
			var cell = new ButtonCell(activity, title);
			return cell;
		}

		public override RecommendationCell CreateRecommendationCellTyped(Core.DTO.Badges.BadgeUserRecommendationDTO item)
		{
			var cell = new RecommendationCell(activity, item);
			return cell;
		}
	}
}
