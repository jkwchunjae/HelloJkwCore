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
        services.AddSingleton<ICacheClearInvoker, CacheClearInvoker>();
        services.AddSingleton<IWorldCupService, WorldCupService>();
        services.AddSingleton<IBettingService, BettingService>();
        services.AddSingleton<IFifa, Fifa>();
        services.AddSingleton<I2022WorldCupService, BettingService2022>();

        // 2022 서비스 (기본값 pathKey="Betting2022")
        services.AddSingleton<IBettingGroupStageService, BettingGroupStageService>();
        services.AddSingleton<IBettingRound16Service, BettingRound16Service>();
        services.AddSingleton<IBettingFinalService, BettingFinalService>();

        // 2026 서비스 (keyed, 기존 클래스 재사용)
        services.AddKeyedSingleton<IBettingGroupStageService>("2026", (sp, _) =>
            new BettingGroupStageService(
                sp.GetRequiredService<IFileSystemService>(),
                sp.GetRequiredService<IWorldCupService>(),
                sp.GetRequiredService<ICacheClearInvoker>(),
                sp.GetRequiredService<WorldCupOption>(),
                pathKey: "Betting2026",
                getGroupsFunc: wcs => wcs.GetGroupsFromStandingAsync()));

        services.AddKeyedSingleton<IBettingRound16Service>("2026-round32", (sp, _) =>
            new BettingRound16Service(
                sp.GetRequiredService<IFileSystemService>(),
                sp.GetRequiredService<IFifa>(),
                sp.GetRequiredService<IWorldCupService>(),
                sp.GetRequiredService<ICacheClearInvoker>(),
                sp.GetRequiredService<WorldCupOption>(),
                pathKey: "Betting2026",
                bettingType: BettingType.Round32,
                getMatchesFunc: wcs => wcs.GetRound32MatchesAsync(),
                startTime: WorldCupConst.Round32Match1StartTime,
                winnersStageId: Fifa.Round16StageId));

        services.AddKeyedSingleton<IBettingRound16Service>("2026", (sp, _) =>
            new BettingRound16Service(
                sp.GetRequiredService<IFileSystemService>(),
                sp.GetRequiredService<IFifa>(),
                sp.GetRequiredService<IWorldCupService>(),
                sp.GetRequiredService<ICacheClearInvoker>(),
                sp.GetRequiredService<WorldCupOption>(),
                pathKey: "Betting2026",
                bettingType: BettingType.Round16,
                getMatchesFunc: wcs => wcs.GetRound16MatchesAsync(),
                startTime: WorldCupConst.Round16Match1StartTime,
                winnersStageId: Fifa.Round8StageId));

        services.AddKeyedSingleton<IBettingFinalService>("2026", (sp, _) =>
            new BettingFinalService(
                sp.GetRequiredService<IFileSystemService>(),
                sp.GetRequiredService<IFifa>(),
                sp.GetRequiredService<IWorldCupService>(),
                sp.GetRequiredService<ICacheClearInvoker>(),
                sp.GetRequiredService<WorldCupOption>(),
                pathKey: "Betting2026"));
    }
}
