// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace mehspot.iOS.Views.Cell
{
    [Register ("SliderCell")]
    partial class SliderCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISlider CellSlider { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel FieldLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel ValueLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (CellSlider != null) {
                CellSlider.Dispose ();
                CellSlider = null;
            }

            if (FieldLabel != null) {
                FieldLabel.Dispose ();
                FieldLabel = null;
            }

            if (ValueLabel != null) {
                ValueLabel.Dispose ();
                ValueLabel = null;
            }
        }
    }
}