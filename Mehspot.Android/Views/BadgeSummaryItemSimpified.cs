using System;
using Android.Content;
using Android.Views;
using Android.Widget;
using Mehspot.Core;
using Mehspot.Core.DTO;

namespace Mehspot.AndroidApp.Resources.layout
{
	public class BadgeSummaryItemSimpified : RelativeLayout
	{
		private readonly BadgeSummaryBaseDTO dto;

		public TextView BadgeName => (TextView)FindViewById(Resource.BadgeSummarySimplified.BadgeName);
		public event Action<BadgeSummaryBaseDTO, BadgeSummaryItemSimpified> Clicked;

		public BadgeSummaryItemSimpified(Context context, BadgeSummaryBaseDTO dto) : base(context)
		{
			this.dto = dto;
			LayoutInflater inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
			inflater.Inflate(Resource.Layout.BadgeSummaryItemSimplified, this);

			BadgeName.Text = MehspotResources.ResourceManager.GetString(dto.BadgeName);

			var identifier = Resources.GetIdentifier(dto.BadgeName.ToLower(), "drawable", context.PackageName);
			using (var Picture = FindViewById<ImageView>(Resource.BadgeSummarySimplified.Picture))
			{
				Picture.SetImageResource(identifier);
			}

			this.Click += Handle_Click;
		}

		void Handle_Click(object sender, EventArgs e)
		{
			this.Clicked?.Invoke(this.dto, this);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				FindViewById<ImageView>(Resource.BadgeSummarySimplified.Picture).Dispose();
			}

			base.Dispose(disposing);
		}
	}
}
