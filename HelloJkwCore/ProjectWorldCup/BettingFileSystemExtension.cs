namespace ProjectWorldCup;

internal static class BettingFileSystemExtension
{
    private static Func<Paths, string> BettingItemPath(string pathKey, BettingType bettingType, StringId userId)
        => path => path[pathKey] + $"/{bettingType}/{bettingType}.{userId}.json";

    private static Func<Paths, string> BettingItemDirectoryPath(string pathKey, BettingType bettingType)
        => path => path[pathKey] + $"/{bettingType}";

    // pathKey를 직접 지정하는 오버로드 (2022/2026 공용)
    public static async Task WriteBettingItemAsync(this IFileSystem fs, string pathKey, BettingType bettingType, IWcBettingItem<Team> bettingItem)
    {
        await fs.WriteJsonAsync(BettingItemPath(pathKey, bettingType, bettingItem.User.Id), bettingItem);
    }

    public static async Task<TWcBettingItem> ReadBettingItemAsync<TWcBettingItem, TTeam>(this IFileSystem fs, string pathKey, BettingType bettingType, AppUser user)
        where TWcBettingItem : IWcBettingItem<TTeam>, new()
        where TTeam : Team
    {
        if (await fs.FileExistsAsync(BettingItemPath(pathKey, bettingType, user.Id)))
        {
            var bettingItem = await fs.ReadJsonAsync<TWcBettingItem>(BettingItemPath(pathKey, bettingType, user.Id));
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

    public static async Task<List<TWcBettingItem>> ReadAllBettingItemsAsync<TWcBettingItem, TTeam>(this IFileSystem fs, string pathKey, BettingType bettingType)
        where TWcBettingItem : IWcBettingItem<TTeam>, new()
        where TTeam : Team
    {
        var directoryPath = BettingItemDirectoryPath(pathKey, bettingType);
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

    // 하위 호환 오버로드 (pathKey 생략 시 Betting2022 사용)
    public static Task WriteBettingItemAsync(this IFileSystem fs, BettingType bettingType, IWcBettingItem<Team> bettingItem)
        => fs.WriteBettingItemAsync("Betting2022", bettingType, bettingItem);

    public static Task<TWcBettingItem> ReadBettingItemAsync<TWcBettingItem, TTeam>(this IFileSystem fs, BettingType bettingType, AppUser user)
        where TWcBettingItem : IWcBettingItem<TTeam>, new()
        where TTeam : Team
        => fs.ReadBettingItemAsync<TWcBettingItem, TTeam>("Betting2022", bettingType, user);

    public static Task<List<TWcBettingItem>> ReadAllBettingItemsAsync<TWcBettingItem, TTeam>(this IFileSystem fs, BettingType bettingType)
        where TWcBettingItem : IWcBettingItem<TTeam>, new()
        where TTeam : Team
        => fs.ReadAllBettingItemsAsync<TWcBettingItem, TTeam>("Betting2022", bettingType);
}
