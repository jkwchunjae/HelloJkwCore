using System.Text.RegularExpressions;

namespace ProjectDiary;

public partial class DiaryService : IDiaryService
{
    public async Task<UserDiaryInfo> GetOrCreateUserDiaryInfoAsync(AppUser user)
    {
        Func<Paths, string> userDiaryPath = path => path.UserDiaryInfo(user);
        if (await _fs.FileExistsAsync(userDiaryPath))
        {
            return await _fs.ReadJsonAsync<UserDiaryInfo>(userDiaryPath);
        }

        var userDiaryInfo = new UserDiaryInfo
        {
            UserId = user.Id,
        };

        var success = await _fs.WriteJsonAsync(userDiaryPath, userDiaryInfo);

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
        if (!Regex.IsMatch(diaryName, @"^[a-z]+$"))
            return false;

        // TODO lock
        var diaryNameList = await GetDiaryNameListAsync();
        if (diaryNameList.Contains(diaryName))
        {
            return false;
        }

        diaryNameList.Add(diaryName);
        await _fs.WriteJsonAsync(path => path.DiaryNameListFile(), diaryNameList);

        return true;

        async Task<List<DiaryName>> GetDiaryNameListAsync()
        {
            try
            {
                var diaryNameList = await _fs.ReadJsonAsync<List<DiaryName>>(path => path.DiaryNameListFile());
                return diaryNameList;
            }
            catch (FileNotFoundException)
            {
                return new();
            }
        }
    }

    public async Task<DiaryInfo> CreateDiaryInfoAsync(AppUser user, DiaryName diaryName, bool isSecret)
    {
        // 1. 일기장 이름 등록 
        if (!await CheckAndAddDiaryNameAsync(diaryName))
        {
            return null;
        }

        // 일단 만들자.
        var userDiaryInfo = await GetOrCreateUserDiaryInfoAsync(user);
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

    public async Task<(bool, DiaryInfo)> UpdateDiaryInfoAsync(AppUser owner, DiaryName diaryName, Func<DiaryInfo, bool> updator)
    {
        var diaryInfo = await GetDiaryInfoAsync(owner, diaryName);

        if (diaryInfo == null)
            return (false, null);

        if (diaryInfo.CanManage(owner.Id))
        {
            if (updator(diaryInfo))
            {
                await _fs.WriteJsonAsync(path => path.DiaryInfo(diaryInfo.DiaryName), diaryInfo);
                return (true, diaryInfo);
            }
        }

        return (false, null);
    }

    public async Task<(bool, UserDiaryInfo)> UpdateUserDiaryInfoAsync(AppUser user, Func<UserDiaryInfo, bool> updator)
    {
        var userDiaryInfo = await GetOrCreateUserDiaryInfoAsync(user);

        if (userDiaryInfo == null)
            return (false, null);

        if (updator(userDiaryInfo))
        {
            await _fs.WriteJsonAsync(path => path.UserDiaryInfo(user), userDiaryInfo);
            return (true, userDiaryInfo);
        }

        return (false, null);
    }
}
