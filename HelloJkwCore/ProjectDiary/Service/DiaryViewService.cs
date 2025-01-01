namespace ProjectDiary;

public partial class DiaryService : IDiaryService
{
    private async Task<DiaryContent> GetDiaryContentAsync(DiaryName diaryName, DiaryFileName fileName)
    {
        return await _fs.ReadJsonAsync<DiaryContent>(path => path.Content(diaryName, fileName.ToString()));
    }

    public async Task<DiaryContent> GetDiaryContentAsync(AppUser user, DiaryInfo diary, DiaryFileName diaryFileName)
    {
        var force = false;
        if (user.HasRole(UserRole.Admin))
            force = true;

        if (!force && !diary.CanRead(user?.Id))
            return null;

        return await GetDiaryContentAsync(diary.DiaryName, diaryFileName);
    }

    public async Task<DiaryView> GetLastDiaryViewAsync(AppUser user, DiaryInfo diary)
    {
        var list = await GetDiaryListAsync(diary.DiaryName);

        if (list.Empty())
        {
            return default;
        }

        DateTime lastDate = list.Last().Date;

        return await GetDiaryViewAsync(user, diary, lastDate);
    }

    public async Task<DiaryView> GetDiaryViewAsync(AppUser user, DiaryInfo diary, DateTime date)
    {
        var list = await GetDiaryListAsync(diary.DiaryName);

        var dates = list
            .Select(x => x.Date)
            .Distinct()
            .OrderBy(x => x)
            .ToList();

        var before = dates.Where(x => x.Date < date.Date).ToList();
        var after = dates.Where(x => x.Date > date.Date).ToList();
        DateTime? prevDate = before.Any() ? before.Last() : null;
        DateTime? nextDate = after.Any() ? after.First() : null;

        var todayContents = await list
            .Where(x => x.Date == date.Date)
            .Select(async fileName => await GetDiaryContentAsync(user, diary, fileName))
            .WhenAll();

        var pictures = await todayContents
            .Where(x => x?.Pictures?.Any() ?? false)
            .SelectMany(x => x.Pictures)
            .Select(async fileName =>
            {
                Func<Paths, string> picturePath = path => path.Picture(diary.DiaryName, fileName);
                if (_fs is AzureFileSystem azureFs)
                {
                    var url = await azureFs.GenerateSasUrlAsync(picturePath);
                    return url;
                } else {
                    return string.Empty;
                }
            })
            .WhenAll();

        return new DiaryView
        {
            DiaryInfo = diary,
            DiaryContents = todayContents.Where(x => x != null).ToList(),
            PicturesUrl = pictures.Where(p => !string.IsNullOrEmpty(p)).ToList(),
            DiaryNavigationData = new DiaryNavigationData
            {
                Today = date.Date,
                PrevDate = prevDate,
                NextDate = nextDate,
            },
        };
    }

    public async Task<List<DiaryFileName>> GetDiaryFileAllAsync(AppUser user, DiaryInfo diary)
    {
        var list = await GetDiaryListAsync(diary.DiaryName);

        return list;
    }
}
