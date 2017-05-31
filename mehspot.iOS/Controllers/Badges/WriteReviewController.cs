using System;
using Mehspot.iOS.Wrappers;
using Mehspot.Core;
using Mehspot.Core.DTO.Badges;
using Mehspot.Core.Services;
using UIKit;
using Mehspot.Core.Contracts.Wrappers;
using Mehspot.Core.Models;

namespace Mehspot.iOS
{
	public partial class WriteReviewController : UIViewController, IWriteReviewController
	{
		private WriteReviewModel model;
		public IViewHelper ViewHelper { get; private set; }

		public int BadgeId { get; set; }
		public string UserId { get; set; }

		public event Action<BadgeUserRecommendationDTO> OnSave;

		public WriteReviewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			this.ViewHelper = new ViewHelper(this.View);
			this.model = new WriteReviewModel(new BadgeService(MehspotAppContext.Instance.DataStorage), this);

			base.ViewDidLoad();
		}

		partial void CancelButtonTouched(UIBarButtonItem sender)
		{
			this.DismissViewController(true, null);
		}

		async partial void SendButtonTouched(UIBarButtonItem sender)
		{
			await model.SaveAsync(this.CommentView.Text);
			this.DismissViewController(true, null);
		}

		public void NotifySaved(BadgeUserRecommendationDTO data)
		{
			this.OnSave?.Invoke(data);
		}
	}
}