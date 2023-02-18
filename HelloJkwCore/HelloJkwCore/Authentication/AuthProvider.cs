namespace HelloJkwCore;

[JsonNetConverter(typeof(JsonNetStringEnumConverter))]
[TextJsonConverter(typeof(TextJsonStringEnumConverter))]
public enum AuthProvider
{
    Google,
    KakaoTalk,
    Dropbox,
}