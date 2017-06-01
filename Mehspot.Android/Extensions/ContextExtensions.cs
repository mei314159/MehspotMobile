using System;
using System.Collections.Generic;
using System.Net;
using Android.Content;
using Android.Graphics;

namespace Mehspot.AndroidApp
{

	public static class ContextExtensions
	{
		public static Bitmap GetImageBitmapFromUrl(this Context activity, string url)
		{
			Bitmap imageBitmap = null;

			using (var webClient = new WebClient())
			{
				var imageBytes = webClient.DownloadData(url);
				if (imageBytes != null && imageBytes.Length > 0)
				{
					imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
				}
			}

			return imageBitmap;
		}
	}
}
