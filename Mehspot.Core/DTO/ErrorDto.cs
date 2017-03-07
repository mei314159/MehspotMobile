using System.Collections.Generic;
using Newtonsoft.Json;

namespace mehspot.Core.Dto
{

    public class ErrorDto
    {
        [JsonProperty (PropertyName = "error")]
        public string Error { get; set; }


        [JsonProperty (PropertyName = "error_description")]
        public string ErrorDescription { get; set; }
    }
}