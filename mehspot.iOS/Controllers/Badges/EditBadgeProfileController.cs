using System;
using UIKit;
using Mehspot.Core.Contracts.Wrappers;
using Mehspot.iOS.Wrappers;
using Mehspot.iOS.Extensions;
using CoreGraphics;
using Mehspot.Core;
using Mehspot.Core.Services;
using Mehspot.Core.Contracts.ViewControllers;
using Foundation;
using Mehspot.iOS.Core.Builders;

namespace Mehspot.iOS
{
	public partial class EditBadgeProfileController : UITableViewController, IEditBadgeProfileController
	{
		private EditBadgeProfileModel<UITableViewCell> model;

		public EditBadgeProfileController(IntPtr handle) : base(handle)
		{
		}

		public IViewHelper ViewHelper { get; set; }
		public int BadgeId { get; set; }
		public string BadgeName { get; set; }
		public bool BadgeIsRegistered { get; set; }
		public bool RedirectToSearchResults { get; set; }
		public bool SaveButtonEnabled
		{
			get
			{
				return this.SaveButton.Enabled;
			}
			set
			{
				this.SaveButton.Enabled = value;
			}
		}

		public string WindowTitle
		{
			get
			{
				return this.NavBar.Title;
			}
			set
			{
				this.NavBar.Title = value;
			}
		}

		public override void ViewDidLoad()
		{

			ViewHelper = new ViewHelper(this.View);

			model = new EditBadgeProfileModel<UITableViewCell>(
				this,
				new BadgeService(MehspotAppContext.Instance.DataStorage),
				new SubdivisionService(MehspotAppContext.Instance.DataStorage),
				new IosCellBuilder());
			model.LoadingStarted += Model_LoadingStarted;
			model.LoadingEnded += Model_LoadingEnded;
			TableView.AddGestureRecognizer(new UITapGestureRecognizer(this.HideKeyboard));
			//TableView.RowHeight = UITableView.AutomaticDimension;
			//TableView.EstimatedRowHeight = 44;
			this.RefreshControl.ValueChanged += RefreshControl_ValueChanged;
		}

		public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
		{
			if (segue.Identifier == "UnwindToSearchResults")
			{
				var controller = (SearchResultsViewController)segue.DestinationViewController;
				controller.ReqiredBadgeWasRegistered();
			}

			base.PrepareForSegue(segue, sender);
		}

		public override async void ViewDidAppear(bool animated)
		{
			await model.LoadAsync();
		}

		private async void RefreshControl_ValueChanged(object sender, EventArgs e)
		{
			await model.ReloadAsync();
		}

		async partial void SaveButtonTouched(UIBarButtonItem sender)
		{
			await model.SaveAsync();
		}

		public void ReloadData()
		{
			TableView.ReloadData();
		}

		public void HideKeyboard()
		{
			ViewExtensions.HideKeyboard(this);
		}

		public void GoToSearchResults()
		{
			PerformSegue("UnwindToSearchResults", this);
		}

		public void Dismiss()
		{
			this.NavigationController.PopViewController(true);
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			return model.Cells[indexPath.Row];
		}

		public override nint RowsInSection(UITableView tableView, nint section)
		{
			return model.Cells.Count;
		}

		public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			var cell = model.Cells[indexPath.Row];
			return cell.Frame.Height;
		}

		private void Model_LoadingStarted()
		{
			TableView.UserInteractionEnabled = this.SaveButtonEnabled = false;
			RefreshControl.BeginRefreshing();
			TableView.SetContentOffset(new CGPoint(0, -this.RefreshControl.Frame.Size.Height), true);
		}

		private void Model_LoadingEnded()
		{
			TableView.UserInteractionEnabled = this.SaveButtonEnabled = true;
			TableView.SetContentOffset(CGPoint.Empty, true);
			RefreshControl.EndRefreshing();
		}
	}
}