using Foundation;
using System;
using UIKit;
using Mehspot.Core.DTO;
using Mehspot.Core;
using SDWebImage;
using CoreGraphics;
using Mehspot.Core.Services;
using Mehspot.Core.Contracts.ViewControllers;
using Mehspot.Core.Contracts.Wrappers;
using Mehspot.Core.Models;
using Mehspot.iOS.Wrappers;

namespace Mehspot.iOS
{
	public partial class MessageBoardViewController : UIViewController, IUITableViewDataSource, IUITableViewDelegate, IMessageBoardViewController
	{
		private MessageBoardModel model;
		private volatile bool goToMessagesWhenAppear;

		private string SelectedUserId;
		private string SelectedUserName;
		private string SelectedUserImagePath;
		private UIRefreshControl refreshControl;

		public string Filter
		{
			get
			{
				return this.SearchBar.Text;
			}
		}

		public IViewHelper ViewHelper { get; private set; }


		public MessageBoardViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			this.ViewHelper = new ViewHelper(this.View);
			model = new MessageBoardModel(new MessagesService(MehspotAppContext.Instance.DataStorage), this);
			model.LoadingStart += Model_LoadingStart;
			model.LoadingEnd += Model_LoadingEnd;

			MessageBoardTable.RegisterNibForCellReuse(MessageBoardCell.Nib, MessageBoardCell.Key);
			MessageBoardTable.WeakDataSource = this;
			MessageBoardTable.Delegate = this;

			this.SearchBar.OnEditingStarted += SearchBar_OnEditingStarted;
			this.SearchBar.OnEditingStopped += SearchBar_OnEditingStopped;
			this.SearchBar.CancelButtonClicked += SearchBar_CancelButtonClicked;
			this.SearchBar.SearchButtonClicked += SearchBar_SearchButtonClicked;

			this.refreshControl = new UIRefreshControl();
			this.refreshControl.ValueChanged += RefreshControl_ValueChanged;
			this.MessageBoardTable.AddSubview(refreshControl);
		}

		public override async void ViewDidAppear(bool animated)
		{
			await model.LoadMessageBoardAsync(model.Items == null);
			AppDelegate.CheckPushNotificationsPermissions();
			if (goToMessagesWhenAppear)
			{
				goToMessagesWhenAppear = false;
                ShowMessagesFromUser(this.SelectedUserId, this.SelectedUserName);
			}
		}

		void Model_LoadingStart()
		{
			this.refreshControl.BeginRefreshing();
			this.MessageBoardTable.SetContentOffset(new CGPoint(0, -this.refreshControl.Frame.Size.Height), true);
		}

		void Model_LoadingEnd()
		{
			this.MessageBoardTable.SetContentOffset(CGPoint.Empty, true);
			this.refreshControl.EndRefreshing();
		}

		public void DisplayMessageBoard()
		{
			MessageBoardTable.ReloadData();
		}

		public nint RowsInSection(UITableView tableView, nint section)
		{
			return model.Items?.Length ?? 0;
		}

		public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var item = model.Items[indexPath.Row];

			var cell = MessageBoardTable.DequeueReusableCell(MessageBoardCell.Key, indexPath);
			ConfigureCell(cell as MessageBoardCell, item);
			return cell;
		}

		[Export("tableView:didSelectRowAtIndexPath:")]
		public void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			var dto = model.Items[indexPath.Row].WithUser;
			this.SelectedUserImagePath = model.Items[indexPath.Row].WithUser.ProfilePicturePath;
			this.SelectedUserId = dto.Id;
			this.SelectedUserName = dto.UserName;
			ShowMessagesFromUser(this.SelectedUserId, this.SelectedUserName);
			tableView.DeselectRow(indexPath, true);
		}

		public void ShowMessagesFromUser(string userId, string userName)
		{
			this.SelectedUserId = userId;
			this.SelectedUserName = userName;
			if (this.IsViewLoaded)
			{
				var storyboard = UIStoryboard.FromName("Contact", null);
				var controller = (MessagingViewController)storyboard.InstantiateViewController("MessagingViewController");

				controller.ToUserName = this.SelectedUserName;
				controller.ToUserId = this.SelectedUserId;
				controller.ProfilePicturePath = this.SelectedUserImagePath;
				controller.ParentController = this;

				this.NavigationController?.ShowDetailViewController(controller, this);
			}
			else
			{
				goToMessagesWhenAppear = true;
			}
		}

		public void UpdateApplicationBadge(int value)
		{
			UIApplication.SharedApplication.ApplicationIconBadgeNumber = value;
		}

		private void ConfigureCell(MessageBoardCell cell, MessageBoardItemDto item)
		{
			NSUrl url = null;
			if (!string.IsNullOrEmpty(item.WithUser.ProfilePicturePath))
			{
				url = NSUrl.FromString(item.WithUser.ProfilePicturePath);
			}

			if (url != null)
			{
				cell.ProfilePicture.SetImage(url);
			}
			else
			{
				cell.ProfilePicture.Image = UIImage.FromFile("profile_image");
			}


			cell.UserName.Text = item.WithUser.UserName;
			cell.Message.Text = item.LastMessage;
			cell.CountLabel.Hidden = item.UnreadMessagesCount == 0;
			cell.CountLabel.Text = item.UnreadMessagesCount.ToString();
		}

		public void UpdateMessageBoardCell(MessageBoardItemDto dto, int index)
		{
			if (dto != null)
			{
				InvokeOnMainThread(() =>
				{
					var cell = MessageBoardTable.CellAt(NSIndexPath.FromItemSection(index, 0)) as MessageBoardCell;
					if (cell != null)
					{
						cell.CountLabel.Text = (int.Parse(cell.CountLabel.Text) + 1).ToString();
						cell.Message.Text = dto.LastMessage;
						cell.CountLabel.Hidden = false;
					}
				});
			}
		}

		private async void RefreshControl_ValueChanged(object sender, EventArgs e)
		{
			await model.LoadMessageBoardAsync();
		}

		private void SearchBar_OnEditingStarted(object sender, EventArgs e)
		{
			SearchBar.SetShowsCancelButton(true, true);
		}

		private void SearchBar_OnEditingStopped(object sender, EventArgs e)
		{
			SearchBar.SetShowsCancelButton(false, true);
			SearchBar.ResignFirstResponder();
		}

		private async void SearchBar_CancelButtonClicked(object sender, EventArgs e)
		{
			SearchBar.EndEditing(true);
			await model.LoadMessageBoardAsync();
		}

		private async void SearchBar_SearchButtonClicked(object sender, EventArgs e)
		{
			SearchBar.EndEditing(true);
			await model.LoadMessageBoardAsync();
		}
	}
}
