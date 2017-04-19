using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using mehspot.iOS.Views;
using Mehspot.Core;
using Mehspot.Core.DTO.Badges;
using Mehspot.Core.Services;
using MehSpot.Core.DTO.Subdivision;
using UIKit;

namespace mehspot.iOS.Controllers.Badges.BadgeProfileDataSource
{
    public class EditBadgeTableSource : UITableViewSource
    {
        KeyValuePair<int?, string> [] states;

        private readonly List<UITableViewCell> cells;

        private readonly BadgeProfileDTO<EditBadgeProfileDTO> badge;
        private readonly SubdivisionService subdivisionService;

        private EditBadgeTableSource (BadgeProfileDTO<EditBadgeProfileDTO> profile, SubdivisionService subdivisionService)
        {
            this.badge = profile;
            this.subdivisionService = subdivisionService;
            cells = new List<UITableViewCell> ();
        }

        public static async Task<EditBadgeTableSource> Create (BadgeProfileDTO<EditBadgeProfileDTO> profile, SubdivisionService subdivisionService)
        {
            var result = new EditBadgeTableSource (profile, subdivisionService);
            await result.InitializeAsync ().ConfigureAwait (false);
            return result;
        }
        public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
        {
            var item = cells [indexPath.Row];
            return item;
        }

        public override nint RowsInSection (UITableView tableview, nint section)
        {
            return cells.Count;
        }


        public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
        {
            return cells [indexPath.Row].Frame.Height;
        }

