using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ProjectPingpong;

public static class PingpongServiceExtension
{
    public static void AddPingpongService(this IServiceCollection services, IConfiguration configuration)
    {
        var pingpongOption = new PingpongOption();
        configuration.GetSection("PingpongService").Bind(pingpongOption);

        services.AddSingleton(pingpongOption);
        services.AddSingleton<IPpService, PpService>();
        services.AddSingleton<IPpMatchService, PpMatchService>();
    }
}
