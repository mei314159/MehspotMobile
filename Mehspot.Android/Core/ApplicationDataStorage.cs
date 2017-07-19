using Android.App;
using Android.Content;
using Mehspot.Core.Contracts;
using Newtonsoft.Json;
using Mehspot.Core.DTO;
using System;
using Mehspot.Core.Push;

namespace Mehspot.AndroidApp.Core
{
	public class ApplicationDataStorage : IApplicationDataStorage
	{
		readonly ISharedPreferences settings;

		public ApplicationDataStorage()
		{
			settings = Application.Context.GetSharedPreferences("Mehspot", FileCreationMode.Private);
		}

		public AuthenticationInfoDTO AuthInfo
		{
			get
			{
				var data = settings.GetString(nameof(IApplicationDataStorage.AuthInfo), null);
				if (!string.IsNullOrWhiteSpace(data))
				{
					var result = JsonConvert.DeserializeObject<AuthenticationInfoDTO>(data);
					return result;
				}

				return null;
			}
			set
			{
				var data = value == null ? null : JsonConvert.SerializeObject(value);
				var prefEditor = settings.Edit();
				prefEditor.PutString(nameof(IApplicationDataStorage.AuthInfo), data);
				prefEditor.Commit();
			}
		}

		public string PushToken
		{
			get
			{
				var result = settings.GetString(nameof(IApplicationDataStorage.PushToken), null);
				return result;
			}

			set
			{
				var prefEditor = settings.Edit();
				prefEditor.PutString(nameof(IApplicationDataStorage.PushToken), value);
				prefEditor.Commit();
			}
		}

		public string OldPushToken
		{
			get
			{
				var result = settings.GetString(nameof(IApplicationDataStorage.OldPushToken), null);
				return result;
			}

			set
			{
				var prefEditor = settings.Edit();
				prefEditor.PutString(nameof(IApplicationDataStorage.OldPushToken), value);
				prefEditor.Commit();
			}
		}

		public bool PushDeviceTokenSentToBackend
		{
			get
			{
				var result = settings.GetBoolean(nameof(IApplicationDataStorage.PushDeviceTokenSentToBackend), false);
				return result;
			}

			set
			{
				var prefEditor = settings.Edit();
				prefEditor.PutBoolean(nameof(IApplicationDataStorage.PushDeviceTokenSentToBackend), value);
				prefEditor.Commit();
			}
		}

		public bool PushIsEnabled
		{
			get
			{
				var result = settings.GetBoolean(nameof(IApplicationDataStorage.PushIsEnabled), false);
				return result;
			}

			set
			{
				var prefEditor = settings.Edit();
				prefEditor.PutBoolean(nameof(IApplicationDataStorage.PushIsEnabled), value);
				prefEditor.Commit();
			}
		}



		public OsType OsType
		{
			get
			{
				return OsType.Android;
			}
		}

		public bool WalkthroughPassed
		{
			get
			{
				return Profile != null &&
				!string.IsNullOrWhiteSpace(Profile.ProfilePicturePath) &&
				!string.IsNullOrWhiteSpace(Profile.Zip) &&
				Profile.SubdivisionId != null;
			}
		}

		public ProfileDto Profile
		{
			get
			{
				var data = settings.GetString(nameof(IApplicationDataStorage.Profile), null);
				if (!string.IsNullOrWhiteSpace(data))
				{
					var result = JsonConvert.DeserializeObject<ProfileDto>(data);
					return result;
				}

				return null;
			}
			set
			{
				var data = value == null ? null : JsonConvert.SerializeObject(value);
				var prefEditor = settings.Edit();
				prefEditor.PutString(nameof(IApplicationDataStorage.Profile), data);
				prefEditor.Commit();
			}
		}

		public T Get<T>(string key)
		{
			var data = settings.GetString(key, null);
			if (!string.IsNullOrWhiteSpace(data))
			{
				var result = JsonConvert.DeserializeObject<T>(data);
				return result;
			}

			return default(T);
		}

		public void Set<T>(string key, T value) where T : class
		{
			var data = value == null ? null : JsonConvert.SerializeObject(value);
			var prefEditor = settings.Edit();
			prefEditor.PutString(key, data);
			prefEditor.Commit();
		}
	}
}
