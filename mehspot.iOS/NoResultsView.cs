using Foundation;
using System;
using UIKit;

namespace mehspot.iOS
{
	public partial class NoResultsView : UIView
	{
		public static readonly NSString Key = new NSString("NoResultsView");
		public static readonly UINib Nib;

		static NoResultsView()
		{
			Nib = UINib.FromName("NoResultsView", NSBundle.MainBundle);
		}

		public NoResultsView(IntPtr handle) : base(handle)
		{
		}

		public static NoResultsView Create()
		{
			var cell = (NoResultsView)Nib.Instantiate(null, null)[0];
			return cell;
		}
	}
}