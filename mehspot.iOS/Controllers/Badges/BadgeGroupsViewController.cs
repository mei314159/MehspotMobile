using Foundation;
using System;
using UIKit;
using Mehspot.Core.Contracts.ViewControllers;
using Mehspot.Core.Models;
using Mehspot.Core.Contracts.Wrappers;
using Mehspot.iOS.Wrappers;
using Mehspot.Core.Services;
using Mehspot.Core;
using mehspot.iOS.Controllers.Badges.DataSources;
using Mehspot.iOS.Views;
using Mehspot.iOS;
using Mehspot.Core.Services.Badges;
using Mehspot.iOS.Extensions;
using System.Linq;
using CoreGraphics;
using System.Threading.Tasks;

namespace mehspot.iOS
{
	public partial class BadgeGroupsViewController : UIViewController, IBadgesViewController
	{
		private const string BadgesTutorialShownKey = "BadgesTutorialShown";

		MehOverlay tutorialOverlay;
		private BadgeInfo selectedBadge;
		private BadgesGrouppingViewDataSource DataSource;
		private BadgesModel model;

		public BadgeGroupsViewController(IntPtr handle) : base(handle)
		{
		}

		public IViewHelper ViewHelper { get; private set; }


		public override void ViewDidLoad()
		{

			this.ViewHelper = new ViewHelper(this.View);
			model = new BadgesModel(new BadgeService(MehspotAppContext.Instance.DataStorage), this);
			model.LoadingStart += Model_LoadingStart;
			model.LoadingEnd += Model_LoadingEnd;


			DataSource = new BadgesGrouppingViewDataSource(model);

			DataSource.SearchButtonTouch += SearchButton_TouchUpInside;
			DataSource.BadgeRegisterButtonTouch += BadgeRegisterButton_TouchUpInside;
			var grouppingView = iOS.GrouppingView.Create(GrouppingView.Bounds, DataSource);
			GrouppingView.AddSubview(grouppingView);
		}

		void Model_LoadingStart()
		{
			//this.RefreshControl.BeginRefreshing();
		}

		void Model_LoadingEnd()
		{
			//this.TableView.SetContentOffset(CGPoint.Empty, true);
			//this.RefreshControl.EndRefreshing();
		}

		public override UIStatusBarStyle PreferredStatusBarStyle()
		{
			return UIStatusBarStyle.LightContent;
		}

		public override void ViewWillAppear(bool animated)
		{
			if (!this.NavigationController.NavigationBarHidden)
				this.NavigationController.SetNavigationBarHidden(true, false);
		}

		public override void ViewWillDisappear(bool animated)
		{
			if (this.NavigationController.NavigationBarHidden)
				this.NavigationController.SetNavigationBarHidden(false, true);
		}

		public override void ViewDidAppear(bool animated)
		{
			DataSource.ReloadDataAsync();
		}

		public void DisplayBadges()
		{
			DataSource.RefreshTable();
			if (model.Items != null && !MehspotAppContext.Instance.DataStorage.Get<bool>(BadgesTutorialShownKey))
			{
				Task.Run(async () =>
				{
					await Task.Delay(500);
					InvokeOnMainThread(ShowTutorial);
					MehspotAppContext.Instance.DataStorage.Set(BadgesTutorialShownKey, (object)true);
				});
			}

		}

		public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
		{
			if (segue.Identifier == "GoToSearchFilterSegue")
			{
				var controller = ((SearchBadgeController)segue.DestinationViewController);
				controller.BadgeSummary = model.BadgeHelper.GetBadgeSummary(this.selectedBadge.SearchBadge);
			}
			else if (segue.Identifier == "GoToEditBadgeSegue")
			{
				var controller = ((EditBadgeProfileController)segue.DestinationViewController);
				controller.BadgeIsRegistered = selectedBadge.Badge.IsRegistered;
				controller.BadgeId = selectedBadge.Badge.BadgeId;
				controller.BadgeName = selectedBadge.BadgeName;
			}

			base.PrepareForSegue(segue, sender);
		}

		//async void RefreshControl_ValueChanged(object sender, EventArgs e)
		//{
		//	await model.RefreshAsync(true, true);
		//}

		void SearchButton_TouchUpInside(BadgeItemCell cell)
		{
			this.selectedBadge = cell.BadgeInfo;
			PerformSegue("GoToSearchFilterSegue", this);
		}

		void BadgeRegisterButton_TouchUpInside(BadgeItemCell cell)
		{
			this.selectedBadge = cell.BadgeInfo;
			PerformSegue("GoToEditBadgeSegue", this);
		}

		void ShowTutorial()
		{
			tutorialOverlay?.Hide();
			tutorialOverlay = View.Window
								  .CreateOverlay()
								  .SetBackgroundColor(UIColor.FromRGBA(0, 0, 0, 0.5f));
			var cells = GrouppingView.FindChildViewsOfType(null, typeof(BadgeItemCell)).Cast<BadgeItemCell>().OrderBy(a => a.Frame.Y);
			var badgeItemCell = cells.FirstOrDefault();
			tutorialOverlay.HighlightSquare(badgeItemCell);

			var frame = View.Window.ConvertRectFromView(badgeItemCell.Bounds, badgeItemCell);

			var image = UIImage.FromBundle("round-arrow");

			tutorialOverlay.AddImage(image, new CGRect(20, frame.Y + frame.Height + 15, 40, 40));

			var label = new UILabel();
			label.Text = "Click on the badges that you are interested in to register";
			label.TextColor = UIColor.White;
			label.Frame = new CGRect(80, frame.Y + frame.Height + 25, View.Window.Frame.Width - 90, 50);
			label.Lines = 0;
			label.Font = UIFont.BoldSystemFontOfSize(20f);
			label.LineBreakMode = UILineBreakMode.WordWrap;
			label.AdjustsFontSizeToFitWidth = true;
			tutorialOverlay.AddLabel(label);
			tutorialOverlay.Show();
			tutorialOverlay.AddGestureRecognizer(new UITapGestureRecognizer(this.TutorialOverlayTouched));
		}

		void TutorialOverlayTouched()
		{
			tutorialOverlay.Hide();
			tutorialOverlay.Dispose();
			tutorialOverlay = null;
		}

	}
}