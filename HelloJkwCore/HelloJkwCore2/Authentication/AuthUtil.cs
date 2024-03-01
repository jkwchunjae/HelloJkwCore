namespace HelloJkwCore2.Authentication;

public class AuthUtil
{
    private List<OAuthConfig> _oauthOptions;

    public AuthUtil(IFileSystem fs)
    {
        var task = fs.ReadJsonAsync<List<OAuthConfig>>(path => path["OAuthOption"]);
        task.Wait();
        _oauthOptions = task.Result;
    }

    public OAuthConfig? GetAuthOption(AuthProvider provider)
    {
        return _oauthOptions
            ?.FirstOrDefault(x => x.Provider == provider);
    }

    public static AuthUtil Create(IConfiguration configuration, CoreOption coreOption)
    {
        var fsOption = new FileSystemOption();
        configuration.GetSection("FileSystem").Bind(fsOption);

        var fsService = new FileSystemService(fsOption, null, null);
        var fs = fsService.GetFileSystem(coreOption.AuthFileSystem, coreOption.Path);

        return new AuthUtil(fs);
    }
}
