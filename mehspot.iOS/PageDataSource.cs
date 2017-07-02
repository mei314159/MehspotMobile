using System;
using UIKit;
using System.Collections.Generic;
using System.Linq;
using Mehspot.Core.Services;
using Mehspot.Core;
using Mehspot.Core.DTO;
using Mehspot.iOS.Extensions;
using Mehspot.Core.Contracts.Wrappers;
using Mehspot.iOS.Wrappers;

namespace mehspot.iOS
{

	public class PageDataSource : UIPageViewControllerDataSource
	{
		List<UIViewController> pages;

		public PageDataSource(List<UIViewController> pages)
		{
			this.pages = pages;
		}

		public override UIViewController GetPreviousViewController(UIPageViewController pageViewController, UIViewController referenceViewController)
		{
			var index = this.pages.IndexOf(referenceViewController);
			return index == 0 ? null : pages[index - 1];
		}

		override public UIViewController GetNextViewController(UIPageViewController pageViewController, UIViewController referenceViewController)
		{
			var index = this.pages.IndexOf(referenceViewController);
			return index == pages.Count - 1 ? null : pages[index + 1];
		}

		public override nint GetPresentationCount(UIPageViewController pageViewController)
		{
			return this.pages.Count;
		}

		public override nint GetPresentationIndex(UIPageViewController pageViewController)
		{
			return this.pages.IndexOf(pageViewController.ViewControllers[0]);
		}
	}
}