using System;
using Android.App;
using Android.Content;
using mehspot.Core.Dto;
using mehspot.Core.Contracts;
using Newtonsoft.Json;

namespace mehspot.Android.Core
{
    public class ApplicationDataStorage : IApplicationDataStorage
    {
        readonly ISharedPreferences settings;

        public ApplicationDataStorage ()
        {
            settings = Application.Context.GetSharedPreferences ("Mehspot", FileCreationMode.Private);
        }

        public AuthenticationInfoDto AuthInfo {
            get {
                var data = settings.GetString (nameof (IApplicationDataStorage.AuthInfo), null);
                if (!string.IsNullOrWhiteSpace (data)) {
                    var result = JsonConvert.DeserializeObject<AuthenticationInfoDto> (data);
                    return result;
                }

                return null;
            }
            set {
                var data = value == null ? null : JsonConvert.SerializeObject (value);
                var prefEditor = settings.Edit ();
                prefEditor.PutString (nameof (IApplicationDataStorage.AuthInfo), data);
                prefEditor.Commit ();
            }
        }
    }
}
