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

namespace Mehspot.AndroidApp.Adapters
{

	public class SubdivisionsListAdapter : BaseAdapter<SubdivisionDTO>
	{
		public event Action<SubdivisionDTO> Clicked;

		private readonly Activity context;
		private readonly List<SubdivisionDTO> items;

		public SubdivisionsListAdapter(Activity context, List<SubdivisionDTO> items)
		{
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
			var view = convertView as TaggedTextView<SubdivisionDTO>; // re-use an existing view, if one is available
			if (view == null) // otherwise create a new one
			{
				view = new TaggedTextView<SubdivisionDTO>(context);
				view.Click += (sender, e) =>
				{
					//view.SetBackgroundResource(Resource.Color.dark_green);
					//view.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(context, Resource.Color.white)));
					Clicked?.Invoke(view.Data);
				};
			}

			view.Data = items[position];
			view.Text = view.Data.DisplayName;

			return view;

		}
	}
}
