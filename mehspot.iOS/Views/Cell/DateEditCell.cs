using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using mehspot.iOS.Extensions;
using SharpMobileCode.ModalPicker;
using UIKit;

namespace mehspot.iOS.Views
{
    [Register ("DateEditCell")]
    public class DateEditCell : UITableViewCell
    {
        private const string dateFormat = "MMMM dd, yyyy";
        private object Model;
        private Action<object> SetProperty;
        private Func<object> GetProperty;
        private Func<object, string> GetPropertyString;
        private KeyValuePair<object, string> [] RowValues;
        private string Placeholder;

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

        [Outlet]
        UIButton ChangeDateButton { get; set; }

        [Outlet]
        UILabel FieldLabel { get; set; }




        public static DateEditCell Create<T, TProperty> (
            T model,
            Func<T, TProperty> getProperty,
            Action<T, TProperty> setProperty,
            Func<TProperty, string> getPropertyString,
            string placeholder,
            IEnumerable<KeyValuePair<TProperty, string>> rowValues = null,
            bool isReadOnly = false)
        {
            var cell = (DateEditCell)Nib.Instantiate (null, null) [0];
            cell.Placeholder = placeholder;
            cell.RowValues = rowValues?.Select (a => new KeyValuePair<object, string> (a.Key, a.Value)).ToArray ();
            cell.GetProperty = () => getProperty (model);
            cell.GetPropertyString = (s) => getPropertyString ((TProperty)s);
            cell.SetProperty = (p) => {
                setProperty (model, (TProperty)p);
                cell.SetChangeValueButtonTitle ();
            };

            cell.ChangeDateButton.Enabled = !isReadOnly;
            cell.FieldLabel.Text = placeholder;
            cell.SetChangeValueButtonTitle ();
            return cell;
        }

        [Action ("EditDateButtonTouched:")]
        async void EditDateButtonTouched (UIButton sender)
        {
            var cell = (DateEditCell)sender.Superview.Superview;
            var controller = cell.GetViewController ();

            var modalPicker = new ModalPickerViewController (ModalPickerType.Date, cell.Placeholder, controller) {
                HeaderBackgroundColor = UIColor.White,
                HeaderTextColor = UIColor.DarkGray,
                TransitioningDelegate = new ModalPickerTransitionDelegate (),
                ModalPresentationStyle = UIModalPresentationStyle.Custom,
            };

            var isDateTimePicker = false;
            var initialValue = cell.GetProperty ();
            if (initialValue != null) {
                DateTime? dateValue;
                if (initialValue is DateTime || initialValue is DateTime?) {
                    isDateTimePicker = true;
                    dateValue = initialValue as DateTime?;
                    modalPicker.DatePicker.Mode = UIDatePickerMode.Date;
                    modalPicker.DatePicker.TimeZone = NSTimeZone.FromGMT (0);
                    modalPicker.DatePicker.SetDate (dateValue.Value.DateTimeToNSDate (), false);
                    modalPicker.OnModalPickerDismissed += (s, ea) => {
                        cell.SetProperty (modalPicker.DatePicker.Date.NSDateToDateTime ().Date);
                    };
                }
            }

            if (!isDateTimePicker) {
                nint selectedRow = 0;
                for (int i = 0; i < cell.RowValues.Length; i++) {
                    var item = cell.RowValues [i];
                    if (item.Key == initialValue) {
                        selectedRow = i;
                        break;
                    }
                }

                modalPicker.PickerType = ModalPickerType.Custom;
                modalPicker.PickerView.Model = new CustomPickerModel (cell.RowValues.Select (a => a.Value).ToList ());
                modalPicker.PickerView.Select (selectedRow, 0, false);
                modalPicker.OnModalPickerDismissed += (s, ea) => {
                    var index = modalPicker.PickerView.SelectedRowInComponent (0);
                    var value = cell.RowValues [(int)index].Key;
                    cell.SetProperty (value);
                };
            }


            await controller.PresentViewControllerAsync (modalPicker, true);
        }

        void SetChangeValueButtonTitle ()
        {
            ChangeDateButton.SetTitle (GetPropertyString (GetProperty ()), UIControlState.Normal);
        }

        void ReleaseDesignerOutlets ()
        {
            if (ChangeDateButton != null) {
                ChangeDateButton.Dispose ();
                ChangeDateButton = null;
            }

            if (FieldLabel != null) {
                FieldLabel.Dispose ();
                FieldLabel = null;
            }
        }
    }
}
