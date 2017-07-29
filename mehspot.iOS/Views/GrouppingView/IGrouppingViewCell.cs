using System;

using Foundation;
using Mehspot.Core;
using Mehspot.Core.DTO;
using ObjCRuntime;
using UIKit;

namespace mehspot.iOS.Views
{
	public interface IGrouppingViewCell<TKey>
	{
		TKey GroupKey { get; }
	}
	
}
