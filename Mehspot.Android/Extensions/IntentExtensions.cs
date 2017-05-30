using System;
using System.Collections.Generic;
using System.Net;
using Android.Content;
using Android.Graphics;

namespace Mehspot.AndroidApp
{
	public static class IntentExtensions
	{
		private static Dictionary<string, object> storage = new Dictionary<string, object>();
		public static void PutExtra<T>(this Intent intent, string key, T value)
		{
			var uniqueKey = Guid.NewGuid().ToString();
			storage.Add(uniqueKey, value);
			intent.PutExtra(key, uniqueKey);
		}

		public static T GetExtra<T>(this Intent intent, string key)
		{
			var uniqueKey = intent.GetStringExtra(key);
			return (T)storage[uniqueKey];
		}
	}

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

