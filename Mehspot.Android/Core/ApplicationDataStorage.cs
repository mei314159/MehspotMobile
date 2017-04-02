using Android.App;
using Android.Content;
using mehspot.Core.Contracts;
using Newtonsoft.Json;
using Mehspot.Core.DTO;
using System;
using Mehspot.Core.Push;

namespace mehspot.Android.Core
{
    public class ApplicationDataStorage : IApplicationDataStorage
    {
        readonly ISharedPreferences settings;

        public ApplicationDataStorage ()
        {
            settings = Application.Context.GetSharedPreferences ("Mehspot", FileCreationMode.Private);
        }

        public AuthenticationInfoDTO AuthInfo {
            get {
                var data = settings.GetString (nameof (IApplicationDataStorage.AuthInfo), null);
                if (!string.IsNullOrWhiteSpace (data)) {
                    var result = JsonConvert.DeserializeObject<AuthenticationInfoDTO> (data);
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

        public string PushToken {
            get {
                var result = settings.GetString (nameof (IApplicationDataStorage.PushToken), null);
                return result;
            }

            set {
                var prefEditor = settings.Edit ();
                prefEditor.PutString (nameof (IApplicationDataStorage.PushToken), value);
                prefEditor.Commit ();
            }
        }

        public string OldPushToken {
            get {
                var result = settings.GetString (nameof (IApplicationDataStorage.OldPushToken), null);
                return result;
            }

            set {
                var prefEditor = settings.Edit ();
                prefEditor.PutString (nameof (IApplicationDataStorage.OldPushToken), value);
                prefEditor.Commit ();
            }
        }

        public bool PushDeviceTokenSentToBackend {
            get {
                var result = settings.GetBoolean (nameof (IApplicationDataStorage.PushDeviceTokenSentToBackend), false);
                return result;
            }

            set {
                var prefEditor = settings.Edit ();
                prefEditor.PutBoolean (nameof (IApplicationDataStorage.PushDeviceTokenSentToBackend), value);
                prefEditor.Commit ();
            }
        }

        public bool PushIsEnabled {
            get {
                var result = settings.GetBoolean (nameof (IApplicationDataStorage.PushIsEnabled), false);
                return result;
            }

            set {
                var prefEditor = settings.Edit ();
                prefEditor.PutBoolean (nameof (IApplicationDataStorage.PushIsEnabled), value);
                prefEditor.Commit ();
            }
        }



        public OsType OsType {
            get {
                return OsType.Android;
            }
        }

        public T Get<T> (string key) where T : class
        {
            var data = settings.GetString (key, null);
            if (!string.IsNullOrWhiteSpace (data)) {
                var result = JsonConvert.DeserializeObject<T> (data);
                return result;
            }

            return null;
        }

        public void Set<T> (string key, T value) where T : class
        {
            var data = value == null ? null : JsonConvert.SerializeObject (value);
            var prefEditor = settings.Edit ();
            prefEditor.PutString (key, data);
            prefEditor.Commit ();
        }
    }
}
