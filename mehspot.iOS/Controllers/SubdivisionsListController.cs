using System;
using Foundation;
using Mehspot.DTO;
using ObjCRuntime;
using UIKit;

namespace mehspot.iOS.Controllers
{
    public partial class SubdivisionsListController : UIViewController
    {
        public static readonly UINib Nib;
        public event Action<SubdivisionDTO> OnDismissed;

        public SubdivisionsListController (IntPtr handle) : base (handle)
        {
        }



        public SubdivisionDTO [] Subdivisions { get; set; }

        public static SubdivisionsListController Create ()
        {
            var arr = NSBundle.MainBundle.LoadNib ("SubdivisionsListController", null, null);
            var v = Runtime.GetNSObject<SubdivisionsListController> (arr.ValueAt (0));
            return v;
        }

        partial void SaveButtonTouched (UIBarButtonItem sender)
        {
            
        }

        partial void CloseButtonTouched (UIBarButtonItem sender)
        {
            DismissViewController (true, null);
        }
    }
}

