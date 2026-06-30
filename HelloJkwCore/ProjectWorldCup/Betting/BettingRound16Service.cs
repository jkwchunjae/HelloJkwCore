namespace ProjectWorldCup;

/// <summary>
/// 32강과 16강을 모두 처리하는 서비스.
/// </summary>
public class BettingRound16Service : IBettingRound16Service
{
    private readonly WorldcupBettingFileSystem<WcBettingItem<Team>, Team> _fs;
    private readonly BettingType _bettingType;
    private readonly Func<IWorldCupService, Task<List<KnMatch>>> _getMatchesFunc;
    private readonly DateTime _startTime;
    private readonly string _currentStageId;
    private readonly string _winnersStageId;
    private object _lock = new object();
    private List<WcBettingItem<Team>> _cache = null;
    private IWorldCupService _worldCupService;
    private System.Timers.Timer _timer;

    public BettingRound16Service(
        IFileSystemService fsService,
        IWorldCupService worldCupService,
        ICacheClearInvoker cacheClearInvoker,
        WorldCupOption option,
        string pathKey,
        BettingType bettingType,
        Func<IWorldCupService, Task<List<KnMatch>>> getMatchesFunc,
        DateTime startTime,
        string currentStageId,
        string winnersStageId)
    {
        _bettingType = bettingType;
        _getMatchesFunc = getMatchesFunc;
        _startTime = startTime;
        _currentStageId = currentStageId;
        _winnersStageId = winnersStageId;
        _fs = new WorldcupBettingFileSystem<WcBettingItem<Team>, Team>(fsService.GetFileSystem(option.FileSystemSelect, option.Path), pathKey);
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
        if (DateTime.Now < _startTime)
            return;

        var matches = await _worldCupService.GetKnockoutStageMatchesAsync();
        var winners = matches
            .Where(match => match.StageId == _winnersStageId)
            .SelectMany(match => match.Teams)
            .Where(team => team != null)
            .ToList();
        var losers = matches
            .Where(match => match.StageId == _currentStageId)
            .Where(match => winners.Any(w => w.Id == match.HomeTeam.Id || w.Id == match.AwayTeam.Id))
            .SelectMany(match => match.Teams)
            .Where(team => winners.All(w => w.Id != team.Id))
            .ToList();
        var bettingItems = await GetAllBettingsAsync();
        bettingItems.ForEach(bettingItem =>
        {
            bettingItem.Fixed = winners;
            bettingItem.Failed = losers;
        });
        var result = new BettingResultTable<WcBettingItem<Team>>(bettingItems);

        await result
            .Select(async item =>
            {
                await SaveBettingItemAsync(item);
            })
            .WhenAll();
    }

    public async ValueTask<List<WcBettingItem<Team>>> GetAllBettingsAsync()
    {
        lock (_lock)
        {
            if (_cache != null)
            {
                return _cache;
            }
        }

        var bettingItems = await _fs.ReadAllBettingItemsAsync(_bettingType);
        lock (_lock)
        {
            _cache = bettingItems;
        }
        return bettingItems;
    }

    public async Task<WcBettingItem<Team>> GetBettingAsync(BettingUser user)
    {
        lock (_lock)
        {
            if (_cache?.Any(x => x.User == user.AppUser) ?? false)
            {
                return _cache.First(x => x.User == user.AppUser);
            }
        }
        var bettingItem = await _fs.ReadBettingItemAsync(_bettingType, user.AppUser);
        return bettingItem;
    }

    public async Task<WcBettingItem<Team>> PickTeamAsync(BettingUser user, Team team)
    {
        if (user.JoinStatus != UserJoinStatus.Joined)
        {
            throw new NotJoinedException();
        }
        if (!(user.JoinedBetting?.Contains(_bettingType) ?? false))
        {
            throw new NotJoinedException();
        }

        var bettingItem = await GetBettingAsync(user)
            ?? new WcBettingItem<Team>
            {
                User = user.AppUser,
            };

        if (team == null)
            return bettingItem;

        if (bettingItem.Picked.Empty(picked => picked == team))
        {
            var matches = await _getMatchesFunc(_worldCupService);
            var match = matches?.FirstOrDefault(m => m.HomeTeam == team || m.AwayTeam == team);

            if (match == null)
                return bettingItem;

            bettingItem.Picked = bettingItem.Picked
                .Where(picked => match.HomeTeam != picked && match.AwayTeam != picked)
                .Concat(new[] { team })
                .OrderBy(picked => matches.FirstOrDefault(m => m.HomeTeam == picked || m.AwayTeam == picked).Time)
                .ToList();
            bettingItem.IsAi = false;
            await SaveBettingItemAsync(bettingItem);
        }
        return bettingItem;
    }

