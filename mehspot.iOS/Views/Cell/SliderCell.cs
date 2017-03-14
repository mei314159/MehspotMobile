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

        public static SliderCell Create<T,TProperty> (T Model, Expression<Func<T, TProperty>> property, string placeholder, float? minValue = null, float? maxValue = null, bool isReadOnly = false) where T : class
        {
            var cell = (SliderCell)Nib.Instantiate (null, null) [0];
            cell.CellSlider.Enabled = !isReadOnly;
            cell.FieldLabel.Text = placeholder;
            if (minValue.HasValue)
                cell.CellSlider.MinValue = minValue.Value;
            if (maxValue.HasValue)
                cell.CellSlider.MaxValue = maxValue.Value;
            var propertyValue = property.Compile ().Invoke (Model);
            cell.CellSlider.Value = (float?)(object)propertyValue ?? cell.CellSlider.MinValue;
            cell.ValueLabel.Text = cell.CellSlider.Value.ToString();

            var propertyType = Nullable.GetUnderlyingType (typeof (TProperty)) ?? typeof (TProperty);

            cell.CellSlider.ValueChanged += (sender, e) => {
                var value = ((UISlider)sender).Value;

                var val = (TProperty)Convert.ChangeType (value, propertyType);
                Model.SetProperty (property, val);
                cell.ValueLabel.Text = val.ToString();
            };

            return cell;
        }
    }
}
