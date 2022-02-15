using System.Runtime.Caching;

namespace ProjectWorldCup;

public partial class WorldCupService
{
    private MemoryCache _cache2018 = new MemoryCache("Result2018");

    public async Task<List<WcBettingItem>> Get2018GroupStageBettingResult()
    {
        if (_cache2018.Contains(nameof(Get2018GroupStageBettingResult)))
        {
            return (List<WcBettingItem>)_cache2018[nameof(Get2018GroupStageBettingResult)];
        }

        var bettingData = await _fs2018.ReadJsonAsync<BettingData2018>(path => path[WorldCupPath.Result2018GroupStage]);

        var result = ToWcBettingItem(bettingData);

        _cache2018.Add(nameof(Get2018GroupStageBettingResult), result, DateTimeOffset.MaxValue);

        return result;
    }

    public Task<List<WcBettingItem>> Get2018Round16BettingResult()
    {
        throw new NotImplementedException();
    }

    public Task<List<WcFinalBettingItem>> Get2018FinalBettingResult()
    {
        throw new NotImplementedException();
    }

    private List<WcBettingItem> ToWcBettingItem(BettingData2018 data)
    {
        var @fixed = data.TargetList
            .Select(e => MakeFakeTeam(e))
            .ToList();

        return data.UserBettingList.Values
            .Where(x => x.BettingGroup == "A")
            .Select(x => new WcBettingItem
            {
                User = new AppUser { Id = AppUser.UserId("fake", x.Username), UserName = x.Username },
                Picked = x.BettingList
                    .Select(e => MakeFakeTeam(e))
                    .ToList(),
                Fixed = @fixed,
            })
            .ToList();
    }

    private Team MakeFakeTeam(ITeam data)
    {
        if (_cache2018.Contains(data.Value))
        {
            return (Team)_cache2018[data.Value];
        }

        var team = new Team
        {
            Id = data.Value,
            Name = data.Value,
            Flag = data.Value,
        };

        _cache2018.Add(data.Value, team, DateTimeOffset.MaxValue);

        return team;
    }
}
