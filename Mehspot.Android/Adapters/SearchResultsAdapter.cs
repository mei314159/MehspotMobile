using Android.App;
using Android.Views;
using Android.Widget;
using Mehspot.AndroidApp.Views;
using Mehspot.Models.ViewModels;
using System.Collections.Generic;
using System;
using Mehspot.Core;
using Android.Content;

namespace Mehspot.AndroidApp.Adapters
{
	public class SearchResultsAdapter : BaseAdapter<ISearchResultDTO>
	{
		public event Action<ISearchResultDTO> MessageButtonClicked;
		public event Action<ISearchResultDTO> ViewProfileButtonClicked;
		public event Action<ISearchResultDTO, SearchResultItem> Clicked;

		private readonly Activity context;
		private readonly SearchResultsModel model;

		public SearchResultsAdapter(Activity context, SearchResultsModel model)
		{
			this.model = model;
			this.context = context;
		}
		public override long GetItemId(int position)
		{
			return position;
		}
		public override ISearchResultDTO this[int position]
		{
			get { return model.Items[position]; }
		}

		public override int Count
		{
			get { return model.GetRowsCount(); }
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			View cell = null;
			if (!this.model.RegisterButtonVisible || position + 1 < model.GetRowsCount())
			{
				var view = convertView as SearchResultItem; // re-use an existing view, if one is available
				if (view == null) // otherwise create a new one
				{
					view = new SearchResultItem(context);
					view.Clicked += (sender, e) => Clicked?.Invoke(sender, e);
					view.ViewProfileButtonClicked += (arg1) => ViewProfileButtonClicked?.Invoke(arg1);
					view.MessageButtonClicked += (arg1) => MessageButtonClicked?.Invoke(arg1);
				}

				view.Init(model.Items[position]);
				cell = view;

			}
			else if (this.model.RegisterButtonVisible)
			{
				var searchLimitCell = new SearchLimitCell(context, model.controller.BadgeSummary.RequiredBadgeName, model.controller.BadgeSummary.BadgeName);
				searchLimitCell.OnRegisterButtonTouched += OnRegisterButtonTouched;
				cell = searchLimitCell;
			}

			return cell;


		}

		void OnRegisterButtonTouched()
		{
			var target = new Intent(this.context, typeof(EditBadgeProfileActivity));
			if (model.controller.BadgeSummary.RequiredBadgeId.HasValue)
			{
				target.PutExtra("badgeId", model.controller.BadgeSummary.RequiredBadgeId.Value);
				target.PutExtra("badgeName", model.controller.BadgeSummary.RequiredBadgeName);
				target.PutExtra("badgeIsRegistered", model.controller.BadgeSummary.RequiredBadgeIsRegistered);
			}
			else
			{
				target.PutExtra("badgeId", model.controller.BadgeSummary.BadgeId);
				target.PutExtra("badgeName", model.controller.BadgeSummary.BadgeName);
				target.PutExtra("badgeIsRegistered", false);
			}

			target.PutExtra("redirectToSearchResults", true);

			this.context.StartActivity(target);
		}
	}
}
