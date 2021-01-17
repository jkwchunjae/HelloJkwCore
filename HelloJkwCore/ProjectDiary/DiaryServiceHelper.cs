using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDiary
{
    public static partial class DiaryServiceHelper
    {
        public static void AddDiaryService(this IServiceCollection services, IConfiguration configuration)
        {
            var diaryOption = new DiaryOption();
            configuration.GetSection("DiaryService").Bind(diaryOption);

            services.AddScoped<UserInstantData>();
            services.AddSingleton(diaryOption);
            services.AddSingleton<IDiaryService, DiaryService>();
            services.AddSingleton<IDiarySearchService, DiarySearchService>();
        }
    }
}
