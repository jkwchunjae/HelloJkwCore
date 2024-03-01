namespace HelloJkwCore2.Authentication;

[TextJsonConverter(typeof(TextJsonStringEnumConverter))]
public enum AuthProvider
{
    Google,
    KakaoTalk,
    Dropbox,
}
