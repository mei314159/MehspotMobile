using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Mehspot.Core.Builders;
using Mehspot.Core.DTO;
using Mehspot.Core.DTO.Badges;
using Mehspot.Core.DTO.Search;
using Mehspot.Core.Services;

namespace Mehspot.Core.Models
{
    public delegate void GoToMessagingHandler(string userId, string userName);

    public class ViewBadgeProfileModel<TCell>
    {
        private volatile bool loading;
        private bool showRecommendations;
        private IBadgeProfileDTO Profile;
        private List<BadgeUserRecommendationDTO> recommendations;

        private readonly string currentUserId;
        private readonly AttributeCellFactory<TCell> cellFactory;
        private readonly IViewBadgeProfileController controller;
        private readonly Type resultType;

        private readonly List<TCell> profileDataCells = new List<TCell>();
        private readonly List<TCell> recommendationCells = new List<TCell>();

        public event Action OnRefreshing;
        public event Action OnRefreshed;
        public event Action OnWriteReviewButtonTouched;
        public event GoToMessagingHandler OnGoToMessaging;


        public IReadOnlyList<TCell> Cells => ShowRecommendations ? recommendationCells : profileDataCells;

        public bool ShowRecommendations
        {
            get
            {
                return showRecommendations;
            }

            private set
            {
                showRecommendations = value;
                controller?.ReloadCells();
            }
        }

        public ViewBadgeProfileModel(IViewBadgeProfileController controller, BadgeService badgeService, CellBuilder<TCell> cellBuilder)
        {
            this.cellFactory = new AttributeCellFactory<TCell>(badgeService, controller.BadgeSummary.BadgeId, cellBuilder);
            this.cellFactory.CellChanged += CellsSource_CellChanged;
            this.controller = controller;
            this.currentUserId = MehspotAppContext.Instance.AuthManager.AuthInfo.UserId;

            var genericParameter = typeof(IBadgeProfileValues)
                  .GetTypeInfo().Assembly.ExportedTypes
                  .FirstOrDefault(a => a.GetTypeInfo()
                  .GetCustomAttribute<ViewProfileDtoAttribute>()?.BadgeName == controller.BadgeSummary.BadgeName);
            resultType = typeof(BadgeProfileDTO<>).MakeGenericType(genericParameter);
        }

        public string GetTitle()
        {
            return (MehspotResources.ResourceManager.GetString(controller.BadgeSummary.BadgeName) ?? controller.BadgeSummary.BadgeName) + " Profile";
        }

        public async Task RefreshView()
        {
            if (loading)
                return;
            loading = true;

            OnRefreshing?.Invoke();
            this.controller.WindowTitle = GetTitle();

            var result = await cellFactory.BadgeService.GetBadgeProfileAsync(this.cellFactory.BadgeId, controller.SearchResultDTO.Details.UserId, resultType);
            if (result.IsSuccess)
            {
                this.Profile = result.Data;
                this.profileDataCells.Clear();
                this.profileDataCells.AddRange(await cellFactory.CreateCellsForObject(result.Data));

                this.controller.WindowTitle = $"{controller.BadgeSummary.BadgeName} {result.Data.Details.UserName}";
                this.controller.SetProfilePictureUrl(result.Data.Details.ProfilePicturePath);
                this.controller.Subdivision = result.Data.Details.SubdivisionName?.Trim();
                this.controller.Distance = Math.Round(controller.SearchResultDTO.Details.DistanceFrom ?? 0, 2) + " miles";
                this.controller.Likes = $"{controller.SearchResultDTO.Details.Likes} Likes / {controller.SearchResultDTO.Details.Recommendations} Recommendations";
                this.controller.FirstName = controller.SearchResultDTO.Details.FirstName;
                this.controller.HideFavoriteIcon = !controller.SearchResultDTO.Details.Favourite;
                this.controller.InfoLabel1 = result.Data.AdditionalInfo.InfoLabel1;
                this.controller.InfoLabel2 = result.Data.AdditionalInfo.InfoLabel2;
                LoadProfile();

            }
            else
            {
                controller.ViewHelper.ShowAlert("Error", "Can not load profile data");
                return;
            }

            OnRefreshed?.Invoke();
            loading = false;
        }

