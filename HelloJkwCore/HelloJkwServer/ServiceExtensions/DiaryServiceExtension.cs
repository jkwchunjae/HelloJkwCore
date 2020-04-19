using Common;
using HelloJkwService.Diary;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HelloJkwServer.ServiceExtensions
{
    public static class DiaryServiceExtension
    {
        public static IServiceCollection AddDiaryService(this IServiceCollection services, IConfiguration configuration)
        {
            var diaryOption = new DiaryOption
            {
                RootPath = configuration.GetPath(PathOf.DiaryRootPath),
                DiaryListPath = configuration.GetPath(PathOf.DiaryListFile),
            };

            return services
                .AddSingleton(diaryOption)
                .AddSingleton<DiaryService>();
        }
    }
}
