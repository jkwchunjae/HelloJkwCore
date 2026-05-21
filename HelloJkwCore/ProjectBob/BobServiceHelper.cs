namespace ProjectBob;

public static class BobServiceHelper
{
    public static void AddBobService(this IServiceCollection services, IConfiguration configuration)
    {
        var option = new BobOption();
        configuration.GetSection("BobService").Bind(option);

        services.AddSingleton(option);
        services.AddSingleton<IBobService, BobService>();
        services.AddKeyedSingleton<IFileSystem>(nameof(BobService), (provider, _) =>
        {
            var fsService = provider.GetRequiredService<IFileSystemService>();
            var bobOption = provider.GetRequiredService<BobOption>();
            return fsService.GetFileSystem(bobOption.FileSystemSelect, bobOption.Path);
        });
    }
}
