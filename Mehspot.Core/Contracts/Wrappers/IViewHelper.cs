using System;

namespace Mehspot.Core.Contracts.Wrappers
{
    public interface IViewHelper
    {
        void ShowAlert(string title, string text, Action action = null);

        void ShowPrompt(string title, string text, string positiveButtonTitle, Action positiveAction);

        void ShowOverlay(string text, bool opaque = false);

        void HideOverlay();
    }
}
