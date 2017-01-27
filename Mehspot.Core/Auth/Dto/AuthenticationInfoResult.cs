using Newtonsoft.Json;

namespace mehspot.Core.Auth.Dto
{
    public class AuthenticationInfoResult
    {
        [JsonProperty (PropertyName = "access_token")]
        public string AccessToken { get; set; }

        [JsonProperty (PropertyName = "token_type")]
        public string TokenType { get; set; }

        [JsonProperty (PropertyName = "expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty (PropertyName = "firstname")]
        public string FirstName { get; set; }

        [JsonProperty (PropertyName = "lastname")]
        public string LastName { get; set; }

        [JsonProperty (PropertyName = "username")]
        public string UserName { get; set; }

        [JsonProperty (PropertyName = "userid")]
        public string UserId { get; set; }
    }

    
}