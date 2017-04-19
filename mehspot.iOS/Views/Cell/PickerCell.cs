using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using Foundation;
using mehspot.iOS.Extensions;
using Mehspot.iOS.Views.CustomPicker;
using Mehspot.iOS.Views.MultiSelectPicker;
using SharpMobileCode.ModalPicker;
using UIKit;

namespace mehspot.iOS.Views
{
    [Register ("PickerCell")]
    public class PickerCell : UITableViewCell
    {
        private const string dateFormat = "MMMM dd, yyyy";
        private PickerTypeEnum pickerType;
        private string placeholder;

        private object Value;
        private Action<object> SetProperty;
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

        public string FieldName { get; set; }

        public KeyValuePair<object, string> [] RowValues { get; set; }


        public bool IsReadOnly {
            get {
                return this.SelectValueButton.Enabled;
            }
            set {
                this.SelectValueButton.Enabled = !value;
            }
        }

        private static PickerCell Instantiate (object initialValue, PickerTypeEnum type, string label, string placeholder = null, bool isReadOnly = false)
        {

            var cell = (PickerCell)Nib.Instantiate (null, null) [0];
            cell.placeholder = placeholder ?? label;
            cell.IsReadOnly = isReadOnly;
            cell.pickerType = type;
            cell.FieldLabel.Text = label;
            cell.Value = initialValue;
            return cell;
        }

        public static PickerCell Create<T> (
            T initialValue,
            Action<T> setProperty,
            string label,
            IEnumerable<KeyValuePair<T, string>> rowValues,
            string placeholder = null,
            bool isReadOnly = false)
        {
            var cell = Instantiate (initialValue, PickerTypeEnum.List, label, placeholder, isReadOnly);
            cell.RowValues = rowValues?.Select (a => new KeyValuePair<object, string> (a.Key, a.Value)).ToArray ();

            cell.GetPropertyString = (value) => {
                return cell.RowValues?.FirstOrDefault (a => Equals (a.Key, value)).Value;
            };
            cell.SetProperty = (p) => {
                cell.Value = p;
                setProperty ((T)p);
                cell.SetSelectValueButtonTitle (p);
            };
            cell.SetSelectValueButtonTitle (initialValue);
            return cell;
        }

        public static PickerCell CreateMultiselect<TProperty> (
            IEnumerable<TProperty> initialValue,
            Action<IEnumerable<TProperty>> setProperty,
            string label,
            IEnumerable<KeyValuePair<TProperty, string>> rowValues,
            bool isReadOnly = false)
        {
            var cell = Instantiate (initialValue, PickerTypeEnum.Multiselect, label, null, isReadOnly);
            cell.RowValues = rowValues?.Select (a => new KeyValuePair<object, string> (a.Key, a.Value)).ToArray ();

            cell.GetPropertyString = (value) => {
                if (value != null) {
                    var values = ((IEnumerable)value).Cast<TProperty> ();
                    if (values != null) {
                        var valuesStrings = cell.RowValues.Join (values, a => a.Key, a => a, (a, b) => a.Value).ToArray ();
                        if (valuesStrings != null) {
                            return string.Join (Environment.NewLine, valuesStrings);
                        }
                    }
                }
                return null;
            };

            cell.SetProperty = (p) => {
                cell.Value = p;
                setProperty (p != null ? ((IEnumerable)p).Cast<TProperty> () : null);
                cell.SetSelectValueButtonTitle (p);
            };
            cell.SetSelectValueButtonTitle (initialValue);
            return cell;
        }

        public static PickerCell CreateDatePicker<T> (
            T initialValue,
            Action<T> setProperty,
            string label,
            bool isReadOnly = false)
        {
            var cell = Instantiate (initialValue, PickerTypeEnum.Date, label, null, isReadOnly);

            cell.GetPropertyString = (value) => ((DateTime?)value)?.ToString (dateFormat);
            cell.SetProperty = (p) => {
                cell.Value = p;
                setProperty ((T)p);
                cell.SetSelectValueButtonTitle (p);
            };

            cell.SetSelectValueButtonTitle (initialValue);
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

            if (cell.pickerType == PickerTypeEnum.Date) {

                DateTime? dateValue;
                dateValue = cell.Value as DateTime?;
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
                        if (item.Key == cell.Value) {
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
                var values = cell.Value as IEnumerable;
                var rows = cell.RowValues.Select ((x, i) => new SourceItem { Name = x.Value, Selected = values != null && values.Cast<object> ().Any (a => Equals (x.Key, a)) }).ToArray ();
                var picker = new MultiSelectPickerViewController (cell.placeholder, controller, rows) {
                    HeaderBackgroundColor = UIColor.White,
                    HeaderTextColor = UIColor.DarkGray,
                    TransitioningDelegate = new ModalPickerTransitionDelegate (),
                    ModalPresentationStyle = UIModalPresentationStyle.Custom,
                };

                picker.OnModalPickerDismissed += (indexes) => {
                    object value = null;
                    if (indexes != null) {
                        value = indexes.Select (a => cell.RowValues [a].Key).ToArray ();
                    }

                    cell.SetProperty (value);
                };
                await controller.PresentViewControllerAsync (picker, true);
            }
        }

        void SetSelectValueButtonTitle (object value)
        {
            string title = null;
            if (value != null) {
                var val = value as IEnumerable;
                if (!(val is string) && val != null) {
                    var count = val.Count ();
                    if (count > 0)
                        title = $"{count} items";
                } else {
                    title = GetPropertyString (value);
                }
            }

            SelectValueButton.SetTitle (title ?? this.placeholder, UIControlState.Normal);
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
