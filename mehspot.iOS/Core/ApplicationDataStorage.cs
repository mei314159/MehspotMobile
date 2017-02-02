using System;
using Foundation;
using mehspot.Core.Dto;
using mehspot.Core.Contracts;
using Newtonsoft.Json;

namespace mehspot.iOS.Core
{
    public class ApplicationDataStorage : IApplicationDataStorage
    {
        public ApplicationDataStorage ()
        {
        }

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
    }
}
