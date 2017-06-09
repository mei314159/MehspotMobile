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
			var height = (int)ConvertDpToPixel(40, Context);
			var padding = (int)ConvertDpToPixel(10, Context);
			this.SetHeight(height);
			this.SetPadding(padding, 0, padding, 0);
		}

		private static float ConvertDpToPixel(float dp, Context context)
		{
			var resources = context.Resources;
			DisplayMetrics metrics = resources.DisplayMetrics;
			float px = dp * ((float)metrics.DensityDpi / (float)DisplayMetricsDensity.Default);
			return px;
		}
	}
}
