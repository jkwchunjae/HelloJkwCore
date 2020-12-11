using System.Text.Json.Serialization;

namespace HelloJkwCore.Authentication
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AuthProvider
    {
        Google,
        KakaoTalk,
        Dropbox,
    }
}
