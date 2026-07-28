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

        return services;
    }
}
