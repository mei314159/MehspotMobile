using Android.Content;
using Android.Views;
using Android.Widget;

namespace Mehspot.AndroidApp.Resources.layout
{

	public class NoResultsView : RelativeLayout
	{
		public NoResultsView(Context context) : base(context)
		{
			LayoutInflater inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
			inflater.Inflate(Resource.Layout.NoResultsView, this);
		}
	}
}
