namespace ProjectWorldCup;

public partial class BettingService : IBettingService
{
    object _usersCacheLock = new object();
    List<BettingUser> _bettingUsersCache;

    public void ClearUserCache()
    {
        lock (_usersCacheLock)
        {
            _bettingUsersCache = null;
        }
    }
    private async Task SaveUserAsync(BettingUser user)
    {
        Func<Paths, string> userPath = path => path[WorldCupPath.Betting2022Users] + @$"/{user.AppUser.Id}.json";

        lock (_usersCacheLock)
        {
            if (_bettingUsersCache == null)
            {

            }
            else if (_bettingUsersCache.Any(x => x.AppUser == user.AppUser))
            {
                var index = _bettingUsersCache.FindIndex(x => x.AppUser == user.AppUser);
                _bettingUsersCache[index] = user;
            }
            else
            {
                _bettingUsersCache.Add(user);
            }
        }
        await _fs.WriteJsonAsync(userPath, user);
    }
    public async Task<BettingUser> MakeJoinRequestAsync(AppUser appUser)
    {
        var user = await GetBettingUserAsync(appUser);
        user.JoinStatus = UserJoinStatus.Requested;
        user.BettingHistories ??= new();
        user.BettingHistories.Add(new BettingHistory
        {
            Type = HistoryType.JoinRequest,
            Comment = "참가 신청",
        });
        await SaveUserAsync(user);

        return user;
    }
    public async Task ApproveUserAsync(BettingUser user, int initValue, AppUser approveBy)
    {
        user.JoinStatus = UserJoinStatus.Joined;
        user.BettingHistories.Add(new BettingHistory
        {
            Type = HistoryType.JoinApproved,
            Value = initValue,
            Comment = $"{approveBy.DisplayName}님에 의해 승인",
        });
        await SaveUserAsync(user);
    }
    public async Task RejectUserAsync(BettingUser user, AppUser rejectBy)
    {
        user.JoinStatus = UserJoinStatus.Rejected;
        user.BettingHistories.Add(new BettingHistory
        {
            Type = HistoryType.JoinRejected,
            Comment = rejectBy == null ? "system에 의해 거절" : $"{rejectBy.DisplayName}님에 의해 거절",
        });
        await SaveUserAsync(user);
    }
    public async Task CancelJoinRequestAsync(AppUser appUser)
    {
        var user = await GetBettingUserAsync(appUser);
        user.JoinStatus = UserJoinStatus.None;
        user.BettingHistories.Add(new BettingHistory
        {
            Type = HistoryType.CancelJoinRequest,
            Comment = "참가 신청 취소",
        });
        await SaveUserAsync(user);
    }
    public async Task<BettingUser> GetBettingUserAsync(AppUser appUser)
    {
        lock (_usersCacheLock)
        {
            if (_bettingUsersCache != null)
            {
                var user = _bettingUsersCache.FirstOrDefault(user => user.AppUser == appUser);
                if (user != null)
                {
                    return user;
                }
            }
        }

        Func<Paths, string> userPath = path => path[WorldCupPath.Betting2022Users] + @$"/{appUser.Id}.json";
        if (await _fs.FileExistsAsync(userPath))
        {
            var bettingUser = await _fs.ReadJsonAsync<BettingUser>(userPath);
            return bettingUser;
        }
        else
        {
            return new BettingUser
            {
                AppUser = appUser,
            };
        }
    }
    public async Task<IEnumerable<BettingUser>> GetBettingUsersAsync()
    {
        lock (_usersCacheLock)
        {
            if (_bettingUsersCache != null)
            {
                return _bettingUsersCache;
            }
        }

        var userfiles = await _fs.GetFilesAsync(path => path[WorldCupPath.Betting2022Users]);
        var users = await userfiles
            .Select(filename => _fs.ReadJsonAsync<BettingUser>(path => path[WorldCupPath.Betting2022Users] + $@"/{filename}"))
            .WhenAll();

        lock (_usersCacheLock)
        {
            _bettingUsersCache = users.ToList();
        }
        return users;
    }
}
