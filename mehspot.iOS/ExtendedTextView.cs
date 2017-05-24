using Foundation;
using System;
using UIKit;
using System.ComponentModel;
using CoreGraphics;

namespace Mehspot.iOS
{
    [Register ("ExtendedTextView"), DesignTimeVisible (true)]
    public class ExtendedTextView : UITextView
    {
        string placeholder;

        [Export ("Placeholder"), Browsable (true)]
        public string Placeholder {
            get {
                return placeholder;
            }

            set {
                placeholder = value;
                SetNeedsDisplay();
            }
        }

        public ExtendedTextView (IntPtr handle)
            : base (handle)
        {
        }

        public override void AwakeFromNib ()
        {
            base.AwakeFromNib ();
            
            Initialize ();
        }

        void Initialize ()
        {
            ShouldBeginEditing = t => {
                HidePlaceholder ();

                return true;
            };
            ShouldEndEditing = t => {
                ShowPlaceholder ();

                return true;
            };

            ShowPlaceholder ();
        }

        public override bool BecomeFirstResponder ()
        {
            HidePlaceholder ();

            return base.BecomeFirstResponder ();
        }

        void HidePlaceholder ()
        {
            if (Text == Placeholder) {
                Text = string.Empty;
                this.TextColor = UIColor.DarkTextColor;
            }
        }

        void ShowPlaceholder ()
        {
            if (string.IsNullOrEmpty (Text)) {
                Text = Placeholder;
                this.TextColor = UIColor.LightGray;
            }
        }
   }
}