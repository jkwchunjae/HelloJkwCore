using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;

namespace Common
{
    [System.Text.Json.Serialization.JsonConverter(typeof(JsonStringEnumConverter))]
    [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
    public enum AuthProvider
    {
        Google,
        KakaoTalk,
        Dropbox,
    }
}
