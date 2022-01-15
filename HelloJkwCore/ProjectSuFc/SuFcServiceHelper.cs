using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ProjectSuFc;

public static class SuFcServiceHelper
{
    public static void AddSuFcService(this IServiceCollection services, IConfiguration configuration)
    {
        var suFcOption = new SuFcOption();
        configuration.GetSection("SuFcService").Bind(suFcOption);

        services.AddSingleton(suFcOption);
        services.AddSingleton<ISuFcService, SuFcService>();
    }
}