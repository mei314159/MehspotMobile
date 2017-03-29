using System;
using UIKit;
using CoreGraphics;
using Foundation;
using System.Linq;

namespace Mehspot.iOS.Views.MultiSelectPicker
{
    
    public class MultiSelectPickerViewController : UIViewController, IUITableViewDataSource
    {
        public event Action<int[]> OnModalPickerDismissed;
        const float _headerBarHeight = 40;
        const string CellIdentifier = "MultiselectCell";

        public UIColor HeaderBackgroundColor { get; set; }
        public UIColor HeaderTextColor { get; set; }
        public string HeaderText { get; set; }
        public string DoneButtonText { get; set; }
        public string CancelButtonText { get; set; }
        public UITableView TableView { get; private set; } = new UITableView ();

        UILabel _headerLabel;
        UIButton _doneButton;
        UIButton _cancelButton;
        UIViewController _parent;
        UIView _internalView;
        SourceItem [] TableItems;

        public MultiSelectPickerViewController (string headerText, UIViewController parent, SourceItem [] tableItems)
        {
            //TableView.AllowsMultipleSelection = true;
            TableView.AllowsMultipleSelectionDuringEditing = true;
            TableItems = tableItems;
            HeaderBackgroundColor = UIColor.White;
            HeaderTextColor = UIColor.Black;
            HeaderText = headerText;
            _parent = parent;
            DoneButtonText = "Done";
            CancelButtonText = "Cancel";
            TableView.SetEditing (true, false);
            TableView.DataSource = this;
        }

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();

            InitializeControls ();
        }

        public override void ViewWillAppear (bool animated)
        {
            base.ViewDidAppear (animated);
            TableView.ReloadData ();
            Show ();
        }

        void InitializeControls ()
        {
            View.BackgroundColor = UIColor.Clear;
            _internalView = new UIView ();

            _headerLabel = new UILabel (new CGRect (0, 0, 320 / 2, 44));
            _headerLabel.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
            _headerLabel.BackgroundColor = HeaderBackgroundColor;
            _headerLabel.TextColor = HeaderTextColor;
            _headerLabel.Text = HeaderText;
            _headerLabel.TextAlignment = UITextAlignment.Center;

            _cancelButton = UIButton.FromType (UIButtonType.System);
            _cancelButton.SetTitleColor (HeaderTextColor, UIControlState.Normal);
            _cancelButton.BackgroundColor = UIColor.Clear;
            _cancelButton.SetTitle (CancelButtonText, UIControlState.Normal);
            _cancelButton.TouchUpInside += CancelButtonTapped;

            _doneButton = UIButton.FromType (UIButtonType.System);
            _doneButton.SetTitleColor (HeaderTextColor, UIControlState.Normal);
            _doneButton.BackgroundColor = UIColor.Clear;
            _doneButton.SetTitle (DoneButtonText, UIControlState.Normal);
            _doneButton.TouchUpInside += DoneButtonTapped;


            _internalView.AddSubview (TableView);
            _internalView.BackgroundColor = HeaderBackgroundColor;

            _internalView.AddSubview (_headerLabel);
            _internalView.AddSubview (_cancelButton);
            _internalView.AddSubview (_doneButton);

            Add (_internalView);
        }

        void Show (bool onRotate = false)
        {
            var buttonSize = new CGSize (71, 30);

            var width = _parent.View.Frame.Width;

            TableView.Frame = new CGRect (0, 0, width, 216);

            var internalViewSize = CGSize.Empty;
            internalViewSize = new CGSize (width, TableView.Frame.Height + _headerBarHeight);

            var internalViewFrame = CGRect.Empty;
            if (InterfaceOrientation == UIInterfaceOrientation.Portrait) {
                if (onRotate) {
                    internalViewFrame = new CGRect (0, View.Frame.Height - internalViewSize.Height, internalViewSize.Width, internalViewSize.Height);
                } else {
                    internalViewFrame = new CGRect (0, View.Bounds.Height - internalViewSize.Height, internalViewSize.Width, internalViewSize.Height);
                }
            } else {
                if (onRotate) {
                    internalViewFrame = new CGRect (0, View.Bounds.Height - internalViewSize.Height, internalViewSize.Width, internalViewSize.Height);
                } else {
                    internalViewFrame = new CGRect (0, View.Frame.Height - internalViewSize.Height, internalViewSize.Width, internalViewSize.Height);
                }
            }
            _internalView.Frame = internalViewFrame;

            TableView.Frame = new CGRect (TableView.Frame.X, _headerBarHeight, _internalView.Frame.Width, TableView.Frame.Height);

            _headerLabel.Frame = new CGRect (20 + buttonSize.Width, 4, _parent.View.Frame.Width - (40 + 2 * buttonSize.Width), 35);
            _doneButton.Frame = new CGRect (internalViewFrame.Width - buttonSize.Width - 10, 7, buttonSize.Width, buttonSize.Height);
            _cancelButton.Frame = new CGRect (10, 7, buttonSize.Width, buttonSize.Height);
        }

        void DoneButtonTapped (object sender, EventArgs e)
        {
            var indexes = TableView.IndexPathsForSelectedRows?.Select (a => a.Row).ToArray ();
            TableView.SetEditing (false, false);
            TableView.RemoveFromSuperview ();
            TableView.Dispose ();
            TableView = null;
            DismissViewController (true, null);
            OnModalPickerDismissed?.Invoke (indexes);
        }

        void CancelButtonTapped (object sender, EventArgs e)
        {
            DismissViewController (true, null);
        }

        public override void DidRotate (UIInterfaceOrientation fromInterfaceOrientation)
        {
            base.DidRotate (fromInterfaceOrientation);

            if (InterfaceOrientation == UIInterfaceOrientation.Portrait ||
                InterfaceOrientation == UIInterfaceOrientation.LandscapeLeft ||
                InterfaceOrientation == UIInterfaceOrientation.LandscapeRight) {
                Show (true);
                View.SetNeedsDisplay ();
            }
        }

        public nint RowsInSection (UITableView tableView, nint section)
        {
            return TableItems.Length;
        }

        public UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell (CellIdentifier);
            var item = TableItems [indexPath.Row];

            //---- if there are no cells to reuse, create a new one
            if (cell == null) {
                cell = new UITableViewCell (UITableViewCellStyle.Default, CellIdentifier);
            }

            cell.TextLabel.Text = item.Name;
            cell.SetSelected (item.Selected, false);
            return cell;
        }
    }
}

