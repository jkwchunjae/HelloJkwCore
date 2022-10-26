namespace ProjectWorldCup;

public class BettingRound16Service : IBettingRound16Service
{
    private readonly IFileSystem _fs;
    private object _lock = new object();
    private List<WcBettingItem<Team>> _cache = null;
    private IFifa _fifa;
    private IWorldCupService _worldCupService;
    private System.Timers.Timer _timer;

    public BettingRound16Service(
        IFileSystemService fsService,
        IFifa fifa,
        IWorldCupService worldCupService,
        WorldCupOption option)
    {
        _fs = fsService.GetFileSystem(option.FileSystemSelect, option.Path);
        _fifa = fifa;
        _worldCupService = worldCupService;

        _timer = new System.Timers.Timer(TimeSpan.FromMinutes(10).TotalMilliseconds);
        _timer.Elapsed += async (s, e) => await UpdateStandingsAsync();
        _timer.AutoReset = true;
        _timer.Start();
    }

    private async Task UpdateStandingsAsync()
    {
        if (DateTime.Now < WorldCupConst.Round16Match1StartTime)
            return;

        var matches = await _worldCupService.GetKnockoutStageMatchesAsync();
        var winners = matches
            .Where(match => match.StageId == Fifa.Round8StageId)
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

        var bettingItems = await _fs.ReadAllBettingItemsAsync<WcBettingItem<Team>, Team>(BettingType.Round16);
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
        var bettingItem = await _fs.ReadBettingItemAsync<WcBettingItem<Team>, Team>(BettingType.Round16, user.AppUser);
        return bettingItem;
    }

    public async Task<WcBettingItem<Team>> PickTeamAsync(BettingUser user, Team team)
    {
        if (user.JoinStatus != UserJoinStatus.Joined)
        {
            throw new NotJoinedException();
        }
        if (!(user.JoinedBetting?.Contains(BettingType.Round16) ?? false))
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
            var matches = await _worldCupService.GetRound16MatchesAsync();
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
        await _fs.WriteBettingItemAsync(BettingType.Round16, item);

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
        if (!(user.JoinedBetting?.Contains(BettingType.Round16) ?? false))
        {
            throw new NotJoinedException();
        }

        var bettingItem = await GetBettingAsync(user);
        var matches = await _worldCupService.GetRound16MatchesAsync();
        var pickTeam = matches
            .Select(match => match.Teams.GetRandom())
            .ToList();

        bettingItem.IsRandom = true;
        bettingItem.Picked = pickTeam;
        await SaveBettingItemAsync(bettingItem);
        return bettingItem;
    }
}
