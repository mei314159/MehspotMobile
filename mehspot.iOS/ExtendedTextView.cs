using Foundation;
using System;
using UIKit;
using System.ComponentModel;

namespace Mehspot.iOS
{
    [Register("ExtendedTextView"), DesignTimeVisible(true)]
    public class ExtendedTextView : UITextView
    {
        string placeholder;

        public bool Multiline { get; set; } = true;

        [Export("Placeholder"), Browsable(true)]
        public string Placeholder
        {
            get
            {
                return placeholder;
            }

            set
            {
                placeholder = value;
                SetNeedsDisplay();
            }
        }

        public override bool Editable
        {
            get
            {
                return base.Editable;
            }
            set
            {
                base.Editable = value;
                this.TextColor = value ? UIColor.DarkTextColor : UIColor.LightGray;
            }
        }

        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                if (value != placeholder)
                {
                    this.TextColor = UIColor.DarkTextColor;
                }

                base.Text = value;
            }
        }

        public override UIColor TextColor
        {
            get
            {
                return base.TextColor;
            }
            set
            {
                base.TextColor = this.Editable ? value : UIColor.LightGray;
            }
        }

        public ExtendedTextView(IntPtr handle)
            : base(handle)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            Initialize();
        }

        void Initialize()
        {
            ShouldBeginEditing = t =>
            {
                HidePlaceholder();

                return true;
            };
            ShouldEndEditing = t =>
            {
                ShowPlaceholder();

                return true;
            };

            EnablesReturnKeyAutomatically = true;
            this.ShouldChangeText += (textView, range, text) =>
            {
                return text != Environment.NewLine || this.Multiline;
            };

            ShowPlaceholder();
        }

        public override bool BecomeFirstResponder()
        {
            HidePlaceholder();

            return base.BecomeFirstResponder();
        }

        void HidePlaceholder()
        {
            if (Text == Placeholder)
            {
                Text = string.Empty;
                this.TextColor = UIColor.DarkTextColor;
            }
        }

        public void ShowPlaceholder()
        {
            if (string.IsNullOrEmpty(Text))
            {
                Text = Placeholder;
                this.TextColor = UIColor.LightGray;
            }
        }
    }
}