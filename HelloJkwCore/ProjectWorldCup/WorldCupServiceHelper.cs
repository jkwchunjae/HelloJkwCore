using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ProjectWorldCup;

public static class WorldCupServiceHelper
{
    public static void AddWorldCupService(this IServiceCollection services, IConfiguration configuration)
    {
        var option = new WorldCupOption();
        configuration.GetSection(nameof(WorldCupService)).Bind(option);

        services.AddSingleton(option);
        services.AddSingleton<IWorldCupService, WorldCupService>();
        services.AddSingleton<IBettingService, BettingService>();
        services.AddSingleton<IBettingGroupStageService, BettingGroupStageService>();
        services.AddSingleton<IFifa, Fifa>();
    }
}