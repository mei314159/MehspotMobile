using System;
using Mehspot.Core.Contracts.Wrappers;
using Mehspot.Core.DTO;
using Mehspot.Core.DTO.Search;

namespace Mehspot.Core
{
    public interface ISearchFilterController
    {
        IViewHelper ViewHelper { get; }

        BadgeSummaryDTO BadgeSummary { get; }
        string TitleKey { get; }

        void ReloadData();
    }
}
