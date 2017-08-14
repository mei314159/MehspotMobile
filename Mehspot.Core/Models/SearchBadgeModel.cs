using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Mehspot.Core.Builders;
using Mehspot.Core.DTO.Search;
using Mehspot.Core.Services;

namespace Mehspot.Core
{
    public class SearchBadgeModel<TCell>
    {
        volatile bool viewWasInitialized;

        private readonly ISearchFilterController controller;
        private readonly BadgeService badgeService;

        private readonly AttributeCellFactory<TCell> cellFactory;
        public readonly ISearchQueryDTO SearchQuery;
        public readonly List<TCell> Cells = new List<TCell>();

        public SearchBadgeModel(ISearchFilterController controller, BadgeService badgeService, CellBuilder<TCell> cellBuilder)
        {
            this.controller = controller;
            this.badgeService = badgeService;
            this.cellFactory = new AttributeCellFactory<TCell>(badgeService, controller.BadgeSummary.BadgeId, cellBuilder);
            this.SearchQuery = this.CreateQueryDTO(controller.BadgeSummary.BadgeId, controller.BadgeSummary.BadgeName);
        }


        public string GetTitle()
        {
            var name = this.controller.TitleKey ?? this.controller.BadgeSummary.BadgeName;
            var title = MehspotResources.ResourceManager.GetString("Find_" + name) ?? ("Search for " + name);
            return title;
        }


        private ISearchQueryDTO CreateQueryDTO(int badgeId, string badgeName)
        {
            var queryDtoType = typeof(SearchFilterDTOBase).GetTypeInfo().Assembly.ExportedTypes
            .FirstOrDefault(a => a.GetTypeInfo()
            .GetCustomAttribute<SearchFilterDtoAttribute>()?.BadgeName == badgeName);
            var result = (ISearchQueryDTO)Activator.CreateInstance(queryDtoType);
            result.BadgeId = badgeId;
            return result;
        }

        public async Task LoadCellsAsync()
        {
            if (viewWasInitialized)
                return;

            controller.ViewHelper.ShowOverlay("Wait...");

            this.Cells.Clear();
            this.Cells.AddRange(await this.cellFactory.CreateCellsForObject(this.SearchQuery));

            this.controller.ReloadData();

            controller.ViewHelper.HideOverlay();
            viewWasInitialized = true;
        }
    }
}
