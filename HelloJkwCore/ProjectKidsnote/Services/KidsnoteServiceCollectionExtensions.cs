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
        var options = new KidsnoteOptions();
        configuration.GetSection(KidsnoteOptions.SectionName).Bind(options);

        services.AddSingleton(options);
        services.AddScoped<IKidsnoteClient, KidsnoteClient>();

        return services;
    }
}
