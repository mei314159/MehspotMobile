﻿using System;
using System.Collections.Generic;
using CoreAnimation;
using ObjCRuntime;
using UIKit;

namespace Mehspot.iOS.Extensions
{
	public static class ViewExtensions
	{
		/// <summary>
		/// Find the first responder in the <paramref name="view"/>'s subview hierarchy
		/// </summary>
		/// <param name="view">
		/// A <see cref="UIView"/>
		/// </param>
		/// <returns>
		/// A <see cref="UIView"/> that is the first responder or null if there is no first responder
		/// </returns>
		public static UIView FindFirstResponder(this UIView view)
		{
			if (view.IsFirstResponder)
			{
				return view;
			}
			foreach (UIView subView in view.Subviews)
			{
				var firstResponder = subView.FindFirstResponder();
				if (firstResponder != null)
					return firstResponder;
			}
			return null;
		}

		/// <summary>
		/// Find the first Superview of the specified type (or descendant of)
		/// </summary>
		/// <param name="view">
		/// A <see cref="UIView"/>
		/// </param>
		/// <param name="stopAt">
		/// A <see cref="UIView"/> that indicates where to stop looking up the superview hierarchy
		/// </param>
		/// <param name="type">
		/// A <see cref="Type"/> to look for, this should be a UIView or descendant type
		/// </param>
		/// <returns>
		/// A <see cref="UIView"/> if it is found, otherwise null
		/// </returns>
		public static UIView FindSuperviewOfType(this UIView view, UIView stopAt, Type type)
		{
			if (view.Superview != null)
			{
				if (type.IsAssignableFrom(view.Superview.GetType()))
				{
					return view.Superview;
				}

				if (view.Superview != stopAt)
					return view.Superview.FindSuperviewOfType(stopAt, type);
			}

			return null;
		}

		public static IEnumerable<UIView> FindChildViewsOfType(this UIView view, UIView stopAt, Type type)
		{
			if (view.Subviews != null)
			{
				foreach (var subview in view.Subviews)
				{
					if (type.IsAssignableFrom(subview.GetType()))
					{
						yield return subview;
					}

					if (subview != stopAt)
					{
						var children = subview.FindChildViewsOfType(stopAt, type);
						foreach (var item in children)
						{
							yield return item;
						}
					}
				}
			}
		}

		public static void SwapController(this UIWindow window, UIViewController newController, UIViewAnimationOptions opt = UIViewAnimationOptions.TransitionFlipFromLeft)
		{
			UIViewController rootViewController = null;
			rootViewController = window.RootViewController;
			window.RootViewController = newController;
			foreach (var subview in window.Subviews)
			{
				if (subview.IsKindOfClass(new Class("UITransitionView")))
					subview.RemoveFromSuperview();
			}

			rootViewController?.DismissViewController(false, () =>
			{
				rootViewController.View.RemoveFromSuperview();
				rootViewController.Dispose();
			});
		}

		public static UIViewController GetViewController(this UIView view)
		{
			var responder = (UIResponder)view;
			while (responder != null && !(responder is UIViewController))
			{
				responder = responder.NextResponder;
			}

			return (UIViewController)responder;
		}

		public static void HideKeyboard(this UIViewController controller)
		{
			controller.View.FindFirstResponder()?.ResignFirstResponder();
		}
	}


}
