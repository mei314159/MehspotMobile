// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace Mehspot.iOS.Views.Cell
{
    [Register ("RangeSliderCell")]
    partial class RangeSliderCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel FieldLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel MaxValueLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel MinValueLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        Xamarin.RangeSlider.RangeSliderControl RangeSlider { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (FieldLabel != null) {
                FieldLabel.Dispose ();
                FieldLabel = null;
            }

            if (MaxValueLabel != null) {
                MaxValueLabel.Dispose ();
                MaxValueLabel = null;
            }

            if (MinValueLabel != null) {
                MinValueLabel.Dispose ();
                MinValueLabel = null;
            }

            if (RangeSlider != null) {
                RangeSlider.Dispose ();
                RangeSlider = null;
            }
        }
    }
}