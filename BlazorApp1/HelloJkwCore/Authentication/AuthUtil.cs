namespace HelloJkwCore.Authentication;

public class AuthUtil
{
    private List<OAuthConfig> _oauthOptions;

    public AuthUtil(CoreOption core)
    {
        _oauthOptions = core.AuthOptions?.Select(x => x.Value).ToList() ?? new List<OAuthConfig>();
    }

    public OAuthConfig? GetAuthOption(string provider)
    {
        return _oauthOptions
            ?.FirstOrDefault(x => x.Provider == provider);
    }
}
