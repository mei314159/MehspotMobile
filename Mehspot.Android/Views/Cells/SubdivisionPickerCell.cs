using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Views;
using Android.Widget;
using Mehspot.AndroidApp.Activities;
using Mehspot.Core.Builders;
using Mehspot.Core.DTO.Subdivision;

namespace Mehspot.AndroidApp
{
	public class SubdivisionPickerCell : LinearLayout, ISubdivisionPickerCell
	{
		private string placeholder;

		public Action<SubdivisionDTO> SetProperty { get; private set; }

		public SubdivisionPickerCell(Context context, int? selectedId, int? optionId, Action<SubdivisionDTO> setProperty, string label, List<SubdivisionDTO> list, string zipCode, bool isReadOnly = false) :
											base(context)
		{
			LayoutInflater inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
			inflater.Inflate(Resource.Layout.SubdivisionPickerCell, this);

			this.placeholder = label;
			this.IsReadOnly = isReadOnly;
			this.FieldLabel.Text = label;
			this.ZipCode = zipCode;
			this.SelectedSubdivisionId = selectedId;
            this.SelectedSubdivisionOptionId = optionId;
			this.SetProperty = setProperty;
			this.Subdivisions = list;

			this.SelectValueButton.Click += SelectValueButton_Click;
		}

		public string FieldName { get; set; }

        public int? SelectedSubdivisionId { get; private set; }
		public int? SelectedSubdivisionOptionId { get; private set; }

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
                var dto = value?.FirstOrDefault(a =>
                     a.Id == this.SelectedSubdivisionId &&
                     (this.SelectedSubdivisionId == null ||
                      a.OptionId == this.SelectedSubdivisionOptionId));
                this.SetSelectValueButtonTitle(dto);
            }
        }

        public string ZipCode { get; set; }

        public TextView SelectValueButton => this.FindViewById<TextView>(Resource.SubdivisionPickerCell.SelectValueButton);

        public TextView FieldLabel => this.FindViewById<TextView>(Resource.SubdivisionPickerCell.FieldLabel);


        void SelectValueButton_Click(object sender, EventArgs e)
		{
			var target = new Intent(Context, typeof(SubdivisionsListActivity));
			target.PutExtra("zipCode", this.ZipCode);
			target.PutExtra("subdivisions", this.Subdivisions);
			target.PutExtra<int?>("selectedSubdivisionId", this.SelectedSubdivisionId);
            target.PutExtra<int?>("selectedSubdivisionOptionId", this.SelectedSubdivisionOptionId);
			target.PutExtra<Action<SubdivisionDTO>>("onDismissed", (dto) =>
						{
							this.SetProperty(dto);
							this.SetSelectValueButtonTitle(dto);
							this.SelectedSubdivisionId = dto?.Id;
						});

			this.Context.StartActivity(target);
		}

		void SetSelectValueButtonTitle(SubdivisionDTO selectedSubdivision)
		{
			SelectValueButton.Text = selectedSubdivision?.DisplayName ?? this.placeholder;
		}
	}
}
