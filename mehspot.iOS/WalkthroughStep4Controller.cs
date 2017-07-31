using Foundation;
using System;
using UIKit;
using mehspot.iOS.Views.Cell;
using Mehspot.Core.DTO;

namespace mehspot.iOS
{
	public delegate void WalkthroughStep4Delegate(BadgeGroup group);
	public partial class WalkthroughStep4Controller : UIViewController, IUITableViewDelegate, IUITableViewDataSource
	{
		private BadgeCategoryCell[] Cells;

		public event WalkthroughStep4Delegate OnContinue;

		public WalkthroughStep4Controller(IntPtr handle) : base(handle)
		{
		}

		public BadgeGroup SelectedGroup { get; private set; }

		public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			return Cells[indexPath.Row];
		}

		public nint RowsInSection(UITableView tableView, nint section)
		{
			return 3;
		}

		[Export("tableView:didSelectRowAtIndexPath:")]
		public void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			this.SelectedGroup = Cells[indexPath.Row].BadgeGroup;
			this.ContinueButton.Hidden = false; ;
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			Cells = new[]
			 {
				BadgeCategoryCell.Create(UIColor.FromRGB(251, 114, 0), BadgeGroup.Friends),
				BadgeCategoryCell.Create(UIColor.FromRGB(3, 155, 67), BadgeGroup.Helpers),
				BadgeCategoryCell.Create(UIColor.FromRGB(13, 109, 182), BadgeGroup.Jobs)
			 };

			TableView.DataSource = this;
			TableView.Delegate = this;
		}

		partial void ContinueButtonTouched(UIButton sender)
		{
			OnContinue?.Invoke(this.SelectedGroup);
		}
	}
}