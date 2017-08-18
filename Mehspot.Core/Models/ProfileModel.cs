using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Mehspot.Core.DTO;
using Mehspot.Core.DTO.Subdivision;
using Mehspot.Core.Services;
using Mehspot.Core.Builders;
using System.IO;

namespace Mehspot.Core.Models
{
    public class ProfileModel<TView> : IListModel<TView>
    {
        private volatile bool loading;

        private readonly IProfileViewController viewController;

        private CellBuilder<TView> cellBuilder;
        private ProfileService profileService;
        private SubdivisionService subdivisionService;
        private KeyValuePair<string, string>[] genders = {
                new KeyValuePair<string, string>(null, "N/A"),
                new KeyValuePair<string, string>("M", "Male"),
                new KeyValuePair<string, string>("F", "Female")
        };
		
        public volatile bool dataLoaded;

		public IList<TView> Cells { get; }
		public ProfileDto Profile { get; private set; }

		public event Action LoadingStart;
		public event Action<Result<ProfileDto>> LoadingEnd;
		public event Action SignedOut;

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
            var profileResult = await profileService.LoadProfileAsync().ConfigureAwait(false);

            LoadingEnd?.Invoke(profileResult);
            dataLoaded = profileResult.IsSuccess;
            loading = false;
        }

        public async Task InitializeTableAsync(ProfileDto profile)
        {
            this.Profile = profile;
            var subdivisions = await GetSubdivisions(profile.Zip).ConfigureAwait(false);
            viewController.InvokeOnMainThread(() =>
            {
                viewController.UserName = profile.UserName;
                viewController.FullName = $"{profile.FirstName} {profile.LastName}".Trim(' ');
                viewController.ProfilePicturePath = profile.ProfilePicturePath;

                Cells.Clear();
                Cells.Add((TView)cellBuilder.GetTextEditCell(profile.UserName, (c, a) => profile.UserName = a, "User Name"));
                Cells.Add((TView)cellBuilder.GetTextEditCell(profile.Email, (c, a) => profile.Email = a, "Email", KeyboardType.Email, null, true));
                var subdivisionCell = cellBuilder.GetSubdivisionPickerCell(profile.SubdivisionId, (property) =>
                            {
                                profile.SubdivisionId = property?.Id;
                                profile.SubdivisionOptionId = property?.OptionId;
                            }, "Subdivision", subdivisions, profile.Zip);
                var zipCell = cellBuilder.GetTextEditCell(profile.Zip, (c, a) =>
                            {
                                profile.Zip = a;
                                ZipCell_ValueChanged(c, a, subdivisionCell);
                            }, "Zip", KeyboardType.Numeric, mask: "#####");
                subdivisionCell.IsReadOnly = string.IsNullOrWhiteSpace(profile.Zip) || !zipCell.IsValid;
                Cells.Add((TView)zipCell);
                Cells.Add((TView)subdivisionCell);
                viewController.ReloadData();
            });
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

        public async Task SaveProfileAsync(Stream pictureStream = null)
        {
            viewController.SaveButtonEnabled = false;
            viewController.HideKeyboard();
            viewController.ViewHelper.ShowOverlay("Saving...");

            if (pictureStream != null)
            {
                await this.profileService.UploadProfileImageAsync(pictureStream).ConfigureAwait(false);
            }

            var result = await this.profileService.UpdateAsync(Profile).ConfigureAwait(false);
            viewController.ViewHelper.HideOverlay();
            viewController.InvokeOnMainThread(() =>
            {
                if (result.IsSuccess)
                {
                    viewController.ViewHelper.ShowAlert("Success", "Profile successfully saved");
                }
                else
                {
                    var error = result.ModelState?.ModelState?.SelectMany(a => a.Value)?.FirstOrDefault();
                    var message = error != null ? error : result.ErrorMessage;
                    var title = error != null ? MehspotResources.ValidationError : result.ErrorMessage;
                    viewController.ViewHelper.ShowAlert(title, message);
                }

                this.viewController.SaveButtonEnabled = true;
            });
        }

        private async void ZipCell_ValueChanged(ITextEditCell sender, string value, ISubdivisionPickerCell subdivisionCell)
        {
            subdivisionCell.IsReadOnly = true;

            subdivisionCell.SetProperty(null);
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
    }
}
