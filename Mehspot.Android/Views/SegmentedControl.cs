using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Support.V4.Content;
using Android.Support.V4.Content.Res;
using Android.Views;
using Android.Widget;

namespace Mehspot.AndroidApp.Views
{
	public class SegmentedControl : LinearLayout
	{
		readonly Context context;

		public SegmentedControl(Context context, Android.Util.IAttributeSet attrs) : base(context, attrs)
		{
			this.context = context;
			LayoutInflater inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
			inflater.Inflate(Resource.Layout.SegmentedControl, this);

			this.DetailsButton.Click += Button_Click;
			this.MessageButton.Click += Button_Click;
			this.RecommendationsButton.Click += Button_Click;
			Buttons = new[] { DetailsButton, RecommendationsButton, MessageButton };
			HighlightSelectedButton(DetailsButton);
		}

		public ImageButton DetailsButton => (ImageButton)FindViewById(Resource.SegmentedControl.DetailsButton);
		public ImageButton RecommendationsButton => (ImageButton)FindViewById(Resource.SegmentedControl.RecommendationsButton);
		public ImageButton MessageButton => (ImageButton)FindViewById(Resource.SegmentedControl.MessageButton);

		private ImageButton[] Buttons;


		void Button_Click(object sender, System.EventArgs e)
		{
			var button = (ImageButton)sender;
			HighlightSelectedButton(button);
		}

		public void HighlightSelectedButton(ImageButton button)
		{
			foreach (var item in Buttons)
			{
				item.SetBackgroundResource(Resource.Color.white);
				item.SetColorFilter(new Color(ContextCompat.GetColor(context, Resource.Color.dark_green)));
			}

			button.SetBackgroundResource(Resource.Color.dark_green);
			button.SetColorFilter(Color.White);
		}
	}
}