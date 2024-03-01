namespace HelloJkwCore2;

public class CoreOption
{
    public FileSystemSelectOption? AuthFileSystem { get; set; }
    public FileSystemSelectOption? UserStoreFileSystem { get; set; }
    public PathMap? Path { get; set; }

    public static CoreOption Create(IConfiguration configuration)
    {
        var option = new CoreOption();
        configuration.GetSection("HelloJkw").Bind(option);
        return option;
    }
}
