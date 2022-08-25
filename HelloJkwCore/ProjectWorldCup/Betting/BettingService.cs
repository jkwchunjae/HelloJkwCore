namespace ProjectWorldCup;

public partial class BettingService : IBettingService
{
    private readonly IFileSystem _fs;
    Dictionary<BettingType, List<WcBettingItem<Team>>> _cache = new();

    public BettingService(
        IFileSystemService fsService,
        WorldCupOption option)
    {
        _fs = fsService.GetFileSystem(option.FileSystemSelect, option.Path);
        _fs2018 = fsService.GetFileSystem(option.FileSystemSelect2018, option.Path);
    }

    public async ValueTask<List<WcBettingItem<Team>>> GetAllBettingItemsAsync(BettingType bettingType)
    {
        lock (_cache)
        {
            if (_cache.TryGetValue(bettingType, out var bettingItemss))
            {
                return bettingItemss;
            }
        }

        var bettingItems = await _fs.ReadAllBettingItemsAsync(bettingType);
        lock (_cache)
        {
            _cache[bettingType] = bettingItems;
        }
        return bettingItems;
    }

    public async Task<WcBettingItem<Team>> GetBettingItemAsync(BettingType bettingType, AppUser user)
    {
        var bettingItem = await _fs.ReadBettingItemAsync(bettingType, user);
        return bettingItem;
    }

    private async Task SaveBettingItemAsync(BettingType bettingType, WcBettingItem<Team> item)
    {
        lock (_cache)
        {
            if (_cache.TryGetValue(bettingType, out var list))
            {
                list.RemoveAll(x => x.User.Id == item.User.Id);
                list.Add(item);
            }
            else
            {
                _cache[bettingType] = new() { item };
            }
        }

        await _fs.WriteBettingItemAsync(bettingType, item);
    }
}
