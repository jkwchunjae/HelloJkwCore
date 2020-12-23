using System.Text.Json.Serialization;

namespace Common.Authentication
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AuthProvider
    {
        Google,
        KakaoTalk,
        Dropbox,
    }
}
