using Android.App;
using Android.Views;
using Android.Widget;
using Mehspot.AndroidApp.Views;
using Mehspot.Models.ViewModels;
using System.Collections.Generic;
using System;

namespace Mehspot.AndroidApp.Adapters
{
	public class SearchResultsAdapter : BaseAdapter<ISearchResultDTO>
	{
		public event Action<ISearchResultDTO> MessageButtonClicked;
		public event Action<ISearchResultDTO> ViewProfileButtonClicked;
		public event Action<ISearchResultDTO, SearchResultItem> Clicked;

		public readonly List<ISearchResultDTO> Items;
		Activity context;
		public SearchResultsAdapter(Activity context)
		{
			this.context = context;
			this.Items = new List<ISearchResultDTO>();
		}
		public override long GetItemId(int position)
		{
			return position;
		}
		public override ISearchResultDTO this[int position]
		{
			get { return Items[position]; }
		}
		public override int Count
		{
			get { return Items.Count; }
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			var view = convertView as SearchResultItem; // re-use an existing view, if one is available
			if (view == null) // otherwise create a new one
			{
				view = new SearchResultItem(context);
				view.Clicked += (sender, e) => Clicked?.Invoke(sender, e);
				view.ViewProfileButtonClicked += (arg1) => ViewProfileButtonClicked?.Invoke(arg1);
				view.MessageButtonClicked += (arg1) => MessageButtonClicked?.Invoke(arg1);
			}

			view.Init(Items[position]);

			return view;
		}
	}
}
