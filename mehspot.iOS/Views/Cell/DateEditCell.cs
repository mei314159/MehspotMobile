using System;
using System.Linq.Expressions;
using Foundation;
using mehspot.iOS.Extensions;
using SharpMobileCode.ModalPicker;
using UIKit;

namespace mehspot.iOS.Views
{
    public partial class DateEditCell : UITableViewCell
    {
        private const string dateFormat = "MMMM dd, yyyy";
        private Action<DateTime> SetModel;
        private Func<DateTime?> GetModel;

        public static readonly NSString Key = new NSString ("DateEditCell");
        public static readonly UINib Nib;

        static DateEditCell ()
        {
            Nib = UINib.FromName ("DateEditCell", NSBundle.MainBundle);
        }

        protected DateEditCell (IntPtr handle) : base (handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public static DateEditCell Create<T> (T model, Expression<Func<T, DateTime?>> property, string placeholder, bool isReadOnly = false) where T : class
        {
            var cell = (DateEditCell)Nib.Instantiate (null, null) [0];
            cell.ChangeDateButton.Enabled = !isReadOnly;
            cell.FieldLabel.Text = placeholder;
            Func<T, DateTime?> getProperty = property.Compile ();
            cell.ChangeDateButton.SetTitle (getProperty(model)?.ToString (dateFormat), UIControlState.Normal);


            cell.GetModel = () => getProperty(model);
            cell.SetModel = (DateTime date) => {
                cell.ChangeDateButton.SetTitle (date.ToString (dateFormat), UIControlState.Normal);
                model.SetProperty (property, date);
            };

            return cell;
        }

        async partial void EditDateButtonTouched (UIButton sender)
        {
            var cell = (DateEditCell)sender.Superview.Superview;
            var controller = cell.GetViewController ();
            var modalPicker = new ModalPickerViewController (ModalPickerType.Date, "Select A Date", controller) {
                HeaderBackgroundColor = UIColor.White,
                HeaderTextColor = UIColor.DarkGray,
                TransitioningDelegate = new ModalPickerTransitionDelegate (),
                ModalPresentationStyle = UIModalPresentationStyle.Custom,
            };
            modalPicker.DatePicker.Mode = UIDatePickerMode.Date;

            var initialDate = cell.GetModel ();
            if (initialDate != null) {
                modalPicker.DatePicker.SetDate (initialDate.Value.DateTimeToNSDate (), false);
            }

            modalPicker.OnModalPickerDismissed += (s, ea) => {
                cell.SetModel (modalPicker.DatePicker.Date.NSDateToDateTime ());
            };

            await controller.PresentViewControllerAsync (modalPicker, true);
        }

    }
}
