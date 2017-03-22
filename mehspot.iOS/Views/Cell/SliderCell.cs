using System;
using System.Linq.Expressions;
using Foundation;
using mehspot.iOS.Extensions;
using UIKit;

namespace mehspot.iOS.Views.Cell
{
    public partial class SliderCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString ("SliderCell");
        public static readonly UINib Nib;

        static SliderCell ()
        {
            Nib = UINib.FromName ("SliderCell", NSBundle.MainBundle);
        }

        protected SliderCell (IntPtr handle) : base (handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public static SliderCell Create<T,TProperty> (T Model, Expression<Func<T, TProperty>> property, string placeholder, float? minValue, float? maxValue, bool isReadOnly = false) where T : class
        {
            var cell = (SliderCell)Nib.Instantiate (null, null) [0];
            cell.CellSlider.Enabled = !isReadOnly;
            cell.FieldLabel.Text = placeholder;
            if (minValue.HasValue) {
                cell.CellSlider.MinValue = minValue.Value - 1;
            }

            if (maxValue.HasValue) {
                cell.CellSlider.MaxValue = maxValue.Value - 1;
            }

            var propertyValue = property.Compile ().Invoke (Model);
            cell.CellSlider.Value = (float?)(object)propertyValue ?? cell.CellSlider.MinValue;
            cell.SetValueLabel (cell.CellSlider.Value == cell.CellSlider.MinValue ? string.Empty : cell.CellSlider.Value.ToString ());

            var propertyType = Nullable.GetUnderlyingType (typeof (TProperty)) ?? typeof (TProperty);

            cell.CellSlider.ValueChanged += (sender, e) => {
                var slider = (UISlider)sender;
                var value = slider.Value;
                var val = (TProperty)Convert.ChangeType (value, propertyType);
                var clearValue = slider.Value == slider.MinValue;
                if (clearValue) {
                    val = default(TProperty);
                }

                cell.SetValueLabel (clearValue ? string.Empty : val?.ToString ());
                Model.SetProperty (property, val);
            };

            return cell;
        }

        private void SetValueLabel (string value) {
            this.ValueLabel.Text = value;
        }
    }
}
