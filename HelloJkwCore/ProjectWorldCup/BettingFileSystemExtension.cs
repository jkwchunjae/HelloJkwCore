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

    public static async Task<WcBettingItem<TTeam>> ReadBettingItemAsync<TTeam>(this IFileSystem fs, BettingType bettingType, AppUser user)
        where TTeam : Team
    {
        if (await fs.FileExistsAsync(BettingItemPath(bettingType, user.Id)))
        {
            var bettingItem = await fs.ReadJsonAsync<WcBettingItem<TTeam>>(BettingItemPath(bettingType, user.Id));
            bettingItem.User = user;
            return bettingItem;
        }
        else
        {
            return new WcBettingItem<TTeam>
            {
                User = user,
            };
        }
    }

    public static async Task<List<WcBettingItem<TTeam>>> ReadAllBettingItemsAsync<TTeam>(this IFileSystem fs, BettingType bettingType)
        where TTeam : Team
    {
        var directoryPath = BettingItemDirectoryPath(bettingType);
        if (await fs.DirExistsAsync(directoryPath))
        {
            var files = await fs.GetFilesAsync(directoryPath);
            var bettingItems = await files
                .Select(async filename => await fs.ReadJsonAsync<WcBettingItem<TTeam>>(path => directoryPath(path) + $"/{filename}"))
                .WhenAll();
            return bettingItems.ToList();
        }
        else
        {
            return new();
        }
    }
}
