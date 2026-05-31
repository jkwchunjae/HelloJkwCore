namespace ProjectWorldCup;

internal class WorldcupBettingFileSystem<TWcBettingItem, TTeam>(IFileSystem fs, string pathKey)
        where TWcBettingItem : IWcBettingItem<TTeam>, new()
        where TTeam : Team
{
    private readonly IFileSystem _fs = fs;
    private readonly string _pathKey = pathKey;

    private Func<Paths, string> BettingItemPath(BettingType bettingType, StringId userId)
    => path => path[_pathKey] + $"/{bettingType}/{bettingType}.{userId}.json";

    private Func<Paths, string> BettingItemDirectoryPath(BettingType bettingType)
        => path => path[_pathKey] + $"/{bettingType}";

    public async Task WriteBettingItemAsync(BettingType bettingType, IWcBettingItem<TTeam> bettingItem)
    {
        await _fs.WriteJsonAsync(BettingItemPath(bettingType, bettingItem.User.Id), bettingItem);
    }

    public async Task<TWcBettingItem> ReadBettingItemAsync(BettingType bettingType, AppUser user)
    {
        if (await _fs.FileExistsAsync(BettingItemPath(bettingType, user.Id)))
        {
            var bettingItem = await _fs.ReadJsonAsync<TWcBettingItem>(BettingItemPath(bettingType, user.Id));
            bettingItem.User = user;
            return bettingItem;
        }
        else
        {
            return new TWcBettingItem
            {
                User = user,
            };
        }
    }

    public async Task<List<TWcBettingItem>> ReadAllBettingItemsAsync(BettingType bettingType)
    {
        var directoryPath = BettingItemDirectoryPath(bettingType);
        if (await _fs.DirExistsAsync(directoryPath))
        {
            var files = await _fs.GetFilesAsync(directoryPath);
            var bettingItems = await files
                .Select(async filename => await _fs.ReadJsonAsync<TWcBettingItem>(path => directoryPath(path) + $"/{filename}"))
                .WhenAll();
            return bettingItems.ToList();
        }
        else
        {
            return new();
        }
    }
}
