namespace HelloJkwCore.Authentication;

[TextJsonConverter(typeof(TextJsonStringEnumConverter))]
public enum AuthProvider
{
    Google,
    KakaoTalk,
    Dropbox,
}
