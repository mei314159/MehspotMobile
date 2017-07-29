using Foundation;
using Mehspot.Core.Contracts;
using Newtonsoft.Json;
using Mehspot.Core.DTO;
using Mehspot.Core.Push;
using System;

namespace Mehspot.iOS.Core
{
	public class ApplicationDataStorage : IApplicationDataStorage
	{
		public AuthenticationInfoDTO AuthInfo
		{
			get
			{
				var data = NSUserDefaults.StandardUserDefaults.StringForKey(nameof(IApplicationDataStorage.AuthInfo));

				if (!string.IsNullOrWhiteSpace(data))
				{
					var result = JsonConvert.DeserializeObject<AuthenticationInfoDTO>(data);
					return result;
				}

				return null;
			}
			set
			{
				if (value == null)
				{
					NSUserDefaults.StandardUserDefaults.RemoveObject(nameof(IApplicationDataStorage.AuthInfo));
				}
				else
				{
					var data = JsonConvert.SerializeObject(value);
					NSUserDefaults.StandardUserDefaults.SetString(data, nameof(IApplicationDataStorage.AuthInfo));
					NSUserDefaults.StandardUserDefaults.Synchronize();
				}
			}
		}

		public string PushToken
		{
			get
			{
				var result = NSUserDefaults
					.StandardUserDefaults
					.StringForKey(nameof(IApplicationDataStorage.PushToken));

				return result;
			}

			set
			{
				NSUserDefaults
					.StandardUserDefaults
					.SetString(value, nameof(IApplicationDataStorage.PushToken));
				NSUserDefaults.StandardUserDefaults.Synchronize();
			}
		}

		public string OldPushToken
		{
			get
			{
				var result = NSUserDefaults
					.StandardUserDefaults
					.StringForKey(nameof(IApplicationDataStorage.OldPushToken));

				return result;
			}

			set
			{
				if (value == null)
				{
					NSUserDefaults.StandardUserDefaults.RemoveObject(nameof(IApplicationDataStorage.OldPushToken));
				}
				else
				{
					NSUserDefaults
						.StandardUserDefaults
						.SetString(value, nameof(IApplicationDataStorage.OldPushToken));
					NSUserDefaults.StandardUserDefaults.Synchronize();
				}
			}
		}

		public bool PushDeviceTokenSentToBackend
		{
			get
			{
				var result = NSUserDefaults
					.StandardUserDefaults
					.BoolForKey(nameof(IApplicationDataStorage.PushDeviceTokenSentToBackend));

				return result;
			}

			set
			{
				NSUserDefaults
					.StandardUserDefaults
					.SetBool(value, nameof(IApplicationDataStorage.PushDeviceTokenSentToBackend));
				NSUserDefaults.StandardUserDefaults.Synchronize();
			}
		}

		public bool PushIsEnabled
		{
			get
			{
				var result = NSUserDefaults
					.StandardUserDefaults
					.BoolForKey(nameof(IApplicationDataStorage.PushIsEnabled));

				return result;
			}

			set
			{
				NSUserDefaults
					.StandardUserDefaults
					.SetBool(value, nameof(IApplicationDataStorage.PushIsEnabled));
				NSUserDefaults.StandardUserDefaults.Synchronize();
			}
		}

		public OsType OsType
		{
			get
			{
				return OsType.iOS;
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
				var data = NSUserDefaults.StandardUserDefaults.StringForKey(nameof(IApplicationDataStorage.Profile));

				if (!string.IsNullOrWhiteSpace(data))
				{
					var result = JsonConvert.DeserializeObject<ProfileDto>(data);
					return result;
				}

				return null;
			}
			set
			{
				if (value == null)
				{
					NSUserDefaults.StandardUserDefaults.RemoveObject(nameof(IApplicationDataStorage.Profile));
				}
				else
				{
					var data = JsonConvert.SerializeObject(value);
					NSUserDefaults.StandardUserDefaults.SetString(data, nameof(IApplicationDataStorage.Profile));
					NSUserDefaults.StandardUserDefaults.Synchronize();
				}
			}
		}

		public UserProfileSummaryDTO UserProfile
		{
			get
			{
				var data = NSUserDefaults.StandardUserDefaults.StringForKey(nameof(IApplicationDataStorage.UserProfile));

				if (!string.IsNullOrWhiteSpace(data))
				{
					var result = JsonConvert.DeserializeObject<UserProfileSummaryDTO>(data);
					return result;
				}

				return null;
			}

			set
			{
				if (value == null)
				{
					NSUserDefaults.StandardUserDefaults.RemoveObject(nameof(IApplicationDataStorage.UserProfile));
				}
				else
				{
					var data = JsonConvert.SerializeObject(value);
					NSUserDefaults.StandardUserDefaults.SetString(data, nameof(IApplicationDataStorage.UserProfile));
					NSUserDefaults.StandardUserDefaults.Synchronize();
				}
			}
		}

        public BadgeGroup PreferredBadgeGroup
		{
			get
			{
				var result = (BadgeGroup)(int)NSUserDefaults
					.StandardUserDefaults
					.IntForKey(nameof(IApplicationDataStorage.PreferredBadgeGroup));

				return result;
			}

			set
			{
				NSUserDefaults
					.StandardUserDefaults
					.SetInt((int)value, nameof(IApplicationDataStorage.PreferredBadgeGroup));
				NSUserDefaults.StandardUserDefaults.Synchronize();
			}
		}

		public T Get<T>(string key)
		{
			var data = NSUserDefaults.StandardUserDefaults.StringForKey(key);

			if (!string.IsNullOrWhiteSpace(data))
			{
				var result = JsonConvert.DeserializeObject<T>(data);
				return result;
			}

			return default(T);
		}

		public void Set<T>(string key, T value) where T : class
		{
			if (value == null)
			{
				NSUserDefaults.StandardUserDefaults.RemoveObject(key);
			}
			else
			{
				var data = JsonConvert.SerializeObject(value);
				NSUserDefaults.StandardUserDefaults.SetString(data, key);
				NSUserDefaults.StandardUserDefaults.Synchronize();
			}
		}
	}
}
