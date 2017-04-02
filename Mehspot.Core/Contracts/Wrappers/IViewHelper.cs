namespace Mehspot.Core.Contracts.Wrappers
{
    public interface IViewHelper
    {
        void ShowAlert (string title, string text);

        void ShowOverlay (string text);

        void HideOverlay ();
    }
}
