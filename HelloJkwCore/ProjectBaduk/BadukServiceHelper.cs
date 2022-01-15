using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ProjectBaduk;

public static class BadukServiceHelper
{
    public static void AddBadukService(this IServiceCollection services, IConfiguration configuration)
    {
        var badukOption = new BadukOption();
        configuration.GetSection("BadukService").Bind(badukOption);

        services.AddSingleton<IBadukService, BadukService>();
        services.AddSingleton(badukOption);
    }
}