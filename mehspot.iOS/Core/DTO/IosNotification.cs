using System;
using Newtonsoft.Json;

namespace mehspot.iOS.Core.DTO
{
    public class IosNotification
    {
        [JsonProperty (PropertyName = "aps")]
        public NotificationData Data { get; set; }

        [JsonProperty (PropertyName = "fromUserId")]
        public string FromUserId { get; set; }

        [JsonProperty (PropertyName = "fromUserName")]
        public string FromUserName { get; set; }
    }

    public class NotificationData
    {
        [JsonProperty (PropertyName = "alert")]
        public string Message { get; set; }


        [JsonProperty (PropertyName = "badge")]
        public int? Badge { get; set; }


        [JsonProperty (PropertyName = "sound")]
        public string Sound { get; set; }
    }
}
