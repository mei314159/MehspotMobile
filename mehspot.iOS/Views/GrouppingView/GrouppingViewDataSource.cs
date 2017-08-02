using Foundation;
using System;
using UIKit;
using System.Collections.Generic;
using mehspot.iOS.Views;
using System.Linq;
using CoreGraphics;

namespace mehspot.iOS
{
	public delegate void GrouppingViewDataSourceItemChanged<TGroupView>(TGroupView groupView);
	public abstract class GrouppingViewDataSource<TKey, T, TGroupView, TTableView> : UICollectionViewDataSource, IUICollectionViewDelegate,
	IUITableViewDataSource,
	IUITableViewDelegate
		where TGroupView : UICollectionViewCell, IGrouppingViewCell<TKey>
		where TTableView : UITableViewCell
	{
		private UICollectionView collectionView;
		private UITableView tableView;
		private readonly CGAffineTransform transform = CGAffineTransform.MakeScale(0.8f, 0.8f);
		private readonly CGAffineTransform identityTransform = CGAffineTransform.MakeIdentity();
		private readonly nfloat animationSpeed = 0.2f;
		protected bool IsFirstTimeTransform { get; set; }

		public GrouppingViewDataSource()
		{
			this.IsFirstTimeTransform = true;
		}

		public GrouppingViewDataSource(IReadOnlyDictionary<TKey, IReadOnlyCollection<T>> items) : this()
		{
			this.Groups = items;
		}

		public event GrouppingViewDataSourceItemChanged<TGroupView> OnGroupChanged;

		public abstract UINib GroupViewNib { get; }
		public abstract string GroupViewKey { get; }
		public abstract UINib TableViewNib { get; }
		public abstract string TableViewKey { get; }

		public UICollectionView CollectionView
		{
			get
			{
				return collectionView;
			}
			set
			{
				value.RegisterNibForCell(GroupViewNib, GroupViewKey);
				collectionView = value;
			}
		}

		public virtual UITableView TableView
		{
			get
			{
				return tableView;
			}
			set
			{
				value.RegisterNibForCellReuse(TableViewNib, TableViewKey);
				tableView = value;
			}
		}

		private NSIndexPath SelectedItem;
		public TKey CurrentKey { get; set; }
		public IReadOnlyDictionary<TKey, IReadOnlyCollection<T>> Groups { get; protected set; }

		public abstract void InflateView(TGroupView cell, TKey groupKey);
		public abstract void InflateView(TTableView cell, TKey groupKey, T item);
		public abstract TGroupView CreateGroupView();
		public abstract TTableView CreateTableView();

		public override nint GetItemsCount(UICollectionView collectionView, nint section)
		{
			return Groups != null ? 10000 : 0;
		}

		public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = CollectionView.DequeueReusableCell(GroupViewKey, indexPath) as TGroupView ?? this.CreateGroupView();

			var index = indexPath.Row % Groups.Count;
			var group = Groups.Keys.ElementAt(index);
			this.InflateView(cell, group);

			var middle = (int)GetItemsCount(collectionView, 0) / 2;
			var groupKeyIndex = CurrentKey != null ? Groups.Keys.Select((a, i) => new { a, i }).First(a => Equals(a.a, CurrentKey)).i : 0;
			int estimatedRow = (middle - middle % Groups.Count) + groupKeyIndex;
			int initialElementPath = SelectedItem?.Row ?? estimatedRow;

			if (indexPath.Row == initialElementPath && IsFirstTimeTransform)
			{
				this.SelectedItem = indexPath;
				cell.Transform = identityTransform;
				this.CurrentKey = cell.GroupKey;

				this.OnGroupChanged?.Invoke(cell);
				IsFirstTimeTransform = false;
			}
			else
			{
				cell.Transform = transform; // the new cell will always be transform and without animation
			}

			if (indexPath.Row == 0 && IsFirstTimeTransform)
			{ // make a bool and set YES initially, this check will prevent fist load transform
				collectionView.ScrollToItem(NSIndexPath.FromItemSection(initialElementPath, 0), UICollectionViewScrollPosition.CenteredHorizontally, false);
			}

			return cell;
		}

		[Export("scrollViewWillEndDragging:withVelocity:targetContentOffset:")]
		public virtual void WillEndDragging(UIScrollView scrollView, CGPoint velocity, ref CGPoint targetContentOffset)
		{
			if (scrollView != this.collectionView)
			{
				return;
			}

			var pageWidth = 130 + 30; // width + space

			var currentOffset = scrollView.ContentOffset.X;
			var targetOffset = targetContentOffset.X;
			double newTargetOffset = 0;

			if (targetOffset > currentOffset)
				newTargetOffset = Math.Ceiling(currentOffset / pageWidth) * pageWidth;
			else
				newTargetOffset = Math.Floor(currentOffset / pageWidth) * pageWidth;

			if (newTargetOffset < 0)
				newTargetOffset = 0;
			else if (newTargetOffset > scrollView.ContentSize.Width)
				newTargetOffset = scrollView.ContentSize.Width;

			targetContentOffset.X = currentOffset;

			scrollView.SetContentOffset(new CGPoint(newTargetOffset + 37.5, 0), true);

			int index = (int)(newTargetOffset / pageWidth);
			this.ItemSelected(collectionView, NSIndexPath.FromItemSection(index + 1, 0));
		}

		public virtual nint RowsInSection(UITableView tableView, nint section)
		{
			return this.Groups?[this.CurrentKey].Count ?? 0;
		}

		public virtual UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell(TableViewKey, indexPath) as TTableView ?? this.CreateTableView();
			var item = this.Groups[this.CurrentKey].ElementAt(indexPath.Row);
			this.InflateView(cell, CurrentKey, item);
			return cell;
		}

		[Export("tableView:heightForRowAtIndexPath:")]
		public virtual nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			return tableView.RowHeight;
		}

		[Export("collectionView:didSelectItemAtIndexPath:")]
		public virtual void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
		{
			this.SelectedItem = indexPath;
			var cell = (TGroupView)collectionView.CellForItem(indexPath);
			var lCell = collectionView.CellForItem(NSIndexPath.FromItemSection(indexPath.Item - 1, indexPath.Section));
			var l2Cell = collectionView.CellForItem(NSIndexPath.FromItemSection(indexPath.Item - 2, indexPath.Section));
			var rCell = collectionView.CellForItem(NSIndexPath.FromItemSection(indexPath.Item + 1, indexPath.Section));
			var r2Cell = collectionView.CellForItem(NSIndexPath.FromItemSection(indexPath.Item + 2, indexPath.Section));

			UIView.Animate(animationSpeed, () =>
			{
				cell.Transform = identityTransform;
				if (lCell != null)
					lCell.Transform = transform;
				if (l2Cell != null)
					l2Cell.Transform = transform;
				if (rCell != null)
					rCell.Transform = transform;
				if (r2Cell != null)
					r2Cell.Transform = transform;
			}, () =>
				{
					this.CurrentKey = cell.GroupKey;
					tableView.ReloadData();
					this.OnGroupChanged?.Invoke(cell);
				});
			collectionView.ScrollToItem(indexPath, UICollectionViewScrollPosition.CenteredHorizontally, true);
		}

		public virtual void RefreshTable()
		{
			IsFirstTimeTransform = true;
			this.CollectionView.ReloadData();
			this.TableView.ReloadData();
		}
	}
}