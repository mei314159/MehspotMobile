using Newtonsoft.Json;

namespace Mehspot.Core.Dto
{

    public class ErrorDto
    {
        [JsonProperty(PropertyName = "error")]
        public string Error { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string ErrorMessage { get; set; }
    }
}