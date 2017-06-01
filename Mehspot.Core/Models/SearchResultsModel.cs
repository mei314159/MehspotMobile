using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Mehspot.Core.DTO;
using Mehspot.Core.DTO.Search;
using Mehspot.Core.Services;
using Mehspot.Models.ViewModels;

namespace Mehspot.Core
{
    public delegate void OnLoadingError(Result result);
    public class SearchResultsModel
    {
        private const int pageSize = 20;
        private const int limitedResultsCount = 5;
        private volatile bool loading;
        private volatile int selectedItemIndex;

        private readonly ISearchResultsController controller;
        private readonly BadgeService badgeService;
        private readonly Type resultType;
        private List<int> expandedRows = new List<int>();

        public event OnLoadingError OnLoadingError;
        public event Action LoadingMoreStarted;
        public event Action LoadingMoreEnded;


        public SearchResultsModel(ISearchResultsController controller, BadgeService badgeService)
        {
            this.controller = controller;
            this.badgeService = badgeService;

            var type = typeof(ISearchResultDTO).GetTypeInfo()
            .Assembly.ExportedTypes
            .FirstOrDefault(a => a.GetTypeInfo()
            .GetCustomAttribute<SearchResultDtoAttribute>()?.BadgeName == controller.BadgeSummary.BadgeName);
            resultType = typeof(List<>).MakeGenericType(type);
        }

        public List<ISearchResultDTO> Items { get; } = new List<ISearchResultDTO>();
        public ISearchResultDTO SelectedItem => Items?[selectedItemIndex];

        public bool RegisterButtonVisible =>
        controller.BadgeSummary.RequiredBadgeId.HasValue
         ? !this.controller.BadgeSummary.RequiredBadgeIsRegistered
         : !this.controller.BadgeSummary.IsRegistered;

        public string GetTitle()
        {
            var title = MehspotResources.ResourceManager.GetString(this.controller.BadgeSummary.BadgeName + "_SearchResultsTitle") ??
            ((MehspotResources.ResourceManager.GetString(this.controller.BadgeSummary.BadgeName) ?? this.controller.BadgeSummary.BadgeName) + "s");
            return title;
        }

        public int GetRowsCount()
        {
            var rowsCount = this.Items?.Count ?? 0;
            if (RegisterButtonVisible)
            {
                if (rowsCount > limitedResultsCount)
                {
                    rowsCount = limitedResultsCount + 1;
                }
                else
                {
                    rowsCount++;
                }
            }

            return rowsCount;
        }

        public async Task LoadDataAsync(bool refresh = false)
        {
            if (loading)
                return;
            
            loading = true;
            var skip = refresh ? 0 : (this.Items?.Count ?? 0);
            var result = await badgeService.Search(this.controller.SearchQuery, skip, pageSize, this.resultType);
            if (result.IsSuccess)
            {
                if (refresh)
                {
                    this.Items.Clear();
                }

                this.Items.AddRange(result.Data);
                this.controller.ReloadData();
            }
            else
            {
                OnLoadingError?.Invoke(result);
            }
            loading = false;
        }


        public async Task LoadMoreAsync()
        {
            if (!loading && !this.RegisterButtonVisible)
            {
                LoadingMoreStarted?.Invoke();
                await this.LoadDataAsync();
                LoadingMoreEnded?.Invoke();
            }
        }

        public void SelectItem(ISearchResultDTO dto)
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

        public void SelectRow(int row)
        {
            this.selectedItemIndex = row;
            if (expandedRows.Contains(row))
            {
                expandedRows.Remove(row);
            }
            else
            {
                expandedRows.Add(row);
            }
        }

        public bool IsRowExpanded(int row)
        {
            return expandedRows.Contains(row);
        }
    }
}
