namespace ProjectDiary;

public partial class DiaryService : IDiaryService
{
    private readonly IFileSystem _fs;
    private readonly IDiarySearchService _diarySearchService;
    private readonly Dictionary<DiaryName, List<DiaryFileName>> _filesCache = new();
    private readonly IBackgroundTaskQueue _backgroundQueue;

    public DiaryService(
        DiaryOption option,
        IBackgroundTaskQueue backgroundQueue,
        IDiarySearchService diarySearchService,
        IFileSystemService fsService)
    {
        _backgroundQueue = backgroundQueue;
        _diarySearchService = diarySearchService;
        _fs = fsService.GetFileSystem(option.FileSystemSelect, option.Path);
    }

    private async Task<List<DiaryFileName>> GetDiaryListAsync(DiaryName diaryName)
    {
        lock (_filesCache)
        {
            if (_filesCache.TryGetValue(diaryName, out var list))
            {
                return list;
            }
        }

        Func<Paths, string> diaryPath = path => path.Diary(diaryName);

        if (!(await _fs.DirExistsAsync(diaryPath)))
        {
            await _fs.CreateDirectoryAsync(diaryPath);
        }

        var files = await _fs.GetFilesAsync(diaryPath, ".diary");

        var diaryFileNameList = files
            .OrderBy(x => x)
            .Where(x => x.Left(8).TryToDate(out var _))
            .Select(x => new DiaryFileName(x))
            .ToList();

        lock (_filesCache)
        {
            _filesCache[diaryName] = diaryFileNameList;
        }

        return diaryFileNameList;
    }

    private void SaveDiaryCache(DiaryName diaryName, List<DiaryFileName> list)
    {
        lock (_filesCache)
        {
            _filesCache[diaryName] = list;
        }
    }

    public async Task<DiaryContent> WriteDiaryAsync(AppUser user, DiaryInfo diary, DateTime date, string text)
    {
        return await WriteDiaryAsync(user, diary, date, text, false);
    }

    public async Task<DiaryContent> WriteDiaryAsync(AppUser user, DiaryInfo diary, DateTime date, string text, string password)
    {
        var cipherText = text.Encrypt(password);
        return await WriteDiaryAsync(user, diary, date, cipherText, true);
    }

    private async Task<DiaryContent> WriteDiaryAsync(AppUser user, DiaryInfo diary, DateTime date, string text, bool isSecret)
    {
        var list = await GetDiaryListAsync(diary.DiaryName);

        var content = new DiaryContent
        {
            Date = date,
            Text = text,
            IsSecret = isSecret,
            RegDate = DateTime.Now,
            LastModifyDate = DateTime.Now,
            Index = MakeNewIndex(list, date),
        };

        Func<Paths, string> diaryPath = path => path.Content(diary.DiaryName, content.GetFileName());
        await _fs.WriteJsonAsync(diaryPath, content);

        var diaryFileName = new DiaryFileName(content.GetFileName());
        list.Add(diaryFileName);

        list = list.OrderBy(x => x).ToList();

        SaveDiaryCache(diary.DiaryName, list);

        if (!diary.IsSecret)
        {
            _backgroundQueue?.QueueBackgroundWorkItem(async token =>
            {
                await _diarySearchService?.AppendDiaryTextAsync(diary.DiaryName, diaryFileName, text);
                await _diarySearchService?.SaveDiaryTrie(diary.DiaryName, date.Year);
            });
        }

        return content;

    }

    private int MakeNewIndex(List<DiaryFileName> fileList, DateTime date)
    {
        var today = fileList.Where(x => x.Date == date).ToList();
        if (today.Any())
        {
            return today.Max(x => x.Index) + 1;
        }
        else
        {
            return 1;
        }
    }

    public async Task<List<DiaryContent>> UpdateDiaryAsync(AppUser user, DiaryInfo diary, List<DiaryContent> contents)
    {
        var list = await GetDiaryListAsync(diary.DiaryName);

        var deleteFiles = contents.Where(x => string.IsNullOrWhiteSpace(x.Text)).ToList();
        var updateFiles = contents.Where(x => !string.IsNullOrWhiteSpace(x.Text)).ToList();

        foreach (var deleteFile in deleteFiles)
        {
            Func<Paths, string> deleteFilePath = path => path.Content(diary.DiaryName, deleteFile.GetFileName());
            await _fs.DeleteFileAsync(deleteFilePath);
            list.Remove(new DiaryFileName(deleteFile.GetFileName()));
        }

        foreach (var updateFile in updateFiles)
        {
            Func<Paths, string> updateFilePath = path => path.Content(diary.DiaryName, updateFile.GetFileName());
            await _fs.WriteJsonAsync(updateFilePath, updateFile);
        }

        SaveDiaryCache(diary.DiaryName, list);

        return updateFiles;
    }

    public async Task<List<DiaryContent>> UpdateDiaryAsync(AppUser user, DiaryInfo diary, List<DiaryContent> contents, string password)
    {
        foreach (var content in contents.Where(x => x.IsSecret))
        {
            content.Text = content.Text.Encrypt(password);
        }

        return await UpdateDiaryAsync(user, diary, contents);
    }
}