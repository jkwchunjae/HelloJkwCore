using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ProjectDiary;

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
        services.AddSingleton<IDiaryAdminService, DiaryAdminService>();
        services.AddSingleton<IDiaryTemporaryService, DiaryTemporaryService>();
        services.AddSingleton<JsonConverter>(new StringIdTextJsonConverter<DiaryName>(id => new DiaryName(id)));
        services.AddKeyedSingleton<IFileSystem>(nameof(DiaryService), (provider, key) =>
        {
            var option = provider.GetRequiredService<DiaryOption>();
            var fileSystemService = provider.GetRequiredService<IFileSystemService>();
            var fileSystem = fileSystemService.GetFileSystem(option.FileSystemSelect, option.Path);
            return fileSystem;
        });
    }
}