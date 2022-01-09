using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HelloJkwCore
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AuthProvider
    {
        Google,
        KakaoTalk,
        Dropbox,
    }
}
