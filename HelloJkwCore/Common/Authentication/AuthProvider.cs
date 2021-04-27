using Newtonsoft.Json.Converters;

namespace Common
{
    [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
    public enum AuthProvider
    {
        Google,
        KakaoTalk,
        Dropbox,
    }
}
