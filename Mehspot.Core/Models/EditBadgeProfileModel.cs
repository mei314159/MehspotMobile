using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mehspot.Core.Builders;
using Mehspot.Core.Contracts.ViewControllers;
using Mehspot.Core.DTO.Badges;
using Mehspot.Core.DTO.Subdivision;
using Mehspot.Core.Models;
using Mehspot.Core.Services;
using Mehspot.Core.Extensions;
namespace Mehspot.Core
{
    public class EditBadgeProfileModel<TCell> : IListModel<TCell>
    {
        private volatile bool loading;
        private volatile bool dataLoaded;
        private BadgeProfileDTO<EditBadgeProfileDTO> profile;
        private IBooleanEditCell toggleEnabledStateCell;
        private IButtonCell deleteBadgeCell;

        private readonly IEditBadgeProfileController controller;
        private SubdivisionService subdivisionService;

        private readonly CellBuilder<TCell> cellBuilder;

        public IList<TCell> Cells { get; }
        public event Action LoadingStarted;
        public event Action LoadingEnded;

        readonly BadgeService badgeService;
        public EditBadgeProfileModel(IEditBadgeProfileController controller, BadgeService badgeService, SubdivisionService subdivisionService, CellBuilder<TCell> cellBuilder)
        {
            Cells = new List<TCell>();
            this.badgeService = badgeService;
            this.controller = controller;
            this.cellBuilder = cellBuilder;
            this.subdivisionService = subdivisionService;
            this.controller.WindowTitle = GetTitle();
        }

        public async Task LoadAsync()
        {
            if (!dataLoaded)
            {
                await ReloadAsync();
            }
        }

        public async Task ReloadAsync()
        {
            if (loading)
                return;
            loading = true;

            LoadingStarted?.Invoke();
            var profileResult = await badgeService.GetMyBadgeProfileAsync(this.controller.BadgeId);
            dataLoaded = profileResult.IsSuccess;
            if (profileResult.IsSuccess)
            {
                this.profile = profileResult.Data;
                await this.LoadCellsAsync();
                controller.ReloadData();
            }
            else if (!profileResult.IsNetworkIssue)
            {
                controller.ViewHelper.ShowAlert("Error", "Can not load profile data");
            }

            LoadingEnded?.Invoke();
            loading = false;
        }

        public async Task SaveAsync()
        {
            controller.SaveButtonEnabled = false;
            controller.HideKeyboard();
            controller.ViewHelper.ShowOverlay("Saving...");

            var result = await this.badgeService.SaveBadgeProfileAsync(profile);
            controller.ViewHelper.HideOverlay();
            string message;
            if (result.IsSuccess)
            {
                if (!controller.BadgeIsRegistered)
                {
                    this.controller.BadgeIsRegistered = this.profile.IsEnabled = true;
                    ShowToggleEnabledCell();
                    controller.ReloadData();
                }

                this.controller.WindowTitle = GetTitle();
                message = $"{controller.BadgeName} badge profile successfully saved";

                controller.ViewHelper.ShowAlert("Success", message, () =>
                {
                    if (controller.RedirectToSearchResults)
                    {
                        controller.GoToSearchResults();
                    }
                });
            }
            else if (!result.IsNetworkIssue)
            {
                var error = result.ModelState?.ModelState?.Select(a => a.Value?.FirstOrDefault())?.FirstOrDefault();
                message = error != null ? error : result.ErrorMessage;
                var title = error != null ? MehspotResources.ValidationError : result.ErrorMessage;
                controller.ViewHelper.ShowAlert(title, message);
            }

            controller.SaveButtonEnabled = true;
        }

        private string GetTitle()
        {
            var badgeName = MehspotResources.ResourceManager.GetString(this.controller.BadgeName) ?? this.controller.BadgeName;
            var title =
                this.controller.BadgeIsRegistered ?
                "Update " + (this.controller.BadgeName == Constants.BadgeNames.BabysitterEmployer ? "babysitting (employer) page" : badgeName)
                : "Sign up " +
                (this.controller.BadgeName == Constants.BadgeNames.Fitness || this.controller.BadgeName == Constants.BadgeNames.Golf || this.controller.BadgeName == Constants.BadgeNames.OtherJobs ? "for " : "as ") + badgeName;

            return title;
        }

