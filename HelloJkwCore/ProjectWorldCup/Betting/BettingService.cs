namespace ProjectWorldCup;

public partial class BettingService : IBettingService
{
    private readonly IFileSystem _fs;

    public BettingService(
        IFileSystemService fsService,
        WorldCupOption option)
    {
        _fs = fsService.GetFileSystem(option.FileSystemSelect, option.Path);
        _fs2018 = fsService.GetFileSystem(option.FileSystemSelect2018, option.Path);
    }

    public Task<BettingUser> JoinBettingAsync(BettingUser user, BettingType bettingType)
    {
        throw new NotImplementedException();
    }

    private async Task SaveBettingItemAsync(BettingType bettingType, IWcBettingItem<Team> item)
    {
        lock (_cache)
        {
            if (_cache.TryGetValue(bettingType, out var list))
            {
                var newList = list
                    .Where(x => x.User != item.User)
                    .Concat(new[] { item })
                    .ToList();
                _cache[bettingType] = newList;
            }
            else
            {
                _cache[bettingType] = new List<IWcBettingItem<Team>> { item };
            }
        }

        await _fs.WriteBettingItemAsync(bettingType, item);
    }
}
