namespace HelloJkwCore;

public class AuthUtil
{
    private List<OAuthOption> _oauthOptions;

    public AuthUtil(IFileSystem fs)
    {
        var task = fs.ReadJsonAsync<List<OAuthOption>>(path => path["OAuthOption"]);
        task.Wait();
        _oauthOptions = task.Result;
    }

    public OAuthOption GetAuthOption(AuthProvider provider)
    {
        return _oauthOptions
            ?.FirstOrDefault(x => x.Provider == provider);
    }
}