        async void BadgeStateChanged(bool isEnabled)
        {
            await badgeService.ToggleEnabledState(this.controller.BadgeId, isEnabled);
        }

        void TableSource_OnDeleteBadgeButtonTouched()
        {
            controller.ViewHelper.ShowPrompt("Delete Badge",
                                 "Do you want to delete this badge registration?",
                                 "Yes, I do", async () =>
                                 {
                                     var result = await badgeService.DeleteBadgeAsync(this.controller.BadgeId);
                                     if (result.IsSuccess)
                                     {
                                         controller.Dismiss();
                                     }
                                     else if (!result.IsNetworkIssue)
                                     {
                                         controller.ViewHelper.ShowAlert("Error", result.ErrorMessage);
                                     }
                                 });
        }

        private void ShowToggleEnabledCell()
        {
            toggleEnabledStateCell = cellBuilder.GetBooleanCell(profile.IsEnabled, v => { profile.IsEnabled = v; IsEnabledCell_ValueChanged(v); }, "Enable Badge");
            deleteBadgeCell = cellBuilder.GetButtonCell("Delete Badge");
            deleteBadgeCell.OnButtonTouched += DeleteBadgeCell_OnButtonTouched;
            Cells.Add((TCell)toggleEnabledStateCell);
            Cells.Add((TCell)deleteBadgeCell);
        }

        private async Task LoadCellsAsync()
        {
            Cells.Clear();

            foreach (var badgeValue in profile.BadgeValues)
            {
                var badgeItem = badgeValue.Value.BadgeBadgeItem.BadgeItem;
				BadgeDataType valueType = BadgeDataType.String;
				Enum.TryParse(badgeItem.ValueType, out valueType);

                ExcludingKey excludingKey = ProfileKeys.ExcludedKeys.FirstOrDefault(a => a.Name == badgeItem.Name);
                if (excludingKey != null)
                {
                    if (excludingKey.UseDefault){
                        switch (valueType)
                        {
                            case BadgeDataType.Boolean:
                                badgeValue.Value.Value = bool.FalseString;
                                break;
                            case BadgeDataType.Integer:
                                badgeValue.Value.Value = default(int).ToString();
                                break;
							case BadgeDataType.Float:
                                badgeValue.Value.Value = default(float).ToString();
								break;
                            default:
                                break;
                        }
                    }

                    continue;
                }

                var itemName = badgeItem.Name;
                var badgeSpecificName = string.Format("{0}{1}", profile.BadgeName, itemName);
                var badgeSpecificValue = MehspotResources.ResourceManager.GetString(badgeSpecificName);
                var badgeItemDefaultKey = string.Format("{0}{1}Default", profile.BadgeName, itemName);
                var placeholder = MehspotResources.ResourceManager.GetString(badgeItemDefaultKey) ?? badgeItem.DefaultValue;
                var label = badgeSpecificValue ?? MehspotResources.ResourceManager.GetString(itemName) ?? itemName;

                if (itemName.EndsWith("Subdivision", StringComparison.OrdinalIgnoreCase) && itemName.StartsWith(profile.BadgeName, StringComparison.OrdinalIgnoreCase))
                {
                    var zipFieldName = itemName.Replace("Subdivision", "Zip");
                    var zipCode = profile.BadgeValues.FirstOrDefault(a => a.Value.BadgeBadgeItem.BadgeItem.Name == zipFieldName).Value?.Value;
                    var subdivisions = await GetSubdivisions(zipCode);
                    int value;
                    var cell = cellBuilder.GetSubdivisionPickerCell(int.TryParse(badgeValue.Value.Value, out value) ? value : (int?)null, null, property => badgeValue.Value.Value = property?.Id.ToString(), label, subdivisions, zipCode);
                    cell.FieldName = itemName;
                    Cells.Add((TCell)cell);
                }
                else if (itemName.EndsWith("Zip", StringComparison.OrdinalIgnoreCase) && itemName.StartsWith(profile.BadgeName, StringComparison.OrdinalIgnoreCase))
                {
                    var cell = cellBuilder.GetTextEditCell(badgeValue.Value.Value, (c, a) => { badgeValue.Value.Value = a; ZipCell_ValueChanged(c, a, itemName.Replace("Zip", "Subdivision")); }, label, KeyboardType.Numeric, mask: "#####");
                    Cells.Add((TCell)cell);
                }
                else if (valueType == BadgeDataType.Boolean)
                {
                    bool value;
                    if (!bool.TryParse(badgeValue.Value.Value, out value))
                    {
                        badgeValue.Value.Value = bool.FalseString;
                    }

                    var cell = (TCell)cellBuilder.GetBooleanCell(value, a => badgeValue.Value.Value = a.ToString(), label);
                    Cells.Add(cell);
                }
                else if (valueType == BadgeDataType.List)
                {
                    var listData = badgeItem.BadgeItemOptions.Select(a => new KeyValuePair<string, string>(a.Id.ToString(), MehspotResources.ResourceManager.GetString(a.Name) ?? a.Name)).ToArray();
                    Cells.Add(cellBuilder.GetPickerCell(badgeValue.Value.Value, (property) => { badgeValue.Value.Value = property; }, label, listData));
                }
                else if (valueType == BadgeDataType.CheckList)
                {
                    var listData = badgeItem.BadgeItemOptions.Select(a => new KeyValuePair<string, string>(a.Id.ToString(), MehspotResources.ResourceManager.GetString(a.Name) ?? a.Name)).ToArray();
                    Cells.Add(cellBuilder.GetMultiselectCell(badgeValue.Value.Values, (property) => { badgeValue.Value.Values = property?.ToArray(); }, label, listData));
                }
                else if (valueType == BadgeDataType.Integer)
                {
                    var cell = cellBuilder.GetTextEditCell(badgeValue.Value.Value, (c, a) => badgeValue.Value.Value = a, label, KeyboardType.Numeric, placeholder, validationRegex: "^[0-9]+$");
                    Cells.Add((TCell)cell);
                }
                else if (valueType == BadgeDataType.Float)
                {
                    var cell = cellBuilder.GetTextEditCell(badgeValue.Value.Value, (c, a) => badgeValue.Value.Value = a, label, KeyboardType.Decimal, placeholder, validationRegex: "^\\d+([\\.\\,]{0,1}\\d{0,2})?$");
                    Cells.Add((TCell)cell);
                }
                else if (valueType == BadgeDataType.Date)
                {
                    Cells.Add(cellBuilder.GetDatePickerCell(badgeValue.Value.Value, (property) => { badgeValue.Value.Value = property; }, label));
                }
                else if (valueType == BadgeDataType.LongString)
                {
                    var cell = cellBuilder.GetTextEditCell(badgeValue.Value.Value, (a, property) => badgeValue.Value.Value = property, label);
                    cell.Multiline = true;
                    Cells.Add((TCell)cell);
                }
                else
                {
                    var cell = (TCell)cellBuilder.GetTextEditCell(badgeValue.Value.Value, (c, a) => badgeValue.Value.Value = a, label, KeyboardType.Default, placeholder);
                    Cells.Add(cell);
                }
            }

            if (profile.Id != default(int))
            {
                ShowToggleEnabledCell();
            }
        }

