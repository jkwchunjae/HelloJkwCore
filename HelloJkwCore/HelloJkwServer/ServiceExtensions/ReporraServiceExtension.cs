using HelloJkwService.Reporra;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloJkwServer.ServiceExtensions
{
    public static class ReporraServiceExtension
    {
        public static IServiceCollection AddReporraService(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .AddSingleton<IReporraLobbyService, ReporraLobbyService>();
        }
    }
}