    public async Task<WcBettingItem<Team>> PickTeamsWithAiAsync(BettingUser user, IEnumerable<Team> teams)
    {
        if (user.JoinStatus != UserJoinStatus.Joined)
        {
            throw new NotJoinedException();
        }
        if (!(user.JoinedBetting?.Contains(_bettingType) ?? false))
        {
            throw new NotJoinedException();
        }

        var picks = teams?.ToList() ?? new List<Team>();
        var matches = await _getMatchesFunc(_worldCupService) ?? new List<KnMatch>();

        if (!matches.Any())
        {
            throw new InvalidOperationException("매치 정보를 불러오지 못했습니다.");
        }
        if (picks.Any(pick => pick == null))
        {
            throw new InvalidOperationException("선택된 팀 목록에 빈 팀이 있습니다.");
        }
        if (picks.Any(pick => string.IsNullOrWhiteSpace(pick.Id)))
        {
            throw new InvalidOperationException("선택된 팀 목록에 확정되지 않은 팀이 있습니다.");
        }
        if (picks.Count != matches.Count)
        {
            throw new InvalidOperationException($"총 {matches.Count}팀을 선택해야 합니다.");
        }
        if (matches.Any(match =>
            match.HomeTeam == null || match.AwayTeam == null
            || string.IsNullOrWhiteSpace(match.HomeTeam.Id)
            || string.IsNullOrWhiteSpace(match.AwayTeam.Id)))
        {
            throw new InvalidOperationException("아직 모든 매치의 팀이 확정되지 않았습니다.");
        }
        if (picks.Distinct().Count() != picks.Count)
        {
            throw new InvalidOperationException("중복 선택된 팀이 있습니다.");
        }
        if (matches.Any(match => picks.Count(pick => match.HomeTeam == pick || match.AwayTeam == pick) != 1))
        {
            throw new InvalidOperationException("각 경기에서 정확히 1팀씩 선택해야 합니다.");
        }

        var bettingItem = await GetBettingAsync(user)
            ?? new WcBettingItem<Team>
            {
                User = user.AppUser,
            };

        if (bettingItem.IsRandom)
        {
            throw new InvalidOperationException("랜덤 선택 이후에는 다시 선택 할 수 없습니다.");
        }

        bettingItem.IsRandom = false;
        bettingItem.IsAi = true;
        bettingItem.Picked = picks
            .OrderBy(picked => matches.FindIndex(match => match.HomeTeam == picked || match.AwayTeam == picked))
            .ToList();

        await SaveBettingItemAsync(bettingItem);
        return bettingItem;
    }

    private async Task SaveBettingItemAsync(WcBettingItem<Team> item)
    {
        await _fs.WriteBettingItemAsync(_bettingType, item);

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

    public async Task<WcBettingItem<Team>> PickRandomAsync(BettingUser user)
    {
        if (user.JoinStatus != UserJoinStatus.Joined)
        {
            throw new NotJoinedException();
        }
        if (!(user.JoinedBetting?.Contains(_bettingType) ?? false))
        {
            throw new NotJoinedException();
        }

        var bettingItem = await GetBettingAsync(user);
        var matches = await _getMatchesFunc(_worldCupService);
        var pickTeam = matches
            .Select(match => match.Teams.GetRandom())
            .ToList();

        bettingItem.IsRandom = true;
        bettingItem.IsAi = false;
        bettingItem.Picked = pickTeam;
        await SaveBettingItemAsync(bettingItem);
        return bettingItem;
    }
}
