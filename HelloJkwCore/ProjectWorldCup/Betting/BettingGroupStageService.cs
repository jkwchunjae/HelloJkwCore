namespace ProjectWorldCup;

public class BettingGroupStageService : IBettingGroupStageService
{
    private readonly WorldcupBettingFileSystem<WcBettingItem<GroupTeam>, GroupTeam> _fs;
    private readonly Func<IWorldCupService, Task<List<WcGroup>>> _getGroupsFunc;
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
        WorldCupOption option,
        string pathKey,
        Func<IWorldCupService, Task<List<WcGroup>>> getGroupsFunc)
    {
        _getGroupsFunc = getGroupsFunc;
        _fs = new WorldcupBettingFileSystem<WcBettingItem<GroupTeam>, GroupTeam>(fsService.GetFileSystem(option.FileSystemSelect, option.Path), pathKey);
        _worldCupService = worldCupService;

        _timer = new System.Timers.Timer(TimeSpan.FromMinutes(10));
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

    /// <summary>
    /// 10분마다 피파에서 데이터를 데이터를 읽어서 유저 정보를 업데이트 한다. <br/>
    /// 순위표 까지 매번 계산한다.
    /// </summary>
    /// <returns></returns>
    private async Task UpdateStandingsAsync()
    {
        if (DateTime.Now < WorldCupConst.WorldCupStartTime)
            return;

        var groups = await _getGroupsFunc(_worldCupService);
        var teamsTop2 = groups
            .SelectMany(group => group.Stands.Take(2))
            .Select(s => s.Team)
            .ToList();
        var bettingItems = await GetAllBettingsAsync();
        bettingItems.ForEach(bettingItem => bettingItem.Fixed = teamsTop2);

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

        var bettingItems = await _fs.ReadAllBettingItemsAsync(BettingType.GroupStage);
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
        var bettingItem = await _fs.ReadBettingItemAsync(BettingType.GroupStage, user.AppUser);
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
                .Append(team)
                .OrderBy(x => x.GroupName)
                .ThenBy(x => x.Placement)
                .ToList();
            await SaveBettingItemAsync(bettingItem);
        }
        return bettingItem;
    }

    public async Task<WcBettingItem<GroupTeam>> PickTeamsAsync(BettingUser user, IEnumerable<GroupTeam> teams)
    {
        if (user.JoinStatus != UserJoinStatus.Joined)
        {
            throw new NotJoinedException();
        }
        if (!(user.JoinedBetting?.Contains(BettingType.GroupStage) ?? false))
        {
            throw new NotJoinedException();
        }

        var picks = teams?.ToList() ?? new List<GroupTeam>();
        var groups = await _getGroupsFunc(_worldCupService);
        var allTeams = groups
            .SelectMany(group => group.Teams)
            .ToList();

        if (picks.Count != 24)
        {
            throw new InvalidOperationException("조별리그는 총 24팀을 선택해야 합니다.");
        }
        if (picks.Distinct().Count() != picks.Count)
        {
            throw new InvalidOperationException("중복 선택된 팀이 있습니다.");
        }
        if (picks.Any(pick => allTeams.Empty(team => team == pick)))
        {
            throw new InvalidOperationException("조별리그에 없는 팀이 포함되어 있습니다.");
        }
        if (groups.Any(group => group.Teams.Count(team => picks.Any(pick => pick == team)) != 2))
        {
            throw new InvalidOperationException("각 조에서 정확히 2팀씩 선택해야 합니다.");
        }

        var bettingItem = await GetBettingAsync(user)
            ?? new WcBettingItem<GroupTeam>
            {
                User = user.AppUser,
            };

        if (bettingItem.IsRandom)
        {
            throw new InvalidOperationException("랜덤 선택 이후에는 다시 선택 할 수 없습니다.");
        }

        bettingItem.IsRandom = false;
        bettingItem.Picked = picks
            .OrderBy(x => x.GroupName)
            .ThenBy(x => x.Placement)
            .ToList();

        await SaveBettingItemAsync(bettingItem);
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

        var groups = await _getGroupsFunc(_worldCupService);
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
