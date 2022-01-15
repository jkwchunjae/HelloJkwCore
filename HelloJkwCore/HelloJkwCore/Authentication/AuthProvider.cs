namespace HelloJkwCore;

[JsonConverter(typeof(StringEnumConverter))]
public enum AuthProvider
{
    Google,
    KakaoTalk,
    Dropbox,
}