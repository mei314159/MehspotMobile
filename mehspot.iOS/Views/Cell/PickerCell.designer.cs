// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace mehspot.iOS.Views
{
    [Register ("PickerCell")]
    partial class PickerCell
    {
        [Action ("SelectValueButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SelectValueButton_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (FieldLabel != null) {
                FieldLabel.Dispose ();
                FieldLabel = null;
            }

            if (SelectValueButton != null) {
                SelectValueButton.Dispose ();
                SelectValueButton = null;
            }
        }
    }
}