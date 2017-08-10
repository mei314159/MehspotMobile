using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mehspot.Core.Contracts.ViewControllers;
using Mehspot.Core.DTO;
using Mehspot.Core.Services;

namespace Mehspot.Core.Models
{
    public class UserProfileViewModel
    {
        private readonly BadgeService badgesService;
        private readonly ProfileService profileService;
        private readonly IUserProfileViewController viewController;
        private volatile bool loading;
        private volatile int selectedBadgeIndex;

        public UserProfileSummaryDTO userProfile;
        public List<BadgeSummaryBaseDTO> Items;
        public string UserId { get; set; }
        public volatile bool dataLoaded;

        public BadgeSummaryBaseDTO SelectedBadge => Items?[selectedBadgeIndex];


        public UserProfileViewModel(BadgeService badgesService, ProfileService profileService, IUserProfileViewController viewController)
        {
            this.badgesService = badgesService;
            this.profileService = profileService;
            this.viewController = viewController;
        }

        public async Task LoadAsync()
        {
            if (loading)
                return;

            loading = true;

            var profileResult = await profileService.LoadProfileSummaryAsync(UserId);
            if (profileResult.IsSuccess)
            {
                this.Items = profileResult.Data.RegisteredBadges;
                if (Items.Count == 0)
                {
                    viewController.ShowLabel();
                }
                else
                {
                    this.userProfile = profileResult.Data;
                    await InitializeDataAsync();
                }
            }
            else
            {
                viewController.ViewHelper.ShowAlert("Error", "Can not load data");
            }

            dataLoaded = profileResult.IsSuccess;
            viewController.ViewHelper.HideOverlay();
            loading = false;
        }

        private async Task InitializeDataAsync()
        {
            viewController.UserName = userProfile.UserName;
            viewController.FullName = $"{userProfile.FirstName} {userProfile.LastName}".Trim(' ');
            if (string.IsNullOrWhiteSpace(viewController.FullName))
            {
                viewController.FullName = userProfile.UserName;
            }
            viewController.ProfilePicturePath = userProfile.ProfilePicturePath;
            viewController.RecommendationsCount = userProfile.RecommendationsCount;
            viewController.ReferencesCount = userProfile.ReferencesCount;

            viewController.ReloadData();
        }

        public void SelectRow(int row)
        {
            this.selectedBadgeIndex = row;
        }

        public void SelectBadge(BadgeSummaryBaseDTO dto)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i] == dto)
                {
                    SelectRow(i);
                    break;
                }
            }
        }
    }
}
