namespace ProjectBaduk;

public class BadukService : IBadukService
{
    IFileSystem _fs;
    public BadukService(
        BadukOption option,
        IFileSystemService fsService)
    {
        _fs = fsService.GetFileSystem(option.FileSystemSelect, option.Path);
    }

    public async Task<BadukDiary> DeleteBadukGameData(DiaryName diaryName, string subject)
    {
        var diary = await GetBadukDiary(diaryName);
        diary.GameDataList = (diary.GameDataList ?? new())
            .Where(gameName => gameName != subject)
            .ToList();

        await _fs.WriteJsonAsync(path => DiaryFilePath(path, diaryName.Name), diary);
        await _fs.DeleteFileAsync(path => GameDataFilePath(path, diaryName, subject));

        return diary;
    }

    public async Task<BadukGameData> GetBadukGameData(DiaryName diaryName, string subject)
    {
        var gameData = await _fs.ReadJsonAsync<BadukGameData>(path => GameDataFilePath(path, diaryName, subject));

        return gameData;
    }

    public async Task<List<BadukGameData>> GetBadukSummaryList(DiaryName diaryName)
    {
        if (!await _fs.DirExistsAsync(path => GameDataSavePath(path, diaryName)))
        {
            await _fs.CreateDirectoryAsync(path => GameDataSavePath(path, diaryName));
        }
        var list = await _fs.GetFilesAsync(path => GameDataSavePath(path, diaryName));

        var gameDataList = await list
            .Select(fileName => Path.GetFileNameWithoutExtension(fileName))
            .Select(async file => await _fs.ReadJsonAsync<BadukGameData>(path => GameDataFilePath(path, diaryName, file)))
            .WhenAll();

        return gameDataList.OrderByDescending(x => x.CreateTime).ToList();
    }

    public async Task<BadukDiary> SaveBadukGameData(DiaryName diaryName, BadukGameData badukGameData)
    {
        if (string.IsNullOrEmpty(badukGameData.Subject.Trim()))
        {
            return null;
        }

        var diary = await GetBadukDiary(diaryName);
        var gameDataList = diary.GameDataList ?? new();
        if (!gameDataList.Contains(badukGameData.Subject))
        {
            gameDataList.Insert(0, badukGameData.Subject);
        }
        diary.GameDataList = gameDataList;

        await _fs.WriteJsonAsync(path => DiaryFilePath(path, diaryName.Name), diary);
        await _fs.WriteJsonAsync(path => GameDataFilePath(path, diaryName, badukGameData.Subject), badukGameData);

        return diary;
    }

    public async Task<BadukDiary> GetBadukDiary(DiaryName diaryName)
    {
        if (!await _fs.FileExistsAsync(path => DiaryFilePath(path, diaryName.Name)))
        {
            return null;
        }
        var diary = await _fs.ReadJsonAsync<BadukDiary>(path => DiaryFilePath(path, diaryName.Name));

        if (diary.GameDataList == null)
        {
            var list = await GetBadukSummaryList(diaryName);
            diary.GameDataList = list.Select(x => x.Subject).ToList();
            await _fs.WriteJsonAsync(path => DiaryFilePath(path, diaryName.Name), diary);
        }
        return diary;
    }

    public async Task<List<BadukDiary>> GetBadukDiaryList(AppUser user)
    {
        if (!await _fs.DirExistsAsync(path => path[BadukPathType.BadukDiaryPath]))
        {
            await _fs.CreateDirectoryAsync(path => path[BadukPathType.BadukDiaryPath]);
        }
        var list = await _fs.GetFilesAsync(path => path[BadukPathType.BadukDiaryPath]);

        var diaryList = await list
            .Select(fileName => Path.GetFileNameWithoutExtension(fileName))
            .Select(async file => await GetBadukDiary(new DiaryName(file)))
            .WhenAll();

        var result = diaryList.ToList()
            .Where(x => x.ConnectUserIdList?.Contains(user.Id) ?? false)
            .OrderBy(x => x.Name)
            .ToList();

        return result;
    }

    public async Task CreateBadukDiary(AppUser user, DiaryName diaryName)
    {
        var duplicated = await _fs.FileExistsAsync(path => DiaryFilePath(path, diaryName.Name));
        if (duplicated)
        {
            return;
        }
        var diaryData = new BadukDiary
        {
            Name = diaryName,
            OwnerUserId = user.Id,
            ConnectUserIdList = new() { user.Id },
        };
        await _fs.WriteJsonAsync(path => DiaryFilePath(path, diaryName.Name), diaryData);
    }

    public async Task DeleteBadukDiary(AppUser user, DiaryName diaryName)
    {
        var diaryList = await GetBadukDiaryList(user);

        var deleteDiary = diaryList?.Find(x => x.OwnerUserId == user.Id && x.Name == diaryName);

        if (deleteDiary != null)
        {
            await _fs.DeleteFileAsync(path => DiaryFilePath(path, diaryName.Name));
        }
    }

    private string GameDataSavePath(Paths path, DiaryName diaryName)
    {
        return path[BadukPathType.BadukSavePath] + "/" + diaryName;
    }

    private string GameDataFilePath(Paths path, DiaryName diaryName, string fileName)
    {
        var badukSavePath = path[BadukPathType.BadukSavePath];
        return $"{badukSavePath}/{diaryName}/{fileName}.json";
    }

    private string DiaryFilePath(Paths path, string diaryFileName)
    {
        return $"{path[BadukPathType.BadukDiaryPath]}/{diaryFileName}.json";
    }
}