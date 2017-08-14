using Mehspot.Core.Contracts.Wrappers;
using Mehspot.Core.DTO;
using Mehspot.Core.DTO.Search;

namespace Mehspot.Core
{

    public interface ISearchResultsController
    {
        IViewHelper ViewHelper { get; }

        BadgeSummaryDTO BadgeSummary { get; }

        ISearchQueryDTO SearchQuery { get; }

        string TitleKey { get; }

        void ReloadData();
    }
}
