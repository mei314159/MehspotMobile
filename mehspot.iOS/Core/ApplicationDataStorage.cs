using System;
using Foundation;
using mehspot.Core.Contracts;
using Newtonsoft.Json;
using Mehspot.Core.DTO;

namespace mehspot.iOS.Core
{
    public class ApplicationDataStorage : IApplicationDataStorage
    {
        public AuthenticationInfoDto AuthInfo {
            get {
                var data = NSUserDefaults.StandardUserDefaults.StringForKey (nameof (IApplicationDataStorage.AuthInfo));

                if (!string.IsNullOrWhiteSpace (data)) {
                    var result = JsonConvert.DeserializeObject<AuthenticationInfoDto> (data);
                    return result;
                }

                return null;
            }
            set {
                var data = value == null ? null : JsonConvert.SerializeObject (value);
                NSUserDefaults.StandardUserDefaults.SetString (data, nameof (IApplicationDataStorage.AuthInfo));
            }
        }

        public string PushToken {
            get {
                var result = NSUserDefaults
                    .StandardUserDefaults
                    .StringForKey (nameof (IApplicationDataStorage.PushToken));

                return result;
            }

            set {
                NSUserDefaults
                    .StandardUserDefaults
                    .SetString (value, nameof (IApplicationDataStorage.PushToken));
            }
        }

        public string OldPushToken {
            get {
                var result = NSUserDefaults
                    .StandardUserDefaults
                    .StringForKey (nameof (IApplicationDataStorage.OldPushToken));

                return result;
            }

            set {
                if (value == null) {
                    NSUserDefaults.StandardUserDefaults.RemoveObject (nameof (IApplicationDataStorage.OldPushToken));
                } else {
                    NSUserDefaults
                        .StandardUserDefaults
                        .SetString (value, nameof (IApplicationDataStorage.OldPushToken));
                }
            }
        }

        public bool PushDeviceTokenSentToBackend {
            get {
                var result = NSUserDefaults
                    .StandardUserDefaults
                    .BoolForKey (nameof (IApplicationDataStorage.PushDeviceTokenSentToBackend));

                return result;
            }

            set {
                NSUserDefaults
                    .StandardUserDefaults
                    .SetBool (value, nameof (IApplicationDataStorage.PushDeviceTokenSentToBackend));
            }
        }

        public bool PushIsEnabled {
            get {
                var result = NSUserDefaults
                    .StandardUserDefaults
                    .BoolForKey (nameof (IApplicationDataStorage.PushIsEnabled));

                return result;
            }

            set {
                NSUserDefaults
                    .StandardUserDefaults
                    .SetBool (value, nameof (IApplicationDataStorage.PushIsEnabled));
            }
        }
    }
}
