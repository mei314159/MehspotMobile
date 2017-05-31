using Android.App;
using Android.Views;
using Android.Widget;
using Mehspot.AndroidApp.Views;
using Mehspot.Models.ViewModels;
using System.Collections.Generic;
using System;
using Mehspot.Core.Models;

namespace Mehspot.AndroidApp.Adapters
{
	public class BadgeProfileListAdapter : BaseAdapter<View>
	{
		private readonly Activity context;
		private readonly ViewBadgeProfileModel<View> model;

		public BadgeProfileListAdapter(Activity context, ViewBadgeProfileModel<View> model)
		{
			this.model = model;
			this.context = context;
		}

		public override long GetItemId(int position)
		{
			return position;
		}

		public override View this[int position]
		{
			get { return model.Cells[position]; }
		}

		public override int Count
		{
			get { return model.Cells.Count; }
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			return model.Cells[position];
		}
	}
}
