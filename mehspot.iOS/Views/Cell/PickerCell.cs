using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using mehspot.iOS.Extensions;
using Mehspot.iOS.Views.MultiSelectPicker;
using SharpMobileCode.ModalPicker;
using UIKit;

namespace mehspot.iOS.Views
{
    [Register ("PickerCell")]
    public class PickerCell : UITableViewCell
    {
        private const string dateFormat = "MMMM dd, yyyy";
        private Type propertyType;
        private PickerTypeEnum pickerType;
        private string placeholder;

        private Action<object> SetProperty;
        private Func<object> GetProperty;
        private Func<object, string> GetPropertyString;

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
            string label,
            IEnumerable<KeyValuePair<TProperty, string>> rowValues,
            bool isReadOnly = false)
        {
            var cell = (PickerCell)Nib.Instantiate (null, null) [0];
            cell.pickerType = PickerTypeEnum.List;
            cell.placeholder = label;
            cell.RowValues = rowValues?.Select (a => new KeyValuePair<object, string> (a.Key, a.Value)).ToArray ();
            cell.propertyType = typeof (TProperty);
            cell.GetProperty = () => getProperty (model);
            cell.GetPropertyString = (s) => {
                return rowValues.FirstOrDefault (a => Equals (a.Key, (TProperty)s)).Value;
            };

            cell.SetProperty = (p) => {
                setProperty (model, (TProperty)p);
                cell.SetSelectValueButtonTitle ();
            };
            cell.IsReadOnly = isReadOnly;
            cell.FieldLabel.Text = label;
            cell.SetSelectValueButtonTitle ();
            return cell;
        }

        public static PickerCell Create<T, TProperty> (
            T model,
            Func<T, TProperty[]> getProperty,
            Action<T, TProperty[]> setProperty,
            string label,
            IEnumerable<KeyValuePair<TProperty, string>> rowValues,
            bool isReadOnly = false)
        {
            var cell = (PickerCell)Nib.Instantiate (null, null) [0];
            cell.pickerType = PickerTypeEnum.Multiselect;
            cell.placeholder = label;
            cell.RowValues = rowValues?.Select (a => new KeyValuePair<object, string> (a.Key, a.Value)).ToArray ();
            cell.propertyType = typeof (TProperty);
            cell.GetProperty = () => getProperty (model);
            cell.GetPropertyString = (s) => {
                var values = (TProperty [])s;
                if (values != null) {
                    var valuesStrings = rowValues.Where (a => values.Contains (a.Key)).Select (a => a.Value).ToArray ();
                    if (valuesStrings != null) {
                        return string.Join (",", valuesStrings);
                    }
                }
                return null;
            };

            cell.SetProperty = (p) => {
                setProperty (model, ((IEnumerable)p).Cast<TProperty>().ToArray());
                cell.SetSelectValueButtonTitle ();
            };
            cell.IsReadOnly = isReadOnly;
            cell.FieldLabel.Text = label;
            cell.SetSelectValueButtonTitle ();
            return cell;
        }

        public static PickerCell CreateDatePicker<T> (
            T model,
            Func<T, DateTime?> getProperty,
            Action<T, DateTime?> setProperty,
            string label,
            bool isReadOnly = false)
        {
            var cell = (PickerCell)Nib.Instantiate (null, null) [0];
            cell.pickerType = PickerTypeEnum.Date;
            cell.placeholder = label;
            cell.propertyType = typeof (DateTime?);
            cell.GetProperty = () => getProperty (model);
            cell.GetPropertyString = (s) => ((DateTime?)s)?.ToString (dateFormat);
            cell.SetProperty = (p) => {
                setProperty (model, (DateTime?)p);
                cell.SetSelectValueButtonTitle ();
            };
            cell.IsReadOnly = isReadOnly;
            cell.FieldLabel.Text = label;
            cell.SetSelectValueButtonTitle ();
            return cell;
        }

        [Action ("SelectValueButton_TouchUpInside:")]
        async void SelectValueButton_TouchUpInside (UIButton sender)
        {
            var cell = (PickerCell)(sender).Superview.Superview;
            var controller = cell.GetViewController ();

            var modalPicker = new ModalPickerViewController (ModalPickerType.Date, cell.placeholder, controller) {
                HeaderBackgroundColor = UIColor.White,
                HeaderTextColor = UIColor.DarkGray,
                TransitioningDelegate = new ModalPickerTransitionDelegate (),
                ModalPresentationStyle = UIModalPresentationStyle.Custom,
            };

            var initialValue = cell.GetProperty ();
            if (cell.pickerType == PickerTypeEnum.Date) {

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

                await controller.PresentViewControllerAsync (modalPicker, true);

            } else if (cell.pickerType == PickerTypeEnum.List) {

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

                await controller.PresentViewControllerAsync (modalPicker, true);
            } else if (cell.pickerType == PickerTypeEnum.Multiselect) {
                var values = cell.GetProperty () as IEnumerable;
                var rows = cell.RowValues.Select ((x, i) => new SourceItem { Name = x.Value, Selected = values != null && values.Cast<object> ().Any (a => Equals (x.Key, a)) }).ToArray ();
                var picker = new MultiSelectPickerViewController (cell.placeholder, controller, rows) {
                    HeaderBackgroundColor = UIColor.White,
                    HeaderTextColor = UIColor.DarkGray,
                    TransitioningDelegate = new ModalPickerTransitionDelegate (),
                    ModalPresentationStyle = UIModalPresentationStyle.Custom,
                };

                picker.OnModalPickerDismissed += (indexes) => {
                    var rowValues = cell.RowValues;
                    cell.SetProperty (indexes.Select (a => rowValues [a].Key).ToArray ());
                };
                await controller.PresentViewControllerAsync (picker, true);
            }
        }

        void SetSelectValueButtonTitle ()
        {
            var v = GetProperty ();
            var title = GetPropertyString (v) ?? this.placeholder;
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

        enum PickerTypeEnum
        {
            Date,
            List,
            Multiselect
        }
    }
}
