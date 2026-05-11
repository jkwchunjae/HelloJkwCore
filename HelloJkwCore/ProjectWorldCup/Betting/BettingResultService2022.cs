using System.Runtime.Caching;

namespace ProjectWorldCup;

public class BettingResultService2022 : IBettingResultService
{
    private readonly MemoryCache _cache2022 = new MemoryCache("Result2022");
    private readonly IFileSystem _fs;

    public BettingResultService2022(
        IFileSystemService fsService,
        WorldCupOption option)
    {
        _fs = fsService.GetFileSystem(option.FileSystemSelect, option.Path);
    }

    public async Task<List<WcBettingItem<Team>>> GetGroupStageBettingResultAsync()
    {
        if (_cache2022.Contains(nameof(GetGroupStageBettingResultAsync)))
        {
            return (List<WcBettingItem<Team>>)_cache2022[nameof(GetGroupStageBettingResultAsync)];
        }

        var result = await ReadAllBettingItemsAsync<WcBettingItem<Team>, Team>(_fs, BettingType.GroupStage);

        _cache2022.Add(
            nameof(GetGroupStageBettingResultAsync),
            result,
            DateTimeOffset.Now.AddDays(10)
        );

        return result;
    }

    public async Task<List<WcBettingItem<Team>>> GetRound16BettingResultAsync()
    {
        if (_cache2022.Contains(nameof(GetRound16BettingResultAsync)))
        {
            return (List<WcBettingItem<Team>>)_cache2022[nameof(GetRound16BettingResultAsync)];
        }

        var result = await ReadAllBettingItemsAsync<WcBettingItem<Team>, Team>(_fs, BettingType.Round16);

        _cache2022.Add(
            nameof(GetRound16BettingResultAsync),
            result,
            DateTimeOffset.Now.AddDays(10)
        );

        return result;
    }

    public async Task<List<WcFinalBettingItem<Team>>> GetFinalBettingResultAsync()
    {
        if (_cache2022.Contains(nameof(GetFinalBettingResultAsync)))
        {
            return (List<WcFinalBettingItem<Team>>)_cache2022[nameof(GetFinalBettingResultAsync)];
        }

        var result = await ReadAllBettingItemsAsync<WcFinalBettingItem<Team>, Team>(_fs, BettingType.Final);

        _cache2022.Add(
            nameof(GetFinalBettingResultAsync),
            result,
            DateTimeOffset.Now.AddDays(10)
        );

        return result;
    }

    public async Task<List<UserResult>> GetBettingSummaryAsync()
    {
        if (_cache2022.Contains(nameof(GetBettingSummaryAsync)))
        {
            return (List<UserResult>)_cache2022[nameof(GetBettingSummaryAsync)];
        }

        // Get all betting results for the three rounds
        var groupStageResults = await GetGroupStageBettingResultAsync();
        var round16Results = await GetRound16BettingResultAsync();
        var finalResults = await GetFinalBettingResultAsync();

        // Aggregate by user
        var allResults = new List<IWcBettingItem<Team>>();
        allResults.AddRange(groupStageResults);
        allResults.AddRange(round16Results);
        allResults.AddRange(finalResults);

        var userGroups = allResults
            .GroupBy(r => r.User.Id)
            .Select(g => new UserResult
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
            nameof(GetBettingSummaryAsync),
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

    public Task<List<WcBettingItem<Team>>> GetRound32BettingResultAsync()
    {
        throw new NotImplementedException();
    }
}
