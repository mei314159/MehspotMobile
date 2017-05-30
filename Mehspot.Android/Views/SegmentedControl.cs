using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;

namespace Mehspot.AndroidApp.Views
{
	public class SegmentedControl : LinearLayout
	{
		readonly Activity activity;

		public SegmentedControl(Activity context) : base(context)
		{
			this.activity = context;
			LayoutInflater inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
			inflater.Inflate(Resource.Layout.SegmentedControl, this);

			this.DetailsButton.Click += Button_Click;
			this.MessageButton.Click += Button_Click;
			this.RecommendationsButton.Click += Button_Click;
			Buttons = new[] { DetailsButton, RecommendationsButton, MessageButton };
		}

		public ImageButton DetailsButton => (ImageButton)FindViewById(Resource.SegmentedControl.DetailsButton);
		public ImageButton RecommendationsButton => (ImageButton)FindViewById(Resource.SegmentedControl.RecommendationsButton);
		public ImageButton MessageButton => (ImageButton)FindViewById(Resource.SegmentedControl.MessageButton);

		private ImageButton[] Buttons;


		void Button_Click(object sender, System.EventArgs e)
		{
			var button = (ImageButton)sender;

			var darkGreenColor = Resources.GetColor(Resource.Color.dark_green);
			foreach (var item in Buttons)
			{
				if (button == item)
				{
					button.SetBackgroundColor(darkGreenColor);
					button.SetColorFilter(Color.White);
				}
				else
				{
					button.SetBackgroundColor(Color.White);
					button.SetColorFilter(darkGreenColor);
				}
			}
		}
	}
}