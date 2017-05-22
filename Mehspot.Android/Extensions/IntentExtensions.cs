using System;
using System.Collections.Generic;
using Android.Content;

namespace Mehspot.AndroidApp
{
	public static class IntentExtensions
	{
		private static Dictionary<string, object> storage = new Dictionary<string, object>();
		public static void PutExtra<T>(this Intent intent, string key, T value) {
			var uniqueKey = Guid.NewGuid().ToString();
			storage.Add(uniqueKey, value);
			intent.PutExtra(key, uniqueKey);
		}

		public static T GetExtra<T>(this Intent intent, string key) {
			var uniqueKey = intent.GetStringExtra(key);
			return (T)storage[uniqueKey];
		}
	}
}
