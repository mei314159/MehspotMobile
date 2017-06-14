using System;
using Android.Content;
using Android.Views;
using Android.Widget;
using Mehspot.Core.DTO;
using Mehspot.Core.DTO.Subdivision;

namespace Mehspot.AndroidApp.Resources.layout
{

	public class SubdivisionsListItem : RelativeLayout
	{
		public SubdivisionDTO SubdivisionDTO { get; set; }

		public SubdivisionsListItem(Context context) : base(context)
		{
			LayoutInflater inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
			inflater.Inflate(Resource.Layout.SubdivisionsListItem, this);
		}

		public TextView TextLabel => (TextView)FindViewById(Resource.SubdivisionsListItem.TextLabel);
	}
}