        private async void ZipCell_ValueChanged(ITextEditCell sender, string value, string subdivisionCellKey)
        {
            var subdivisionCell = Cells.OfType<ISubdivisionPickerCell>().FirstOrDefault(a => a.FieldName == subdivisionCellKey);
            if (subdivisionCell == null)
                return;

            subdivisionCell.SetProperty(null);
            subdivisionCell.IsReadOnly = true;
            if (sender.IsValid)
            {
                subdivisionCell.Subdivisions = await GetSubdivisions(value);
                subdivisionCell.ZipCode = value;
            }

            subdivisionCell.IsReadOnly = !sender.IsValid;
        }

        private async Task<List<SubdivisionDTO>> GetSubdivisions(string zipCode)
        {
            if (!string.IsNullOrWhiteSpace(zipCode))
            {
                var subdivisionsResult = await subdivisionService.ListSubdivisionsAsync(zipCode);
                if (subdivisionsResult.IsSuccess)
                {
                    return subdivisionsResult.Data;
                }
            }

            return new List<SubdivisionDTO>();
        }

        private void IsEnabledCell_ValueChanged(bool value)
        {
            this.BadgeStateChanged(value);
        }

        private void DeleteBadgeCell_OnButtonTouched(object sender)
        {
            this.TableSource_OnDeleteBadgeButtonTouched();
        }
    }
}

