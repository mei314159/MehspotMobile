using Mehspot.Core.DTO.Groups;
using Newtonsoft.Json;

namespace Mehspot.Core.DTO.Push
{
    public class PushNotification
    {
        [JsonProperty(PropertyName = "aps")]
        public NotificationData Data { get; set; }

        [JsonProperty(PropertyName = "fromUserId")]
        public string FromUserId { get; set; }

        [JsonProperty(PropertyName = "fromUserName")]
        public string FromUserName { get; set; }

        [JsonProperty(PropertyName = "notificationType")]
        public NotificationTypeEnum NotificationType { get; set; }

        [JsonProperty(PropertyName = "groupMessage")]
        public GroupMessageDTO GroupMessage { get; set; }

        [JsonProperty(PropertyName = "message")]
        public MessageDto Message { get; set; }
    }

    public class NotificationData
    {
        [JsonProperty(PropertyName = "alert")]
        public string Message { get; set; }


        [JsonProperty(PropertyName = "badge")]
        public int? Badge { get; set; }


        [JsonProperty(PropertyName = "sound")]
        public string Sound { get; set; }
    }

    public enum NotificationTypeEnum
    {
        Message = 1,
        GroupMessage = 2,
        EventMessage = 3
    }
}
