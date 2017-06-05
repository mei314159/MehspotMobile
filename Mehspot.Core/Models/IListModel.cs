using System.Collections.Generic;

namespace Mehspot.Core.Models
{
    public interface IListModel<TCell>
    {
        IList<TCell> Cells { get; }
    }
}