using Foundation;
using System;
using UIKit;

namespace mehspot.iOS
{
	public partial class WalkthroughStep3Controller : UIViewController
	{
		public event Action OnContinue;
		public WalkthroughStep3Controller(IntPtr handle) : base(handle)
		{
		}

		partial void ContinueButtonTouched(UIButton sender)
		{
			OnContinue?.Invoke();
		}
	}
}