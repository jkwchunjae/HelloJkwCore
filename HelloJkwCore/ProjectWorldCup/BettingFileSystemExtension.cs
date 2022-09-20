namespace ProjectWorldCup;

internal static class BettingFileSystemExtension
{
    private static Func<Paths, string> BettingItemPath(BettingType bettingType, StringId userId)
    {
        Func<Paths, string> itemPathFunc
            = path => path["Betting2022"] + $"/{bettingType}/{bettingType}.{userId}.json";
        return itemPathFunc;
    }

    private static Func<Paths, string> BettingItemDirectoryPath(BettingType bettingType)
    {
        Func<Paths, string> itemPathFunc
            = path => path["Betting2022"] + $"/{bettingType}";
        return itemPathFunc;
    }
    public static async Task WriteBettingItemAsync(this IFileSystem fs, BettingType bettingType, IWcBettingItem<Team> bettingItem)
    {
        await fs.WriteJsonAsync(BettingItemPath(bettingType, bettingItem.User.Id), bettingItem);
    }

    public static async Task<TWcBettingItem> ReadBettingItemAsync<TWcBettingItem, TTeam>(this IFileSystem fs, BettingType bettingType, AppUser user)
        where TWcBettingItem : IWcBettingItem<TTeam>, new()
        where TTeam : Team
    {
        if (await fs.FileExistsAsync(BettingItemPath(bettingType, user.Id)))
        {
            var bettingItem = await fs.ReadJsonAsync<TWcBettingItem>(BettingItemPath(bettingType, user.Id));
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
    public static async Task<List<TWcBettingItem>> ReadAllBettingItemsAsync<TWcBettingItem, TTeam>(this IFileSystem fs, BettingType bettingType)
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
