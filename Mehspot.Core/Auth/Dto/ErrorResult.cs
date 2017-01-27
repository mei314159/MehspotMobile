using Newtonsoft.Json;

namespace mehspot.Core.Auth.Dto
{

    public class ErrorResult
    {
        [JsonProperty (PropertyName = "error")]
        public string Error { get; set; }


        [JsonProperty (PropertyName = "error_description")]
        public string ErrorDescription { get; set; }
    }
}