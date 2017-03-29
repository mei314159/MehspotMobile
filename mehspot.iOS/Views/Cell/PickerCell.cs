using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using mehspot.iOS.Extensions;
using SharpMobileCode.ModalPicker;
using UIKit;

namespace mehspot.iOS.Views
{
    [Register ("PickerCell")]
    public class PickerCell : UITableViewCell
    {
        private const string dateFormat = "MMMM dd, yyyy";
        private Type propertyType;
        private Action<object> SetProperty;
        private Func<object> GetProperty;
        private Func<object, string> GetPropertyString;

        private string Placeholder;

        public static readonly NSString Key = new NSString ("PickerCell");
        public static readonly UINib Nib;

        static PickerCell ()
        {
            Nib = UINib.FromName ("PickerCell", NSBundle.MainBundle);
        }

        protected PickerCell (IntPtr handle) : base (handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        [Outlet]
        UIButton SelectValueButton { get; set; }

        [Outlet]
        UILabel FieldLabel { get; set; }

        public KeyValuePair<object, string> [] RowValues { get; set; }


        public bool IsReadOnly {
            get {
                return this.SelectValueButton.Enabled;
            }
            set {
                this.SelectValueButton.Enabled = !value;
            }
        }
        public static PickerCell Create<T, TProperty> (
            T model,
            Func<T, TProperty> getProperty,
            Action<T, TProperty> setProperty,
            Func<TProperty, string> getPropertyString,
            string label,
            IEnumerable<KeyValuePair<TProperty, string>> rowValues = null,
            bool isReadOnly = false)
        {
            var cell = (PickerCell)Nib.Instantiate (null, null) [0];
            cell.Placeholder = label;
            cell.RowValues = rowValues?.Select (a => new KeyValuePair<object, string> (a.Key, a.Value)).ToArray ();
            cell.propertyType = typeof (TProperty);
            cell.GetProperty = () => getProperty (model);
            cell.GetPropertyString = (s) => getPropertyString ((TProperty)s);
            cell.SetProperty = (p) => {
                setProperty (model, (TProperty)p);
                cell.SetSelectValueButtonTitle ();
            };
            cell.SelectValueButton.TouchUpInside += SelectValueButton_TouchUpInside;
            cell.IsReadOnly = isReadOnly;
            cell.FieldLabel.Text = label;
            cell.SetSelectValueButtonTitle ();
            return cell;
        }



        public static async void SelectValueButton_TouchUpInside (object sender, EventArgs e)
        {
            var cell = (PickerCell)((UIButton)sender).Superview.Superview;
            var controller = cell.GetViewController ();

            var modalPicker = new ModalPickerViewController (ModalPickerType.Date, cell.Placeholder, controller) {
                HeaderBackgroundColor = UIColor.White,
                HeaderTextColor = UIColor.DarkGray,
                TransitioningDelegate = new ModalPickerTransitionDelegate (),
                ModalPresentationStyle = UIModalPresentationStyle.Custom,
            };

            var initialValue = cell.GetProperty ();
            var isDateTimePicker = cell.propertyType == typeof (DateTime) || cell.propertyType == typeof (DateTime?);
            if (isDateTimePicker) {

                DateTime? dateValue;
                dateValue = initialValue as DateTime?;
                modalPicker.DatePicker.Mode = UIDatePickerMode.Date;
                modalPicker.DatePicker.TimeZone = NSTimeZone.FromGMT (0);
                if (dateValue.HasValue) {
                    modalPicker.DatePicker.SetDate (dateValue.Value.DateTimeToNSDate (), false);
                }
                modalPicker.OnModalPickerDismissed += (s, ea) => {
                    cell.SetProperty (modalPicker.DatePicker.Date.NSDateToDateTime ().Date);
                };
            } else {

                modalPicker.PickerType = ModalPickerType.Custom;
                nint selectedRow = 0;
                if (cell.RowValues != null) {
                    for (int i = 0; i < cell.RowValues.Length; i++) {
                        var item = cell.RowValues [i];
                        if (item.Key == initialValue) {
                            selectedRow = i;
                            break;
                        }
                    }
                    modalPicker.PickerView.Model = new CustomPickerModel (cell.RowValues.Select (a => a.Value).ToList ());
                    modalPicker.PickerView.Select (selectedRow, 0, false);
                }
                modalPicker.OnModalPickerDismissed += (s, ea) => {
                    var index = modalPicker.PickerView.SelectedRowInComponent (0);
                    var value = cell.RowValues? [(int)index].Key;
                    cell.SetProperty (value);
                };
            }
            await controller.PresentViewControllerAsync (modalPicker, true);
        }

        void SetSelectValueButtonTitle ()
        {
            var title = GetPropertyString (GetProperty ()) ?? this.Placeholder;
            SelectValueButton.SetTitle (title, UIControlState.Normal);
        }

        void ReleaseDesignerOutlets ()
        {
            if (SelectValueButton != null) {
                SelectValueButton.Dispose ();
                SelectValueButton = null;
            }

            if (FieldLabel != null) {
                FieldLabel.Dispose ();
                FieldLabel = null;
            }
        }
    }
}
