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

namespace mehspot.iOS
{
	public partial class BadgeGroupsViewController : UIViewController, IBadgesViewController
	{
		private BadgeInfo selectedBadge;
		private BadgesGrouppingViewDataSource DataSource;
		private BadgesModel model;

		public BadgeGroupsViewController(IntPtr handle) : base(handle)
		{
		}

		public IViewHelper ViewHelper { get; private set; }


		public override async void ViewDidLoad()
		{

			this.ViewHelper = new ViewHelper(this.View);
			model = new BadgesModel(new BadgeService(MehspotAppContext.Instance.DataStorage), this);
			model.LoadingStart += Model_LoadingStart;
			model.LoadingEnd += Model_LoadingEnd;


			DataSource = new BadgesGrouppingViewDataSource(model)
			{
				CurrentKey = MehspotAppContext.Instance.DataStorage.PreferredBadgeGroup
			};
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
	}
}