
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Mehspot.AndroidApp.Wrappers;
using Mehspot.Core;
using Mehspot.Core.Contracts.Wrappers;
using Mehspot.Core.DTO.Badges;
using Mehspot.Core.Models;

namespace Mehspot.AndroidApp.Activities
{
	[Activity(Label = "Write Recommendation")]
	public class WriteReviewActivity : Activity, IWriteReviewController
	{
		private WriteReviewModel model;

		public int BadgeId => Intent.GetIntExtra("badgeId", 0);

		public string UserId => Intent.GetStringExtra("userId");

		public IViewHelper ViewHelper { get; private set; }

		public event Action<BadgeUserRecommendationDTO> OnSave;

		public void NotifySaved(BadgeUserRecommendationDTO data)
		{
			this.OnSave?.Invoke(data);
		}

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.WriteReviewActivity);

			this.ViewHelper = new ActivityHelper(this);
			this.model = new WriteReviewModel(new Mehspot.Core.Services.BadgeService(MehspotAppContext.Instance.DataStorage), this);
			this.FindViewById<Button>(Resource.WriteReviewActivity.SaveButton).Click += Handle_Click; ;
		}

		async void Handle_Click(object sender, EventArgs e)
		{
			await model.SaveAsync(this.FindViewById<EditText>(Resource.WriteReviewActivity.CommentView).Text);

            Intent intent = new Intent(this, typeof(ViewBadgeProfileActivity));
            this.SetResult(Result.Ok, intent);
			this.Finish();
		}
	}
}
