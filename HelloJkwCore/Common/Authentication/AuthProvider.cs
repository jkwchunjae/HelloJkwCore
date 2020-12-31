using System.Text.Json.Serialization;

namespace Common
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AuthProvider
    {
        Google,
        KakaoTalk,
        Dropbox,
    }
}
