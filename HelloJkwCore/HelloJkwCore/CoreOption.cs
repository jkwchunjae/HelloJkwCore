using HelloJkwCore.Authentication;

namespace HelloJkwCore;

public class CoreOption
{
    public required Dictionary<string, OAuthConfig> AuthOptions { get; set; }
    public required FileSystemSelectOption UserStoreFileSystem { get; set; }
    public required PathMap Path { get; set; }
}
