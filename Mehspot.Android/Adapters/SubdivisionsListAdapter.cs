using Android.App;
using Android.Views;
using Android.Widget;
using Mehspot.AndroidApp.Views;
using Mehspot.Models.ViewModels;
using System;
using Mehspot.Core;
using Android.Content;
using Mehspot.Core.DTO.Subdivision;
using System.Collections.Generic;
using Android.Support.V4.Content;
using Mehspot.AndroidApp.Resources.layout;

namespace Mehspot.AndroidApp.Adapters
{

	public class SubdivisionsListAdapter : BaseAdapter<SubdivisionDTO>
	{
		private readonly Activity context;
		private readonly List<SubdivisionDTO> items;
		private readonly SubdivisionDTO initiallySelectedItem;

		public SubdivisionsListAdapter(Activity context, List<SubdivisionDTO> items, SubdivisionDTO selectedItem)
		{
			this.initiallySelectedItem = selectedItem;
			this.items = items;
			this.context = context;
		}
		public override long GetItemId(int position)
		{
			return position;
		}

		public override SubdivisionDTO this[int position]
		{
			get { return items[position]; }
		}

		public override int Count
		{
			get { return items.Count; }
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			var subdivisionDTO = items[position];
			var view = convertView as SubdivisionsListItem; // re-use an existing view, if one is available
			if (view == null) // otherwise create a new one
			{
				view = new SubdivisionsListItem(context);
			}

			view.SubdivisionDTO = subdivisionDTO;
			view.TextLabel.Text = view.SubdivisionDTO.DisplayName;

			return view;

		}
	}
}