        private async Task InitializeAsync ()
        {
            var subdivisionCellKey = "";
            this.states = await GetStates ();
            var profileSubdivisions = await GetSubdivisions (badge.Profile.Zip);
            cells.Add (PickerCell.Create (badge.Profile.State, (property) => { badge.Profile.State = property; }, "State", states));
            cells.Add (TextEditCell.Create (badge.Profile.City, a => badge.Profile.City = a, "City"));
            var zipCell = TextEditCell.Create (badge.Profile.Zip, a => badge.Profile.Zip = a, "Zip");
            zipCell.Mask = "#####";
            zipCell.ValueChanged += (arg1, arg2) => ZipCell_ValueChanged (arg1, arg2, subdivisionCellKey);
            cells.Add (zipCell);

            var subdivisionCell = SubdivisionPickerCell.Create (badge.Profile.SubdivisionId, (property) => {
                badge.Profile.SubdivisionId = property?.Id; }, "Subdivision", profileSubdivisions, badge.Profile.Zip, string.IsNullOrWhiteSpace (badge.Profile.Zip) || !zipCell.IsValid);
            subdivisionCell.FieldName = subdivisionCellKey;
            cells.Add (subdivisionCell);

            foreach (var badgeValue in badge.BadgeValues) {
                var badgeItem = badgeValue.Value.BadgeBadgeItem.BadgeItem;
                if (ProfileKeys.NonDuplicatedKeys.Contains (badgeItem.Name))
                    continue;

                var itemName = badgeItem.Name;
                var badgeSpecificName = string.Format ("{0}{1}", badge.BadgeName, itemName);
                var badgeSpecificValue = MehspotResources.ResourceManager.GetString (badgeSpecificName);
                var badgeItemDefaultKey = string.Format ("{0}{1}Default", badge.BadgeName, itemName);
                var placeholder = MehspotResources.ResourceManager.GetString (badgeItemDefaultKey) ?? badgeItem.DefaultValue;
                var label = badgeSpecificValue ?? MehspotResources.ResourceManager.GetString (itemName) ?? itemName;

                BadgeDataType valueType = BadgeDataType.String;
                Enum.TryParse (badgeItem.ValueType, out valueType);
                if (itemName.EndsWith ("Subdivision", StringComparison.InvariantCultureIgnoreCase) && itemName.StartsWith (badge.BadgeName, StringComparison.InvariantCultureIgnoreCase)) {
                    var zipFieldName = itemName.Replace ("Subdivision", "Zip");
                    var zipCode = badge.BadgeValues.FirstOrDefault (a => a.Value.BadgeBadgeItem.BadgeItem.Name == zipFieldName).Value?.Value;
                    var subdivisions = await GetSubdivisions (zipCode);
                    int value;
                    var cell = SubdivisionPickerCell.Create (int.TryParse(badgeValue.Value.Value, out value) ? value : (int?)null, property => badgeValue.Value.Value = property.Id.ToString(), label, subdivisions, zipCode);
                    cell.FieldName = itemName;
                    cells.Add (cell);
                } else if (itemName.EndsWith ("Zip", StringComparison.InvariantCultureIgnoreCase) && itemName.StartsWith (badge.BadgeName, StringComparison.InvariantCultureIgnoreCase)) {
                    var cell = TextEditCell.Create (badgeValue.Value.Value, a => badgeValue.Value.Value = a, label);
                    cell.Mask = "#####";
                    cell.ValueChanged += (arg1, arg2) => ZipCell_ValueChanged (arg1, arg2, itemName.Replace ("Zip", "Subdivision"));
                    cells.Add (cell);
                } else if (valueType == BadgeDataType.Boolean) {
                    bool value;
                    value = bool.TryParse (badgeValue.Value.Value, out value) && value;
                    cells.Add (BooleanEditCell.Create (value, a => badgeValue.Value.Value = a.ToString (), label));
                } else if (valueType == BadgeDataType.List) {
                    var listData = badgeItem.BadgeItemOptions.Select (a => new KeyValuePair<string, string> (a.Id.ToString (), MehspotResources.ResourceManager.GetString (a.Name) ?? a.Name)).ToArray ();
                    cells.Add (PickerCell.Create (badgeValue.Value.Value, (property) => { badgeValue.Value.Value = property; }, label, listData));
                } else if (valueType == BadgeDataType.CheckList) {
                    var listData = badgeItem.BadgeItemOptions.Select (a => new KeyValuePair<string, string> (a.Id.ToString (), MehspotResources.ResourceManager.GetString (a.Name) ?? a.Name)).ToArray ();
                    cells.Add (PickerCell.CreateMultiselect (badgeValue.Value.Values, (property) => { badgeValue.Value.Values = property?.ToArray (); }, label, listData));
                } else if (valueType == BadgeDataType.Integer) {
                    var cell = TextEditCell.Create (badgeValue.Value.Value, a => badgeValue.Value.Value = a, label, placeholder);
                    cell.ValidationRegex = "^[0-9]+$";
                    cell.TextInput.KeyboardType = UIKeyboardType.NumberPad;
                    cells.Add (cell);
                } else if (valueType == BadgeDataType.Float) {
                    var cell = TextEditCell.Create (badgeValue.Value.Value, a => badgeValue.Value.Value = a, label, placeholder);
                    cell.ValidationRegex = "^\\d+([\\.\\,]{0,1}\\d{0,2})?$";
                    cell.TextInput.KeyboardType = UIKeyboardType.DecimalPad;
                    cells.Add (cell);
                } else if (valueType == BadgeDataType.Date) {
                    cells.Add (PickerCell.CreateDatePicker (badgeValue.Value.Value, (property) => { badgeValue.Value.Value = property; }, label));
                } else if (valueType == BadgeDataType.LongString) {
                    cells.Add (MultilineTextEditCell.Create (badgeValue.Value.Value, (property) => badgeValue.Value.Value = property, label));
                } else
                    cells.Add (TextEditCell.Create (badgeValue.Value.Value, a => badgeValue.Value.Value = a, label, placeholder));
            }
        }

        private async void ZipCell_ValueChanged (TextEditCell sender, string value, string subdivisionCellKey)
        {
            var subdivisionCell = cells.OfType<SubdivisionPickerCell> ().FirstOrDefault (a => a.FieldName == subdivisionCellKey);
            if (subdivisionCell == null)
                return;

            subdivisionCell.IsReadOnly = true;
            if (sender.IsValid) {
                subdivisionCell.Subdivisions = await GetSubdivisions (value);
                subdivisionCell.ZipCode = value;
            }

            subdivisionCell.IsReadOnly = !sender.IsValid;
        }

        private async Task<List<SubdivisionDTO>> GetSubdivisions (string zipCode)
        {
            if (!string.IsNullOrWhiteSpace (zipCode)) {
                var subdivisionsResult = await subdivisionService.ListSubdivisionsAsync (zipCode);
                if (subdivisionsResult.IsSuccess) {
                    return subdivisionsResult.Data;
                }
            }

            return null;
        }

        private async Task<KeyValuePair<int?, string> []> GetStates ()
        {
            var statesResult = await subdivisionService.ListStatesAsync ();
            if (statesResult.IsSuccess) {
                return statesResult.Data.Select (a => new KeyValuePair<int?, string> (a.Id, a.Name)).ToArray ();
            }

            return null;
        }
    }
}
