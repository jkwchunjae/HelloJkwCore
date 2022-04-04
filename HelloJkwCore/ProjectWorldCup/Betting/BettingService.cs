namespace ProjectWorldCup;

public partial class BettingService : IBettingService
{
    private readonly IFileSystem _fs;
    Dictionary<BettingType, List<WcBettingItem>> _cache = new();

    public BettingService(
        IFileSystemService fsService,
        WorldCupOption option)
    {
        _fs = fsService.GetFileSystem(option.FileSystemSelect, option.Path);
        _fs2018 = fsService.GetFileSystem(option.FileSystemSelect2018, option.Path);
    }

    public async Task<WcBettingItem> GetBettingItemAsync(BettingType bettingType, AppUser user)
    {
        var bettingItem = await _fs.ReadBettingItemAsync(bettingType, user);
        return bettingItem;
    }

    public async Task SaveBettingItemAsync(BettingType bettingType, WcBettingItem item)
    {
        lock (_cache)
        {
            var list = _cache.ContainsKey(bettingType) ? _cache[bettingType] : new List<WcBettingItem>();

            list.RemoveAll(x => x.User.Id == item.User.Id);
            list.Add(item);
        }

        await _fs.WriteBettingItemAsync(bettingType, item);
    }
}
