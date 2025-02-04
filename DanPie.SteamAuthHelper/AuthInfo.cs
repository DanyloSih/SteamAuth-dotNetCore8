using Newtonsoft.Json;

namespace DanPie.SteamAuthHelper
{
    public class AuthInfo
    {
        [JsonProperty("user")] public string UserName = string.Empty;
        [JsonProperty("pass")] public string Password = string.Empty;
        [JsonProperty("auth")] public string Code = string.Empty;

        public AuthInfo()
        {
        }

        public AuthInfo(string userName, string password, string code)
        {
            UserName = userName;
            Password = password;
            Code = code;
        }
    }
}