using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectWorldCup;

public static class WorldCupServiceHelper
{
    public static void AddWorldCupService(this IServiceCollection services, IConfiguration configuration)
    {
        var option = new WorldCupOption();
        configuration.GetSection(nameof(WorldCupService)).Bind(option);

        services.AddSingleton(option);
        services.AddSingleton<IWorldCupService, WorldCupService>();
        services.AddSingleton<IFifa, Fifa>();
    }
}