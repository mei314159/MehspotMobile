using System;
using Android.Content;
using Android.Text;
using Android.Views;
using Android.Widget;
using Mehspot.AndroidApp.Views;
using Mehspot.Core.Builders;

namespace Mehspot.AndroidApp
{
    public class TextEditCell : RelativeLayout, ITextEditCell
    {
        private const int DefaultMaxLength = 9;

        public Action<ITextEditCell, string> SetModelProperty;
        public event Action<TextEditCell, string> ValueChanged;

        public bool IsValid => TextInput.IsValid;

        public TextEditCell(Context context) : base(context)
        {
            LayoutInflater inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
            inflater.Inflate(Resource.Layout.TextEditCell, this);
        }

        public TextEditCell(Context context, string initialValue, Action<ITextEditCell, string> setProperty, string label, Mehspot.Core.Builders.KeyboardType type = Mehspot.Core.Builders.KeyboardType.Default, string placeholder = null, bool isReadOnly = false, string mask = null, string validationRegex = null) : this(context)
        {
            FieldLabel.Text = label;
            this.TextInput.Hint = placeholder ?? label;
            this.TextInput.Enabled = !isReadOnly;
            this.TextInput.Text = initialValue;
            this.TextInput.TextChanged += TextInput_TextChanged;
            this.SetModelProperty = setProperty;
			this.TextInput.SetKeyboardType(type, DefaultMaxLength);
            this.TextInput.Mask = mask;
            this.TextInput.ValidationRegex = validationRegex;
        }

        public TextView FieldLabel => this.FindViewById<TextView>(Resource.TextEditCell.FieldLabel);

        public ExtendedEditText TextInput => this.FindViewById<ExtendedEditText>(Resource.TextEditCell.TextInput);

        public bool Multiline
        {
            get
            {
                return TextInput.Multiline;
            }
            set
            {
                TextInput.Multiline = value;
            }
        }

        public bool Hidden
        {
            get
            {
                return this.Visibility == ViewStates.Gone;
            }

            set
            {
                this.Visibility = value ? ViewStates.Gone : ViewStates.Visible;
            }
        }

        public string Text
        {
            get
            {
                return this.TextInput?.Text;
            }
            set
            {
                this.TextInput.Text = value;
            }
        }

        public bool Editable
        {
            get
            {
                return this.TextInput.Enabled;
            }

            set
            {
                this.TextInput.Enabled = value;
            }
        }

        public int? MaxLength
        {
            get
            {
                return TextInput.MaxLength;
            }
            set
            {
                TextInput.MaxLength = value;
            }
        }

        private void TextInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.SetModelProperty(this, this.Text);
            this.ValueChanged?.Invoke(this, this.Text);
        }

        public void SetKeyboardType(Mehspot.Core.Builders.KeyboardType type)
        {
            this.TextInput.SetKeyboardType(type, DefaultMaxLength);
        }

        public void UpdateSize()
        {
            
        }
    }

}
