using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDiary
{
    public static class DiaryServiceHelper
    {
        public static void AddDiaryService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<UserInstantData>();
            services.AddSingleton<IDiaryService, DiaryService>();
        }
    }
}
