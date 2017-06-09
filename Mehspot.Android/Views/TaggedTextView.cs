using Android.Content;
using Android.Util;
using Android.Widget;

namespace Mehspot.AndroidApp.Views
{
	public class TaggedTextView<T> : TextView
	{
		public T Data { get; set; }

		public TaggedTextView(Context context) :
			base(context)
		{
			Initialize();
		}

		public TaggedTextView(Context context, IAttributeSet attrs) :
			base(context, attrs)
		{
			Initialize();
		}

		public TaggedTextView(Context context, IAttributeSet attrs, int defStyle) :
			base(context, attrs, defStyle)
		{
			Initialize();
		}

		void Initialize()
		{
		}
	}
}
