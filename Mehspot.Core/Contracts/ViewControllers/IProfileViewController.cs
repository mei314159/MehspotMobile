using System;
using Mehspot.Core.Contracts.Wrappers;

namespace Mehspot.Core
{
    public interface IProfileViewController
    {
        IViewHelper ViewHelper { get; }

        string UserName { get; set; }
        string FullName { get; set; }
        string ProfilePicturePath { get; set; }
        bool SaveButtonEnabled { get; set; }
        bool IsActive { get; }
        void ReloadData();
        void HideKeyboard();

        void InvokeOnMainThread(Action action);
    }
}
