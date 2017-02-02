using Mehspot.Core.DTO;

namespace mehspot.Core.Messaging
{
    public class MessagesResult
    {
        public CollectionDto<MessageDto> Messages { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsSuccess { get; set; }
    }
}