using Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectKidsnote.Client;
using ProjectKidsnote.Configuration;

namespace ProjectKidsnote.Services;

public static class KidsnoteServiceCollectionExtensions
{
    public static IServiceCollection AddKidsnoteService(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var options = configuration.GetSection(KidsnoteOptions.SectionName).Get<KidsnoteOptions>();

        services.AddSingleton(options!);
        services.AddSingleton<IKidsnoteClient, KidsnoteClient>();
        services.AddSingleton<IKidsnoteService, KidsnoteService>();
        services.AddKeyedSingleton<IFileSystem>(
            nameof(KidsnoteService),
            (provider, _) =>
            {
                var kidsnoteOptions = provider.GetRequiredService<KidsnoteOptions>();
                var fileSystemService = provider.GetRequiredService<IFileSystemService>();
                return fileSystemService.GetFileSystem(
                    kidsnoteOptions.FileSystemSelect,
                    kidsnoteOptions.Path);
            });

        return services;
    }
}
