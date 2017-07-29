// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace mehspot.iOS
{
    [Register ("GrouppingView")]
    partial class GrouppingView
    {
        [Outlet]
        UIKit.UICollectionView CategoriesView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        mehspot.iOS.Views.GrouppingView.DelimiterView Delimiter { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView TableView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (CategoriesView != null) {
                CategoriesView.Dispose ();
                CategoriesView = null;
            }

            if (Delimiter != null) {
                Delimiter.Dispose ();
                Delimiter = null;
            }

            if (TableView != null) {
                TableView.Dispose ();
                TableView = null;
            }
        }
    }
}