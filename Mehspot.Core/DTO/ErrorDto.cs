using Newtonsoft.Json;

namespace Mehspot.Core.Dto
{

    public class ErrorDto
    {
        [JsonProperty(PropertyName = "error")]
        public virtual string Error { get; set; }

        [JsonProperty(PropertyName = "message")]
        public virtual string ErrorMessage { get; set; }
    }


    public class AuthenticationErrorDto : ErrorDto
    {
        [JsonProperty(PropertyName = "error_description")]
        public override string ErrorMessage { get; set; }
    }
}