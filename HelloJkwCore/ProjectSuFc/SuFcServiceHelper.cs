using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSuFc
{
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
}
