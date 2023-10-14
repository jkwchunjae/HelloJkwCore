
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GameLibra;

public static class GameLibraServiceExtension
{
    public static void AddGameLibra(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ILibraService, LibraService>();
    }
}
