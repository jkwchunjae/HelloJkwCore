using System.Text.RegularExpressions;

namespace ProjectDiary;

public partial class DiaryService : IDiaryService
{
    private async Task<UserDiaryInfo> CreateUserDiaryInfoAsync(AppUser user)
    {
        Func<Paths, string> path = pathof => pathof.UserDiaryInfo(user);
        if (await _fs.FileExistsAsync(path))
        {
            return await _fs.ReadJsonAsync<UserDiaryInfo>(path);
        }

        var userDiaryInfo = new UserDiaryInfo
        {
            UserId = user.Id,
        };

        var success = await _fs.WriteJsonAsync(path, userDiaryInfo);

        if (success)
        {
            return userDiaryInfo;
        }
        return null;
    }

    private async Task<bool> CheckAndAddDiaryNameAsync(DiaryName diaryName)
    {
        if (diaryName.Length < 3)
            return false;
        if (diaryName.Length > 30)
            return false;
        if (!Regex.IsMatch(diaryName.Name, @"^[a-z]+$"))
            return false;

        // TODO lock
        var diaryNameList = await _fs.ReadJsonAsync<List<DiaryName>>(path => path.DiaryNameListFile());
        if (diaryNameList == default(List<DiaryName>))
        {
            diaryNameList = new List<DiaryName>();
        }

        if (diaryNameList.Contains(diaryName))
        {
            return false;
        }

        diaryNameList.Add(diaryName);
        await _fs.WriteJsonAsync(path => path.DiaryNameListFile(), diaryNameList);

        return true;
    }

    public async Task<DiaryInfo> CreateDiaryInfoAsync(AppUser user, DiaryName diaryName, bool isSecret)
    {
        // 1. 일기장 이름 등록 
        if (!await CheckAndAddDiaryNameAsync(diaryName))
        {
            return null;
        }

        // 일단 만들자.
        var userDiaryInfo = await CreateUserDiaryInfoAsync(user);
        if (userDiaryInfo == null)
        {
            // 만들지 못했거나 읽기 실패
            return null;
        }

        userDiaryInfo.AddMyDiary(diaryName);
        await _fs.WriteJsonAsync(path => path.UserDiaryInfo(user), userDiaryInfo);

        var newDiary = new DiaryInfo(user.Id, diaryName, isSecret);
        await _fs.WriteJsonAsync(path => path.DiaryInfo(diaryName), newDiary);

        return newDiary;
    }

    public async Task<UserDiaryInfo> GetUserDiaryInfoAsync(AppUser user)
    {
        if (await _fs.FileExistsAsync(path => path.UserDiaryInfo(user)))
        {
            return await _fs.ReadJsonAsync<UserDiaryInfo>(path => path.UserDiaryInfo(user));
        }

        return null;
    }

    public async Task<DiaryInfo> GetDiaryInfoAsync(AppUser user, DiaryName diaryName)
    {
        var userDiaryInfo = await GetUserDiaryInfoAsync(user);

        if (userDiaryInfo?.IsViewable(diaryName) ?? false)
        {
            if (await _fs.FileExistsAsync(path => path.DiaryInfo(diaryName)))
            {
                return await _fs.ReadJsonAsync<DiaryInfo>(path => path.DiaryInfo(diaryName));
            }
        }

        return null;
    }

    public async Task<List<DiaryInfo>> GetWritableDiaryInfoAsync(AppUser user)
    {
        var userDiaryInfo = await GetUserDiaryInfoAsync(user);

        var writableList = userDiaryInfo.MyDiaryList
            .Concat(userDiaryInfo.WriterList);

        var writableDiaryList = await writableList
            .Select(async diaryName => await GetDiaryInfoAsync(user, diaryName))
            .WhenAll();

        return writableDiaryList.ToList();
    }

    public async Task<List<DiaryInfo>> GetViewableDiaryInfoAsync(AppUser user)
    {
        var userDiaryInfo = await GetUserDiaryInfoAsync(user);

        var viewableList = userDiaryInfo.MyDiaryList
            .Concat(userDiaryInfo.WriterList)
            .Concat(userDiaryInfo.ViewList);

        var viewableDiaryList = await viewableList
            .Select(async diaryName => await GetDiaryInfoAsync(user, diaryName))
            .WhenAll();

        return viewableDiaryList.ToList();
    }

    public async Task UpdateDiaryInfoAsync(AppUser user, DiaryInfo diaryInfo)
    {
        if (diaryInfo.CanManage(user.Id) || user.HasRole(UserRole.Admin))
        {
            await _fs.WriteJsonAsync(path => path.DiaryInfo(diaryInfo.DiaryName), diaryInfo);
        }
    }
}
