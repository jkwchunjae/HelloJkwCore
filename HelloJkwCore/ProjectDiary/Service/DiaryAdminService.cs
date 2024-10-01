namespace ProjectDiary;

public class DiaryAdminService : IDiaryAdminService
{
    private readonly IFileSystem _fs;

    public DiaryAdminService(
        [FromKeyedServices(nameof(DiaryService))] IFileSystem fileSystem)
    {
        _fs = fileSystem;
    }

    public async Task<List<DiaryInfo>> GetAllDiaryListAsync(AppUser user)
    {
        if (!(user?.HasRole(UserRole.Admin) ?? false))
            return null;

        var diaryNameList = await _fs.ReadJsonAsync<List<DiaryName>>(path => path.DiaryNameListFile());
        if (diaryNameList?.Empty() ?? true)
            return new List<DiaryInfo>();

        var diaryInfoList = await diaryNameList
            .Select(async diaryName => await _fs.ReadJsonAsync<DiaryInfo>(path => path.DiaryInfo(diaryName)))
            .WhenAll();

        return diaryInfoList.ToList();
    }
}
