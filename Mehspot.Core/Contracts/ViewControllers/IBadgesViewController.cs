using Mehspot.Core.Contracts.Wrappers;
using Mehspot.Core.DTO;

namespace Mehspot.Core.Contracts.ViewControllers
{

	public interface IBadgesViewController
	{
		IViewHelper ViewHelper { get; }

        void DisplayBadges();
    }
}
