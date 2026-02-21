namespace ProjectWorldCup;

public class BettingRound16Service : IBettingRound16Service
{
    private readonly IFileSystem _fs;
    private readonly string _pathKey;
    private readonly BettingType _bettingType;
    private readonly Func<IWorldCupService, Task<List<KnMatch>>> _getMatchesFunc;
    private readonly DateTime _startTime;
    private readonly string _winnersStageId;
    private object _lock = new object();
    private List<WcBettingItem<Team>> _cache = null;
    private IFifa _fifa;
    private IWorldCupService _worldCupService;
    private System.Timers.Timer _timer;

    public BettingRound16Service(
        IFileSystemService fsService,
        IFifa fifa,
        IWorldCupService worldCupService,
        ICacheClearInvoker cacheClearInvoker,
        WorldCupOption option,
        string pathKey = "Betting2022",
        BettingType bettingType = BettingType.Round16,
        Func<IWorldCupService, Task<List<KnMatch>>> getMatchesFunc = null,
        DateTime? startTime = null,
        string winnersStageId = null)
    {
        _pathKey = pathKey;
        _bettingType = bettingType;
        _getMatchesFunc = getMatchesFunc ?? (wcs => wcs.GetRound16MatchesAsync());
        _startTime = startTime ?? WorldCupConst.Round16Match1StartTime;
        _winnersStageId = winnersStageId ?? Fifa.Round8StageId;
        _fs = fsService.GetFileSystem(option.FileSystemSelect, option.Path);
        _fifa = fifa;
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
        var bettingItems = await GetAllBettingsAsync();
        bettingItems.ForEach(bettingItem => bettingItem.Fixed = winners);
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

        var bettingItems = await _fs.ReadAllBettingItemsAsync<WcBettingItem<Team>, Team>(_pathKey, _bettingType);
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
        var bettingItem = await _fs.ReadBettingItemAsync<WcBettingItem<Team>, Team>(_pathKey, _bettingType, user.AppUser);
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
                .OrderBy(picked => matches?.FirstOrDefault(m => m.HomeTeam == picked || m.AwayTeam == picked)?.Time ?? DateTime.Now)
                .ToList();
            await SaveBettingItemAsync(bettingItem);
        }
        return bettingItem;
    }

    private async Task SaveBettingItemAsync(WcBettingItem<Team> item)
    {
        await _fs.WriteBettingItemAsync(_pathKey, _bettingType, item);

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
        bettingItem.Picked = pickTeam;
        await SaveBettingItemAsync(bettingItem);
        return bettingItem;
    }
}