        public Task<Result> ToggleBadgeUserDescriptionAsync(BadgeUserDescriptionDTO dto)
        {
            return cellFactory.BadgeService.ToggleBadgeUserDescriptionAsync(dto);
        }

        public void LoadProfile()
        {
            this.ShowRecommendations = false;
        }

        public async Task LoadRecommendations()
        {
            if (recommendationCells.Count == 0)
            {
                controller.ViewHelper.ShowOverlay("Wait...");

                recommendationCells.Clear();
                var result = await cellFactory.BadgeService.GetBadgeRecommendationsAsync(this.controller.BadgeSummary.BadgeId, this.controller.SearchResultDTO.Details.UserId);
                if (result.IsSuccess)
                {
                    bool reviewed = false;
                    if (result.Data?.Recommendations != null)
                    {
                        foreach (var item in result.Data.Recommendations)
                        {
                            if (item.FromUserId == currentUserId)
                            {
                                reviewed = true;
                            }

                            recommendationCells.Add(cellFactory.CreateRecommendationCell(item));
                        }

                        this.recommendations = result.Data.Recommendations;
                    }

                    if (!reviewed)
                    {
                        var createRecommendationCell = cellFactory.CreateButtonCell("Write Recommendation");
                        createRecommendationCell.OnButtonTouched += a => OnWriteReviewButtonTouched?.Invoke();
                        recommendationCells.Insert(0, (TCell)createRecommendationCell);
                    }
                }

                controller.ViewHelper.HideOverlay();
            }

            this.ShowRecommendations = true;

            //TableView.Hidden = true;
            //TableView.Source = recommendationsDataSource;
            //TableView.ReloadData();
            //TableView.Hidden = false;
        }

        public async Task ToggleFavoriteAsync()
        {
            var dto = new BadgeUserDescriptionDTO
            {
                BadgeName = this.controller.BadgeSummary.BadgeName,
                Delete = this.controller.SearchResultDTO.Details.Favourite,
                EmployeeId = this.controller.SearchResultDTO.Details.UserId,
                Type = BadgeDescriptionTypeEnum.Favourite
            };

            this.controller.HideFavoriteIcon = this.controller.SearchResultDTO.Details.Favourite;
            var result = await ToggleBadgeUserDescriptionAsync(dto);
            if (result.IsSuccess)

            {
                this.controller.SearchResultDTO.Details.Favourite = !this.controller.SearchResultDTO.Details.Favourite;
            }
            else
            {
                this.controller.HideFavoriteIcon = !this.controller.SearchResultDTO.Details.Favourite;
            }
        }

        public void AddRecommendation(BadgeUserRecommendationDTO recommendation)
        {
            recommendationCells.Add(cellFactory.CreateRecommendationCell(recommendation));
        }

        public void HideCreateButton()
        {
            recommendationCells.RemoveAll(a => a is IButtonCell);
        }

        private void CellsSource_CellChanged(object obj, string propertyName, object value)
        {
            if (obj is BadgeProfileDetailsDTO)
            {
                if (propertyName == nameof(BadgeProfileDetailsDTO.IsHired))
                {
                    cellFactory.BadgeService.ToggleBadgeEmploymentHistoryAsync(Profile.Details.UserId, this.cellFactory.BadgeId, !(bool)value);
                }
                else if (propertyName == nameof(BadgeProfileDetailsDTO.HasReference))
                {
                    var dto = new BadgeUserDescriptionDTO
                    {
                        EmployeeId = Profile.Details.UserId,
                        BadgeName = controller.BadgeSummary.BadgeName,
                        Delete = !(bool)value,
                        Type = BadgeDescriptionTypeEnum.Reference
                    };

                    ToggleBadgeUserDescriptionAsync(dto);
                }
            }
        }

        public void RowSelected(int row)
        {
            if (ShowRecommendations)
            {
                var dto = recommendations[row];
                if (this.currentUserId != dto.FromUserId)
                {
                    OnGoToMessaging?.Invoke(dto.FromUserId, dto.FromUserName);
                }
            }
        }
    }
}



