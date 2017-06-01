using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Widget;

namespace Mehspot.AndroidApp
{
	public class MultiSpinner : Spinner,
	IDialogInterfaceOnMultiChoiceClickListener,
	IDialogInterfaceOnCancelListener
	{
		public class MultiSpinnerSelectionEventArgs : EventArgs
		{
			public MultiSpinnerSelectionEventArgs(bool[] selected)
			{
				Selected = selected;
			}

			public bool[] Selected { get; private set; }
		}



		public delegate void ItemsSelectedHandler(object sender,
			MultiSpinnerSelectionEventArgs args);
		public event ItemsSelectedHandler ItemsSelected;
		private ISpinnerAdapter RealAdapter;
		private bool[] selected;

		public MultiSpinner(IntPtr a, JniHandleOwnership b) : base(a, b) { }

		public MultiSpinner(Context context) : base(context)
		{
		}

		public MultiSpinner(Context context, IAttributeSet attrs)
			: base(context, attrs)
		{
		}

		public MultiSpinner(Context context, IAttributeSet attrs, int defStyle)
			: base(context, attrs, defStyle)
		{
		}

		public void OnClick(IDialogInterface dialog, int which, bool isChecked)
		{
			selected[which] = isChecked;
		}

		private ISpinnerAdapter CreateLabelAdapter()
		{
			List<string> names = new List<string>();
			int count = RealAdapter != null ? RealAdapter.Count : 0;
			for (int i = 0; i < count; i++)
			{
				if (selected[i]) names.Add(RealAdapter.GetItem(i).ToString());
			}
			string label = string.Join(", ", names);
			if (label.Length == 0)
				label = Context.GetString(Resource.String.LabelNone);
			return new ArrayAdapter<string>(Context, Android.Resource.Layout.SimpleSpinnerItem,
				new string[] { label });
		}

		public void OnCancel(IDialogInterface dialog)
		{
			base.Adapter = CreateLabelAdapter();
			if (ItemsSelected != null)
				ItemsSelected(this, new MultiSpinnerSelectionEventArgs(selected));
		}

		public override bool PerformClick()
		{
			AlertDialog.Builder builder = new AlertDialog.Builder(Context);
			List<string> names = new List<string>();
			int count = RealAdapter != null ? RealAdapter.Count : 0;
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					names.Add(RealAdapter.GetItem(i).ToString());
				}
				builder.SetMultiChoiceItems(names.ToArray(), selected, this);
				builder.SetPositiveButton(global::Android.Resource.String.Ok,
					delegate (object o, DialogClickEventArgs e)
					{
						(o as AlertDialog).Cancel();
					});
				builder.SetOnCancelListener(this);
				builder.Show();
			}
			return true;
		}

		public override ISpinnerAdapter Adapter
		{
			get { return RealAdapter; }
			set
			{
				selected = new bool[value.Count];
				RealAdapter = value;
				base.Adapter = CreateLabelAdapter();
			}
		}
	}
}
