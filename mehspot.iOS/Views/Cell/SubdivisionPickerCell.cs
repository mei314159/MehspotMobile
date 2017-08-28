﻿using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using Mehspot.iOS.Controllers;
using Mehspot.iOS.Extensions;
using Mehspot.Core.DTO.Subdivision;
using UIKit;
using Mehspot.Core.Builders;

namespace Mehspot.iOS.Views
{
	[Register("SubdivisionPickerCell")]
	public class SubdivisionPickerCell : UITableViewCell, ISubdivisionPickerCell
	{
		private string placeholder;

		public Action<SubdivisionDTO> SetProperty { get; private set; }

		public static readonly NSString Key = new NSString("SubdivisionPickerCell");
		public static readonly UINib Nib;

		static SubdivisionPickerCell()
		{
			Nib = UINib.FromName("SubdivisionPickerCell", NSBundle.MainBundle);
		}

		protected SubdivisionPickerCell(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		[Outlet]
		UIButton SelectValueButton { get; set; }

		[Outlet]
		UILabel FieldLabel { get; set; }

		public string FieldName { get; set; }

		List<SubdivisionDTO> subdivisions;
		public List<SubdivisionDTO> Subdivisions
		{
			get
			{
				return subdivisions;
			}

			set
			{
				subdivisions = value;
                var dto = value?
                    .FirstOrDefault(a =>
                                    a.Id == this.SelectedSubdivisionId
                                    && (this.SelectedSubdivisionOptionId == null ||
                                        a.OptionId == SelectedSubdivisionOptionId));
                this.SetSelectValueButtonTitle(dto);
			}
		}

		public int? SelectedSubdivisionId { get; set; }
		public int? SelectedSubdivisionOptionId { get; private set; }

        public string ZipCode { get; set; }

        public bool IsReadOnly
        {
            get
            {
                return this.SelectValueButton.Enabled;
            }
            set
            {
                this.SelectValueButton.Enabled = !value;
            }
        }


        public static SubdivisionPickerCell Create(int? selectedId, int? optionId, Action<SubdivisionDTO> setProperty, string label, List<SubdivisionDTO> list, string zipCode, bool isReadOnly = false)
		{
			var cell = (SubdivisionPickerCell)Nib.Instantiate(null, null)[0];
			cell.placeholder = label;
			cell.IsReadOnly = isReadOnly;
			cell.FieldLabel.Text = label;
			cell.ZipCode = zipCode;
			cell.SelectedSubdivisionId = selectedId;
            cell.SelectedSubdivisionOptionId = optionId;
			cell.SetProperty = setProperty;
			cell.Subdivisions = list;
			return cell;
		}


		[Action("SelectValueButton_TouchUpInside:")]
		async void SelectValueButton_TouchUpInside(UIButton sender)
		{
			var cell = (SubdivisionPickerCell)(sender).Superview.Superview;
			var controller = cell.GetViewController();

			var subdivisionsListController = new SubdivisionsListController();
			subdivisionsListController.ZipCode = this.ZipCode;
			subdivisionsListController.Subdivisions = this.Subdivisions;
			subdivisionsListController.SelectedSubdivisionId = this.SelectedSubdivisionId;
            subdivisionsListController.SelectedSubdivisionOptionId = this.SelectedSubdivisionOptionId;
			subdivisionsListController.OnDismissed += (dto) =>
			{
				cell.SetProperty(dto);
				cell.SetSelectValueButtonTitle(dto);
				cell.SelectedSubdivisionId = dto.Id;
                cell.SelectedSubdivisionOptionId = dto.OptionId;
			};

			await controller.PresentViewControllerAsync(subdivisionsListController, true);
		}

		void SetSelectValueButtonTitle(SubdivisionDTO selectedSubdivision)
		{
			SelectValueButton.SetTitle(selectedSubdivision?.DisplayName ?? this.placeholder, UIControlState.Normal);
		}

		void ReleaseDesignerOutlets()
		{
			if (SelectValueButton != null)
			{
				SelectValueButton.Dispose();
				SelectValueButton = null;
			}

			if (FieldLabel != null)
			{
				FieldLabel.Dispose();
				FieldLabel = null;
			}
		}
	}
}
