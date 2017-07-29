using Foundation;
using System;
using UIKit;
using System.ComponentModel;
using ObjCRuntime;
using CoreGraphics;
using mehspot.iOS.Views;

namespace mehspot.iOS
{
	[DesignTimeVisible(true)]
	public partial class GrouppingView : UIView
	{
		public GrouppingView(IntPtr handle) : base(handle)
		{
		}

		public override void AwakeFromNib()
		{

		}

		public static GrouppingView Create<TKey, T, TGroupView, TTableView>(CGRect frame, GrouppingViewDataSource<TKey, T, TGroupView, TTableView> dataSource)
			where TGroupView : UICollectionViewCell, IGrouppingViewCell<TKey>
			where TTableView : UITableViewCell
		{
			var arr = NSBundle.MainBundle.LoadNib("GrouppingView", null, null);
			var v = Runtime.GetNSObject<GrouppingView>(arr.ValueAt(0));
			v.Frame = frame;
			v.Initialize(dataSource);
			return v;
		}


		public void Initialize<TKey, T, TGroupView, TTableView>(GrouppingViewDataSource<TKey, T, TGroupView, TTableView> dataSource)
			where TGroupView : UICollectionViewCell, IGrouppingViewCell<TKey>
			where TTableView : UITableViewCell
		{
			dataSource.CollectionView = CategoriesView;
			dataSource.TableView = TableView;
			this.CategoriesView.DataSource = dataSource;
			this.CategoriesView.Delegate = dataSource;
			this.TableView.DataSource = dataSource;
			this.TableView.Delegate = dataSource;
			dataSource.OnGroupChanged += (groupView) =>
			{
				this.Delimiter.Color = UIColor.FromCGColor(groupView.Layer.BorderColor);
			};
		}
	}
}