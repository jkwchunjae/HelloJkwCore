using Microsoft.Extensions.DependencyInjection;

namespace ProjectBaduk;

public class BadukService : IBadukService
{
    private readonly IFileSystem _fs;
    public BadukService(
        [FromKeyedServices(nameof(BadukService))] IFileSystem fileSystem
    )
    {
        _fs = fileSystem;
    }

    public async Task<BadukDiary> DeleteBadukGameData(BadukDiaryName diaryName, string subject)
    {
        var diary = await GetBadukDiary(diaryName);
        diary.GameDataList = (diary.GameDataList ?? new())
            .Where(gameName => gameName != subject)
            .ToList();

        await _fs.WriteJsonAsync(path => DiaryFilePath(path, diaryName), diary);
        await _fs.DeleteFileAsync(path => GameDataFilePath(path, diaryName, subject));

        return diary;
    }

    public async Task<BadukGameData> GetBadukGameData(BadukDiaryName diaryName, string subject)
    {
        var gameData = await _fs.ReadJsonAsync<BadukGameData>(path => GameDataFilePath(path, diaryName, subject));

        return gameData;
    }

    public async Task<List<BadukGameData>> GetBadukSummaryList(BadukDiaryName diaryName)
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

    public async Task<BadukDiary> SaveBadukGameData(BadukDiaryName diaryName, BadukGameData badukGameData)
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

        await _fs.WriteJsonAsync(path => DiaryFilePath(path, diaryName), diary);
        await _fs.WriteJsonAsync(path => GameDataFilePath(path, diaryName, badukGameData.Subject), badukGameData);

        return diary;
    }

    public async Task<BadukDiary> GetBadukDiary(BadukDiaryName diaryName)
    {
        if (!await _fs.FileExistsAsync(path => DiaryFilePath(path, diaryName)))
        {
            return null;
        }
        var diary = await _fs.ReadJsonAsync<BadukDiary>(path => DiaryFilePath(path, diaryName));

        if (diary.GameDataList == null)
        {
            var list = await GetBadukSummaryList(diaryName);
            diary.GameDataList = list.Select(x => x.Subject).ToList();
            await _fs.WriteJsonAsync(path => DiaryFilePath(path, diaryName), diary);
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
            .Select(async file => await GetBadukDiary(new BadukDiaryName(file)))
            .WhenAll();

        var result = diaryList.ToList()
            .Where(x => x.ConnectUserIdList?.Contains(user.Id) ?? false)
            .OrderBy(x => x)
            .ToList();

        return result;
    }

    public async Task CreateBadukDiary(AppUser user, BadukDiaryName diaryName)
    {
        var duplicated = await _fs.FileExistsAsync(path => DiaryFilePath(path, diaryName));
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
        await _fs.WriteJsonAsync(path => DiaryFilePath(path, diaryName), diaryData);
    }

    public async Task DeleteBadukDiary(AppUser user, BadukDiaryName diaryName)
    {
        var diaryList = await GetBadukDiaryList(user);

        var deleteDiary = diaryList?.Find(x => x.OwnerUserId == user.Id && x.Name == diaryName);

        if (deleteDiary != null)
        {
            await _fs.DeleteFileAsync(path => DiaryFilePath(path, diaryName));
        }
    }

    private string GameDataSavePath(Paths path, BadukDiaryName diaryName)
    {
        return path[BadukPathType.BadukSavePath] + "/" + diaryName;
    }

    private string GameDataFilePath(Paths path, BadukDiaryName diaryName, string fileName)
    {
        var badukSavePath = path[BadukPathType.BadukSavePath];
        return $"{badukSavePath}/{diaryName}/{fileName}.json";
    }

    private string DiaryFilePath(Paths path, string diaryFileName)
    {
        return $"{path[BadukPathType.BadukDiaryPath]}/{diaryFileName}.json";
    }
}