﻿using Common;
using Microsoft.AspNetCore.Identity;

namespace ProjectWorldCup;

public partial class BettingService : IBettingService
{
    object _usersCacheLock = new object();
    List<BettingUser> _bettingUsersCache;
    private readonly IUserStore<AppUser> _userStore;

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
            Value = 20000,
        });
        await SaveUserAsync(user);

        return user;
    }
    public async Task SetRequestStateAsync(BettingUser user, AppUser operateBy)
    {
        user.JoinStatus = UserJoinStatus.Requested;
        user.BettingHistories.Add(new BettingHistory
        {
            Type = HistoryType.None,
            Comment = $"{operateBy.DisplayName}님에 의해 신청상태로 변경",
        });
        await SaveUserAsync(user);
    }
    public async Task ApproveUserAsync(BettingUser user, int initValue, AppUser approveBy)
    {
        user.JoinStatus = UserJoinStatus.Joined;
        user.BettingHistories.Add(new BettingHistory
        {
            Type = HistoryType.JoinApproved,
            // Value = initValue,
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
    public async Task<BettingUser> AddHistoryAsync(BettingUser user, BettingHistory history)
    {
        user.BettingHistories.Add(history);
        await SaveUserAsync(user);
        return user;
    }
    public async Task<BettingUser> DeleteHistoryAsync(BettingUser user, BettingHistory history)
    {
        user.BettingHistories.Remove(history);
        await SaveUserAsync(user);
        return user;
    }
    public async Task<BettingUser> GetBettingUserAsync(AppUser appUser)
    {
        if (appUser == null)
        {
            return null;
        }
        lock (_usersCacheLock)
        {
            if (_bettingUsersCache != null)
            {
                var user = _bettingUsersCache.FirstOrDefault(user => user.AppUser == appUser);
                if (user != null)
                {
                    user.AppUser = appUser;
                    return user;
                }
            }
        }

        Func<Paths, string> userPath = path => path[WorldCupPath.Betting2022Users] + @$"/{appUser.Id}.json";
        if (await _fs.FileExistsAsync(userPath))
        {
            var bettingUser = await _fs.ReadJsonAsync<BettingUser>(userPath);
            bettingUser.AppUser = appUser;
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
    public async Task<IEnumerable<BettingUser>> GetBettingUsersAsync(bool updateAppUser = false)
    {
        IEnumerable<BettingUser> users = null;
        lock (_usersCacheLock)
        {
            if (_bettingUsersCache != null)
            {
                users = _bettingUsersCache;
            }
        }
        if (users != null)
        {
            return updateAppUser ? await FillAppUsers(users) : users;
        }

        var userfiles = await _fs.GetFilesAsync(path => path[WorldCupPath.Betting2022Users]);
        users = await userfiles
            .Select(filename => _fs.ReadJsonAsync<BettingUser>(path => path[WorldCupPath.Betting2022Users] + $@"/{filename}"))
            .WhenAll();
        if (updateAppUser)
        {
            users = await FillAppUsers(users);
        }

        lock (_usersCacheLock)
        {
            _bettingUsersCache = users.ToList();
        }
        return users;

        async Task<IEnumerable<BettingUser>> FillAppUsers(IEnumerable<BettingUser> bettingUsers)
        {
            return await bettingUsers
                .Select(async user =>
                {
                    user.AppUser = await _userStore.FindByIdAsync(user.AppUser.Id.ToString(), default);
                    return user;
                })
                .WhenAll();
        }
    }

    public async Task<BettingUser> AddRewardAsync(BettingUser user, HistoryType rewardType, long reward)
    {
        var bettingName =
              rewardType == HistoryType.Reward1 ? "16강 진출팀 맞추기"
            : rewardType == HistoryType.Reward2 ? "8강 진출팀 맞추기"
            : rewardType == HistoryType.Reward3 ? "우승팀 맞추기"
            : string.Empty;
        var resultUrl =
              rewardType == HistoryType.Reward1 ? "group-stage"
            : rewardType == HistoryType.Reward2 ? "round16"
            : rewardType == HistoryType.Reward3 ? "final"
            : string.Empty;
        if (user.BettingHistories.Any(x => x.Type == rewardType))
        {
            var rewardHistory = user.BettingHistories.First(x => x.Type == rewardType);
            rewardHistory.Value = reward;
            rewardHistory.Comment = $"{bettingName} 내기 결과: {reward:#,#}";
        }
        else
        {
            user.BettingHistories.Add(new BettingHistory
            {
                Type = rewardType,
                Value = reward,
                ResultUrl = $"/worldcup/2022/result/{resultUrl}",
                Comment = $"'{bettingName}' 내기 결과: {reward:#,#}",
            });
        }
        await SaveUserAsync(user);

        return user;
    }
}
