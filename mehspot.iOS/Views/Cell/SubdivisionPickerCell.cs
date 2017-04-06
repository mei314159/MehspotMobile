using System;
using System.Linq;
using Foundation;
using mehspot.iOS.Controllers;
using mehspot.iOS.Extensions;
using Mehspot.DTO;
using UIKit;

namespace mehspot.iOS.Views
{
    [Register ("SubdivisionPickerCell")]
    public class SubdivisionPickerCell : UITableViewCell
    {
        private string placeholder;

        private Action<SubdivisionDTO> SetProperty;
        private Func<int, string> GetPropertyString;


        public static readonly NSString Key = new NSString ("SubdivisionPickerCell");
        public static readonly UINib Nib;

        static SubdivisionPickerCell ()
        {
            Nib = UINib.FromName ("SubdivisionPickerCell", NSBundle.MainBundle);
        }

        protected SubdivisionPickerCell (IntPtr handle) : base (handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        [Outlet]
        UIButton SelectValueButton { get; set; }

        [Outlet]
        UILabel FieldLabel { get; set; }

        public string FieldName { get; set; }

        public SubdivisionDTO [] Subdivisions { get; set; }

        public int? SelectedSubdivisionId { get; set; }

        public bool IsReadOnly {
            get {
                return this.SelectValueButton.Enabled;
            }
            set {
                this.SelectValueButton.Enabled = !value;
            }
        }

        public static SubdivisionPickerCell Create (int? selectedId, Action<SubdivisionDTO> setProperty, string label, SubdivisionDTO [] list, bool isReadOnly = false)
        {
            var cell = (SubdivisionPickerCell)Nib.Instantiate (null, null) [0];
            cell.placeholder = label;
            cell.IsReadOnly = isReadOnly;
            cell.FieldLabel.Text = label;
            cell.SelectedSubdivisionId = selectedId;
            cell.Subdivisions = list;
            cell.SetProperty = setProperty;
            cell.GetPropertyString = (value) => {
                return cell.Subdivisions.FirstOrDefault (a => Equals (a.Id, value)).DisplayName;
            };

            cell.SetSelectValueButtonTitle (selectedId);
            return cell;
        }


        [Action ("SelectValueButton_TouchUpInside:")]
        async void SelectValueButton_TouchUpInside (UIButton sender)
        {
            var cell = (SubdivisionPickerCell)(sender).Superview.Superview;
            var controller = cell.GetViewController ();

            var subdivisionsListController = SubdivisionsListController.Create ();
            subdivisionsListController.Subdivisions = this.Subdivisions;
            subdivisionsListController.SelectedSubdivisionId = this.SelectedSubdivisionId;
            subdivisionsListController.OnDismissed += (dto) => {
                cell.SetProperty (dto);
                cell.SetSelectValueButtonTitle (dto.Id);
                cell.SelectedSubdivisionId = dto.Id;
            };

            await controller.PresentViewControllerAsync (subdivisionsListController, true);
        }

        void SetSelectValueButtonTitle (int? selectedSubdivisionId)
        {
            SelectValueButton.SetTitle (selectedSubdivisionId.HasValue ? GetPropertyString (selectedSubdivisionId.Value) ?? this.placeholder : this.placeholder, UIControlState.Normal);
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
