﻿namespace ProjectWorldCup;

public class BettingGroupStageService : IBettingGroupStageService
{
    private readonly IFileSystem _fs;
    /// <summary> for _cache </summary>
    private object _lock = new object();
    private List<WcBettingItem<GroupTeam>> _cache = null;
    private IWorldCupService _worldCupService;
    private System.Timers.Timer _timer;
    private readonly DateTime _gameStartTime = WorldCupConst.GroupStageStartTime;

    public BettingGroupStageService(
        IFileSystemService fsService,
        IWorldCupService worldCupService,
        ICacheClearInvoker cacheClearInvoker,
        WorldCupOption option)
    {
        _fs = fsService.GetFileSystem(option.FileSystemSelect, option.Path);
        _worldCupService = worldCupService;

        _timer = new System.Timers.Timer(TimeSpan.FromMinutes(10).TotalMilliseconds);
        _timer.Elapsed += async (s, e) => await UpdateStandingsAsync();
        _timer.AutoReset = true;
        _timer.Start();

        cacheClearInvoker.ClearCacheInvoked += (_, _) =>
        {
            lock (_lock)
            {
                _cache = null;
            }
        };
    }

    private async Task UpdateStandingsAsync()
    {
        if (DateTime.Now < WorldCupConst.WorldCupStartTime)
            return;

        var groups = await _worldCupService.GetGroupsAsync();
        var team16 = groups
            .SelectMany(group => group.Stands.Take(2))
            .Select(s => s.Team)
            .ToList();
        var bettingItems = await GetAllBettingsAsync();
        bettingItems.ForEach(bettingItem => bettingItem.Fixed = team16);

        // 객체를 생성하면서 Reward를 계산한다.
        var result = new BettingResultTable<WcBettingItem<GroupTeam>>(bettingItems);

        await result
            .Select(async item =>
            {
                await SaveBettingItemAsync(item);
            })
            .WhenAll();
    }

    public int GetRemainSeconds()
    {
        return (int)(_gameStartTime - DateTime.Now).TotalSeconds;
    }

    public async ValueTask<List<WcBettingItem<GroupTeam>>> GetAllBettingsAsync()
    {
        lock (_lock)
        {
            if (_cache != null)
            {
                return _cache;
            }
        }

        var bettingItems = await _fs.ReadAllBettingItemsAsync<WcBettingItem<GroupTeam>, GroupTeam>(BettingType.GroupStage);
        lock (_lock)
        {
            _cache = bettingItems;
        }
        return bettingItems;
    }

    public async Task<WcBettingItem<GroupTeam>> GetBettingAsync(BettingUser user)
    {
        lock (_lock)
        {
            if (_cache?.Any(x => x.User == user.AppUser) ?? false)
            {
                return _cache.First(x => x.User == user.AppUser);
            }
        }
        var bettingItem = await _fs.ReadBettingItemAsync<WcBettingItem<GroupTeam>, GroupTeam>(BettingType.GroupStage, user.AppUser);
        return bettingItem;
    }

    public async Task<WcBettingItem<GroupTeam>> PickTeamAsync(BettingUser user, GroupTeam team)
    {
        if (user.JoinStatus != UserJoinStatus.Joined)
        {
            throw new NotJoinedException();
        }
        if (!(user.JoinedBetting?.Contains(BettingType.GroupStage) ?? false))
        {
            throw new NotJoinedException();
        }
        var bettingItem = await GetBettingAsync(user)
            ?? new WcBettingItem<GroupTeam>
            {
                User = user.AppUser,
            };

        if (bettingItem.Picked.Empty(picked => picked == team))
        {
            bettingItem.Picked = bettingItem.Picked
                .Concat(new[] { team })
                .OrderBy(x => x.GroupName)
                .ThenBy(x => x.Placement)
                .ToList();
            await SaveBettingItemAsync(bettingItem);
        }
        return bettingItem;
    }

    public async Task<WcBettingItem<GroupTeam>> UnpickTeamAsync(BettingUser user, GroupTeam team)
    {
        var bettingItem = await GetBettingAsync(user);
        if (bettingItem == null)
            return null;

        if (bettingItem.Picked.Any(picked => picked == team))
        {
            bettingItem.Picked = bettingItem.Picked
                .Where(picked => picked != team)
                .OrderBy(x => x.GroupName)
                .ThenBy(x => x.Placement)
                .ToList();
            await SaveBettingItemAsync(bettingItem);
        }
        return bettingItem;
    }

    private async Task SaveBettingItemAsync(WcBettingItem<GroupTeam> item)
    {
        await _fs.WriteBettingItemAsync(BettingType.GroupStage, item);

        if (_cache?.Any() ?? false)
        {
            lock (_lock)
            {
                var index = _cache.FindIndex(x => x.User == item.User);
                if (index >= 0)
                {
                    _cache[index] = item;
                }
                else
                {
                    _cache.Add(item);
                }
            }
        }
    }

    public async Task<WcBettingItem<GroupTeam>> PickRandomAsync(BettingUser user)
    {
        if (user.JoinStatus != UserJoinStatus.Joined)
        {
            throw new NotJoinedException();
        }
        if (!(user.JoinedBetting?.Contains(BettingType.GroupStage) ?? false))
        {
            throw new NotJoinedException();
        }
        var bettingItem = await GetBettingAsync(user)
            ?? new WcBettingItem<GroupTeam>
            {
                User = user.AppUser,
            };

        var groups = await _worldCupService.GetGroupsAsync();
        var pickTeam = groups
            .SelectMany(group => group.Teams.RandomShuffle().Take(2))
            .ToList();

        bettingItem.IsRandom = true;
        bettingItem.Picked = pickTeam
          .OrderBy(x => x.GroupName)
          .ThenBy(x => x.Placement)
          .ToList();
        await SaveBettingItemAsync(bettingItem);
        return bettingItem;
    }
}
