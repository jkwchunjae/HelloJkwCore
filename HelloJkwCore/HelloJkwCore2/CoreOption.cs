using HelloJkwCore2.Authentication;

namespace HelloJkwCore2;

public class CoreOption
{
    public Dictionary<string, OAuthConfig>? AuthOptions { get; set; }
    public FileSystemSelectOption? UserStoreFileSystem { get; set; }
    public PathMap? Path { get; set; }

    public static CoreOption Create(IConfiguration configuration)
    {
        var option = new CoreOption();
        configuration.GetSection("HelloJkw").Bind(option);
        return option;
    }
}
