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

    public async Task<List<WcBettingItem>> Get2018Round16BettingResult()
    {
        if (_cache2018.Contains(nameof(Get2018Round16BettingResult)))
        {
            return (List<WcBettingItem>)_cache2018[nameof(Get2018Round16BettingResult)];
        }

        var bettingData = await _fs2018.ReadJsonAsync<BettingData2018>(path => path[WorldCupPath.Result2018Round16]);

        var result = ToWcBettingItem(bettingData);

        _cache2018.Add(nameof(Get2018Round16BettingResult), result, DateTimeOffset.MaxValue);

        return result;
    }

    public async Task<List<WcFinalBettingItem>> Get2018FinalBettingResult()
    {
        if (_cache2018.Contains(nameof(Get2018FinalBettingResult)))
        {
            return (List<WcFinalBettingItem>)_cache2018[nameof(Get2018FinalBettingResult)];
        }

        var bettingData = await _fs2018.ReadJsonAsync<BettingData2018>(path => path[WorldCupPath.Result2018Final]);

        var result = ToWcFinalBettingItem(bettingData);

        _cache2018.Add(nameof(Get2018FinalBettingResult), result, DateTimeOffset.MaxValue);

        return result;
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

    private List<WcFinalBettingItem> ToWcFinalBettingItem(BettingData2018 data)
    {
        var @fixed = data.TargetList
            .Select(e => MakeFakeTeam(e))
            .ToList();

        var order = new Dictionary<string, int>
        {
            ["Champion"] = 1,
            ["Second"] = 2,
            ["Third"] = 3,
            ["Fourth"] = 4,
        };

        return data.UserBettingList.Values
            .Where(x => x.BettingGroup == "A")
            .Select(x => new WcFinalBettingItem
            {
                User = new AppUser { Id = AppUser.UserId("fake", x.Username), UserName = x.Username },
                Picked = x.BettingList
                    .Where(x => order.ContainsKey(x.Id))
                    .OrderBy(x => order[x.Id])
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
            Flag = $"https://cloudinary.fifa.com/api/v1/picture/flags-sq-1/{data.Value}",
        };

        _cache2018.Add(data.Value, team, DateTimeOffset.MaxValue);

        return team;
    }
}
