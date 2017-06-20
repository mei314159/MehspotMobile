using System;
using System.Collections.Generic;
using Mehspot.Core.Contracts.Wrappers;
using Mehspot.Core.DTO;
using Mehspot.Core.DTO.Subdivision;

namespace Mehspot.Core
{
    public interface IProfileViewController
    {
        IViewHelper ViewHelper { get; }

        string UserName { get; set; }
        string FullName { get; set; }
        string ProfilePicturePath { get; set; }
        bool SaveButtonEnabled { get; set; }

        void ReloadData();
        void HideKeyboard();
    }
}
