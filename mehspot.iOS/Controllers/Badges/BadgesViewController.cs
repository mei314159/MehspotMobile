using Foundation;
using System;
using UIKit;
using Mehspot.Core.DTO;
using Mehspot.Core;
using Mehspot.iOS.Views;
using CoreGraphics;
using Mehspot.Core.Services;
using Mehspot.iOS.Extensions;
using Mehspot.Core.Contracts.ViewControllers;
using Mehspot.iOS.Wrappers;
using Mehspot.Core.Models;
using Mehspot.Core.Contracts.Wrappers;
using System.Linq;
using System.Threading.Tasks;

namespace Mehspot.iOS
{
	public partial class BadgesViewController : UITableViewController, IBadgesViewController
	{
		private const string BadgesTutorialShownKey = "BadgesTutorialShown";
		MehOverlay tutorialOverlay;
		private BadgesModel model;

		public BadgesViewController(IntPtr handle) : base(handle)
		{
		}

		public IViewHelper ViewHelper { get; private set; }


		public override void ViewDidLoad()
		{
			this.ViewHelper = new ViewHelper(this.View);
			model = new BadgesModel(new BadgeService(MehspotAppContext.Instance.DataStorage), this);
			model.LoadingStart += Model_LoadingStart;
			model.LoadingEnd += Model_LoadingEnd;
			TableView.RegisterNibForCellReuse(BadgeItemCell.Nib, BadgeItemCell.Key);
			this.RefreshControl.ValueChanged += RefreshControl_ValueChanged;
		}

		void Model_LoadingStart()
		{
			this.RefreshControl.BeginRefreshing();
			this.TableView.SetContentOffset(new CGPoint(0, -this.RefreshControl.Frame.Size.Height), true);
		}

		void Model_LoadingEnd()
		{
			this.TableView.SetContentOffset(CGPoint.Empty, true);
			this.RefreshControl.EndRefreshing();
		}

		public override async void ViewDidAppear(bool animated)
		{
			await model.RefreshAsync(model.Items == null, false);

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
			var selectedBadge = model.SelectedBadge;
			if (segue.Identifier == "GoToSearchFilterSegue")
			{
				var controller = ((SearchBadgeController)segue.DestinationViewController);
				controller.BadgeSummary = selectedBadge;
			}
			else if (segue.Identifier == "GoToEditBadgeSegue")
			{
				var controller = ((EditBadgeProfileController)segue.DestinationViewController);
				controller.BadgeId = selectedBadge.BadgeId;
				controller.BadgeName = selectedBadge.BadgeName;
				controller.BadgeIsRegistered = selectedBadge.IsRegistered;
			}

			base.PrepareForSegue(segue, sender);
		}

		async void RefreshControl_ValueChanged(object sender, EventArgs e)
		{
			await model.RefreshAsync(true, true);
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var item = model.Items[indexPath.Row];
			var cell = tableView.DequeueReusableCell(BadgeItemCell.Key, indexPath) as BadgeItemCell;
			ConfigureCell(cell, item);
			return cell;
		}

		public override nint RowsInSection(UITableView tableView, nint section)
		{
			return model.Items?.Length ?? 0;
		}

		private void ConfigureCell(BadgeItemCell cell, BadgeSummaryDTO badge)
		{
			cell.Configure(badge);
			cell.SearchButtonTouch = SearchButton_TouchUpInside;
			cell.BadgeRegisterButtonTouch = BadgeRegisterButton_TouchUpInside;
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			model.SelectRow(indexPath.Row);
			tableView.ReloadRows(new[] { indexPath }, UITableViewRowAnimation.Fade);
		}

		public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			if (model.IsRowExpanded(indexPath.Row))
			{
				return 152;
			}
			else
			{
				return 70;
			}
		}

		void SearchButton_TouchUpInside(UIButton button)
		{
			var cell = (BadgeItemCell)button.FindSuperviewOfType(this.View, typeof(BadgeItemCell));
			var indexPath = this.TableView.IndexPathForCell(cell);
			this.model.SelectRow(indexPath.Row);
			PerformSegue("GoToSearchFilterSegue", this);
		}

		void BadgeRegisterButton_TouchUpInside(UIButton button)
		{
			var cell = (BadgeItemCell)button.FindSuperviewOfType(this.View, typeof(BadgeItemCell));
			var indexPath = this.TableView.IndexPathForCell(cell);
			this.model.SelectRow(indexPath.Row);
			PerformSegue("GoToEditBadgeSegue", this);
		}

		public void DisplayBadges()
		{
			TableView.ReloadData();
		}

		void ShowTutorial()
		{
			tutorialOverlay?.Hide();
			tutorialOverlay = View.Window
								  .CreateOverlay()
								  .SetBackgroundColor(UIColor.FromRGBA(0, 0, 0, 0.5f));
			var cells = TableView.FindChildViewsOfType(null, typeof(BadgeItemCell)).Cast<BadgeItemCell>().OrderBy(a => a.Frame.Y);
			var badgeItemCell = cells.ElementAt(3);
			tutorialOverlay.HighlightSquare(badgeItemCell);

			var frame = View.Window.ConvertRectFromView(badgeItemCell.Bounds, badgeItemCell);

			var image = UIImage.FromBundle("arrow-up");

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