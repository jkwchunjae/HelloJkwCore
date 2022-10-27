﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ProjectWorldCup;

public static class WorldCupServiceHelper
{
    public static void AddWorldCupService(this IServiceCollection services, IConfiguration configuration)
    {
        var option = new WorldCupOption();
        configuration.GetSection(nameof(WorldCupService)).Bind(option);

        services.AddSingleton(option);
        services.AddSingleton<ICacheClearInvoker, CacheClearInvoker>();
        services.AddSingleton<IWorldCupService, WorldCupService>();
        services.AddSingleton<IBettingService, BettingService>();
        services.AddSingleton<IBettingGroupStageService, BettingGroupStageService>();
        services.AddSingleton<IBettingRound16Service, BettingRound16Service>();
        services.AddSingleton<IBettingFinalService, BettingFinalService>();
        services.AddSingleton<IFifa, Fifa>();
    }
}