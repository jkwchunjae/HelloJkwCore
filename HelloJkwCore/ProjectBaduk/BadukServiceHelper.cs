using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBaduk
{
    public static class BadukServiceHelper
    {
        public static void AddBadukService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IBadukService, BadukService>();
        }
    }
}
