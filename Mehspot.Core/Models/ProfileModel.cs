using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Mehspot.Core.Contracts.ViewControllers;
using Mehspot.Core.DTO;
using Mehspot.Core.DTO.Subdivision;
using Mehspot.Core.Services;
using Mehspot.Core.Builders;
using Mehspot.Core.DTO.Badges;

namespace Mehspot.Core.Models
{
    public class ProfileModel<TView> : IListModel<TView>
    {
        public event Action LoadingStart;
        public event Action LoadingEnd;
        public event Action SignedOut;
        public volatile bool dataLoaded;
        public CellBuilder<TView> cellBuilder;
        //public List<TView> Cells = new List<TView>();
        public IList<TView> Cells { get; }

        private volatile bool loading;
        private ProfileDto profile;
        private ProfileService profileService;
        private SubdivisionService subdivisionService;
        private readonly IProfileViewController viewController;
        private KeyValuePair<string, string>[] genders = {
                new KeyValuePair<string, string>(null, "N/A"),
                new KeyValuePair<string, string>("M", "Male"),
                new KeyValuePair<string, string>("F", "Female")
        };

        public ProfileModel(ProfileService profileService, SubdivisionService subdivisionService, IProfileViewController viewController, CellBuilder<TView> cellBuilder)
        {
            Cells = new List<TView>();
            this.viewController = viewController;
            this.profileService = profileService;
            this.cellBuilder = cellBuilder;
            this.subdivisionService = subdivisionService;
        }

        public async Task RefreshView(bool isFirstLoad = false)
        {
            if (loading)
                return;
            loading = true;

            if (isFirstLoad)
                LoadingStart?.Invoke();

            var profileResult = await profileService.GetProfileAsync();

            if (profileResult.IsSuccess)
            {
                profile = profileResult.Data;
                var states = await GetStates();
                var subdivisions = await GetSubdivisions(profile.Zip);
                InitializeTable(profile, states, subdivisions);
            }
            else
            {
                viewController.ViewHelper.ShowAlert("Error", "Can not load profile data");
            }

            LoadingEnd?.Invoke();
            dataLoaded = profileResult.IsSuccess;
            loading = false;
        }

        void InitializeTable(ProfileDto profile, KeyValuePair<int?, string>[] states, List<SubdivisionDTO> subdivisions)
        {
            viewController.UserName = profile.UserName;
            viewController.FullName = $"{profile.FirstName} {profile.LastName}".Trim(' ');
            viewController.ProfilePicturePath = profile.ProfilePicturePath;

            Cells.Clear();
            Cells.Add((TView)cellBuilder.GetTextEditCell(profile.UserName, (c, a) => profile.UserName = a, "User Name"));
            Cells.Add((TView)cellBuilder.GetTextEditCell(profile.Email, (c, a) => profile.Email = a, "Email", null, true));
            var phoneNumberCell = (TView)cellBuilder.GetTextEditCell(profile.PhoneNumber, (c, a) => profile.PhoneNumber = a, "Phone Number", mask: "(###)###-####");
            Cells.Add(phoneNumberCell);
            Cells.Add((TView)cellBuilder.GetDatePickerCell(profile.DateOfBirth, (property) => { profile.DateOfBirth = property; }, "Date Of Birth"));
            Cells.Add((TView)cellBuilder.GetPickerCell(profile.Gender, (property) => { profile.Gender = property; }, "Gender", genders));
            Cells.Add((TView)cellBuilder.GetTextEditCell(profile.FirstName, (c, a) => profile.FirstName = a, "First Name"));
            Cells.Add((TView)cellBuilder.GetTextEditCell(profile.LastName, (c, a) => profile.LastName = a, "Last Name"));
            Cells.Add((TView)cellBuilder.GetTextEditCell(profile.AddressLine1, (c, a) => profile.AddressLine1 = a, "Address Line 1"));
            Cells.Add((TView)cellBuilder.GetTextEditCell(profile.AddressLine2, (c, a) => profile.AddressLine2 = a, "Address Line 2"));
            Cells.Add((TView)cellBuilder.GetPickerCell(profile.State, (property) => { profile.State = property; }, "State", states));
            Cells.Add((TView)cellBuilder.GetTextEditCell(profile.City, (c, a) => profile.City = a, "City"));
            var subdivisionCell = cellBuilder.GetSubdivisionPickerCell(profile.SubdivisionId, (property) =>
                        {
                            profile.SubdivisionId = property?.Id;
                            profile.SubdivisionOptionId = property?.OptionId;
                        }, "Subdivision", subdivisions, profile.Zip);
            var zipCell = cellBuilder.GetTextEditCell(profile.Zip, (c, a) =>
                        {
                            profile.Zip = a;
                            ZipCell_ValueChanged(c, a, subdivisionCell);
                        }, "Zip", mask: "#####");
            subdivisionCell.IsReadOnly = string.IsNullOrWhiteSpace(profile.Zip) || !zipCell.IsValid;
            Cells.Add((TView)zipCell);
            Cells.Add((TView)subdivisionCell);

            Cells.Add((TView)cellBuilder.GetBooleanCell(profile.MehspotNotificationsEnabled, v => profile.MehspotNotificationsEnabled = v, "Email notifications enabled"));
            Cells.Add((TView)cellBuilder.GetBooleanCell(profile.AllGroupsNotificationsEnabled, v => profile.AllGroupsNotificationsEnabled = v, "Group notifications enabled"));
            viewController.ReloadData();
        }

        public void Signout()
        {
            viewController.ViewHelper.ShowPrompt("Sign Out", "Are you sure you want to sign out?", "Yes, I do",
                () =>
                {
                    MehspotAppContext.Instance.AuthManager.SignOut();
                    MehspotAppContext.Instance.DisconnectSignalR();

                    SignedOut?.Invoke();
                });
        }

        public async Task SaveProfileAsync(byte[] pictureBytes = null)
        {
            viewController.SaveButtonEnabled = false;
            viewController.HideKeyboard();
            viewController.ViewHelper.ShowOverlay("Saving...");

            if (pictureBytes != null)
            {
                await this.profileService.UploadProfileImageAsync(pictureBytes);
            }

            var result = await this.profileService.UpdateAsync(profile);
            viewController.ViewHelper.HideOverlay();

            if (!result.IsSuccess)
            {
                var error = string.Join(Environment.NewLine, result.ModelState.ModelState.SelectMany(a => a.Value));
                viewController.ViewHelper.ShowAlert(result.ErrorMessage, error);
            }

            this.viewController.SaveButtonEnabled = true;
        }

        private async void ZipCell_ValueChanged(ITextEditCell sender, string value, ISubdivisionPickerCell subdivisionCell)
        {
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

        private async Task<KeyValuePair<int?, string>[]> GetStates()
        {
            var statesResult = await subdivisionService.ListStatesAsync();

            if (statesResult.IsSuccess)
            {
                return statesResult.Data.Select(a => new KeyValuePair<int?, string>(a.Id, a.Name)).ToArray();
            }

            return null;
        }
    }
}
