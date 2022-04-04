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
    public static async Task WriteBettingItemAsync(this IFileSystem fs, BettingType bettingType, WcBettingItem<GroupTeam> bettingItem)
    {
        await fs.WriteJsonAsync(BettingItemPath(bettingType, bettingItem.User.Id), bettingItem);
    }

    public static async Task<WcBettingItem<GroupTeam>> ReadBettingItemAsync(this IFileSystem fs, BettingType bettingType, AppUser user)
    {
        if (await fs.FileExistsAsync(BettingItemPath(bettingType, user.Id)))
        {
            var bettingItem = await fs.ReadJsonAsync<WcBettingItem<GroupTeam>>(BettingItemPath(bettingType, user.Id));
            bettingItem.User = user;
            return bettingItem;
        }
        else
        {
            return new WcBettingItem<GroupTeam>
            {
                User = user,
            };
        }
    }

    public static async Task<List<WcBettingItem<GroupTeam>>> ReadAllBettingItemsAsync(this IFileSystem fs, BettingType bettingType)
    {
        var directoryPath = BettingItemDirectoryPath(bettingType);
        if (await fs.DirExistsAsync(directoryPath))
        {
            var files = await fs.GetFilesAsync(directoryPath);
            var bettingItems = await files
                .Select(async filename => await fs.ReadJsonAsync<WcBettingItem<GroupTeam>>(path => directoryPath(path) + $"/{filename}"))
                .WhenAll();
            return bettingItems.ToList();
        }
        else
        {
            return new();
        }
    }
}
