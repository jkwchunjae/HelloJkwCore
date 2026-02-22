using System.Runtime.Caching;

namespace ProjectWorldCup;

public class User2022Result
{
    public int Rank { get; set; }
    public string Name { get; set; }
    public UserId UserId { get; set; }
    public int JoinCount { get; set; }
    public long Reward1 { get; set; }
    public long Reward2 { get; set; }
    public long Reward3 { get; set; }
    public long Total => Reward1 + Reward2 + Reward3;
    public long Profit => 0 - JoinCount * 10000 + Total;
}

public class BettingService2022 : IWorldCupResultService
{
    private readonly MemoryCache _cache2022 = new MemoryCache("Result2022");
    private readonly IFileSystem _fs;

    public BettingService2022(
        IFileSystemService fsService,
        WorldCupOption option)
    {
        _fs = fsService.GetFileSystem(option.FileSystemSelect, option.Path);
    }

    public async Task<List<WcBettingItem<Team>>> Get2022GroupStageBettingResultAsync()
    {
        if (_cache2022.Contains(nameof(Get2022GroupStageBettingResultAsync)))
        {
            return (List<WcBettingItem<Team>>)_cache2022[nameof(Get2022GroupStageBettingResultAsync)];
        }

        var result = await ReadAllBettingItemsAsync<WcBettingItem<Team>, Team>(_fs, BettingType.GroupStage);

        _cache2022.Add(
            nameof(Get2022GroupStageBettingResultAsync),
            result,
            DateTimeOffset.Now.AddDays(10)
        );

        return result;
    }

    public async Task<List<WcBettingItem<Team>>> Get2022Round16BettingResultAsync()
    {
        if (_cache2022.Contains(nameof(Get2022Round16BettingResultAsync)))
        {
            return (List<WcBettingItem<Team>>)_cache2022[nameof(Get2022Round16BettingResultAsync)];
        }

        var result = await ReadAllBettingItemsAsync<WcBettingItem<Team>, Team>(_fs, BettingType.Round16);

        _cache2022.Add(
            nameof(Get2022Round16BettingResultAsync),
            result,
            DateTimeOffset.Now.AddDays(10)
        );

        return result;
    }

    public async Task<List<WcFinalBettingItem<Team>>> Get2022FinalBettingResultAsync()
    {
        if (_cache2022.Contains(nameof(Get2022FinalBettingResultAsync)))
        {
            return (List<WcFinalBettingItem<Team>>)_cache2022[nameof(Get2022FinalBettingResultAsync)];
        }

        var result = await ReadAllBettingItemsAsync<WcFinalBettingItem<Team>, Team>(_fs, BettingType.Final);

        _cache2022.Add(
            nameof(Get2022FinalBettingResultAsync),
            result,
            DateTimeOffset.Now.AddDays(10)
        );

        return result;
    }

    public async Task<List<User2022Result>> Get2022BettingSummaryAsync()
    {
        if (_cache2022.Contains(nameof(Get2022BettingSummaryAsync)))
        {
            return (List<User2022Result>)_cache2022[nameof(Get2022BettingSummaryAsync)];
        }

        // Get all betting results for the three rounds
        var groupStageResults = await Get2022GroupStageBettingResultAsync();
        var round16Results = await Get2022Round16BettingResultAsync();
        var finalResults = await Get2022FinalBettingResultAsync();

        // Aggregate by user
        var allResults = new List<IWcBettingItem<Team>>();
        allResults.AddRange(groupStageResults);
        allResults.AddRange(round16Results);
        allResults.AddRange(finalResults);

        var userGroups = allResults
            .GroupBy(r => r.User.Id)
            .Select(g => new User2022Result
            {
                UserId = g.Key,
                Name = g.First().User.DisplayName,
                JoinCount = g.Count(),
                Reward1 = groupStageResults
                    .FirstOrDefault(r => r.User.Id == g.Key)?.Reward ?? 0,
                Reward2 = round16Results
                    .FirstOrDefault(r => r.User.Id == g.Key)?.Reward ?? 0,
                Reward3 = finalResults
                    .FirstOrDefault(r => r.User.Id == g.Key)?.Reward ?? 0,
            })
            .ToList();

        // Calculate ranks based on profit
        var results = userGroups
            .Select(user =>
            {
                user.Rank = userGroups.Count(x => x.Profit > user.Profit) + 1;
                return user;
            })
            .OrderBy(x => x.Rank)
            .ToList();

        // Cache for 10 days
        _cache2022.Add(
            nameof(Get2022BettingSummaryAsync),
            results,
            DateTimeOffset.Now.AddDays(10)
        );

        return results;
    }

    private static Func<Paths, string> BettingItemDirectoryPath(BettingType bettingType)
    {
        Func<Paths, string> itemPathFunc
            = path => path[WorldCupPath.Betting2022] + $"/{bettingType}";
        return itemPathFunc;
    }
    private static async Task<List<TWcBettingItem>> ReadAllBettingItemsAsync<TWcBettingItem, TTeam>(IFileSystem fs, BettingType bettingType)
        where TWcBettingItem : IWcBettingItem<TTeam>, new()
        where TTeam : Team
    {
        var directoryPath = BettingItemDirectoryPath(bettingType);
        if (await fs.DirExistsAsync(directoryPath))
        {
            var files = await fs.GetFilesAsync(directoryPath);
            var bettingItems = await files
                .Select(async filename => await fs.ReadJsonAsync<TWcBettingItem>(path => directoryPath(path) + $"/{filename}"))
                .WhenAll();
            return bettingItems.ToList();
        }
        else
        {
            return new();
        }
    }
}
