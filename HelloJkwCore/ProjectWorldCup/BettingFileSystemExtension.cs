namespace ProjectWorldCup;

internal static class BettingFileSystemExtension
{
    private static Func<Paths, string> BettingItemPath(BettingType bettingType, StringId userId)
    {
        Func<Paths, string> itemPathFunc
            = path => path["Betting2022"] + $"/{bettingType}/{bettingType}.{userId}.json";
        return itemPathFunc;
    }
    public static async Task WriteBettingItemAsync(this IFileSystem fs, BettingType bettingType, WcBettingItem bettingItem)
    {
        await fs.WriteJsonAsync(BettingItemPath(bettingType, bettingItem.User.Id), bettingItem);
    }

    public static async Task<WcBettingItem> ReadBettingItemAsync(this IFileSystem fs, BettingType bettingType, AppUser user)
    {
        if (await fs.FileExistsAsync(BettingItemPath(bettingType, user.Id)))
        {
            var bettingItem = await fs.ReadJsonAsync<WcBettingItem>(BettingItemPath(bettingType, user.Id));
            bettingItem.User = user;
            return bettingItem;
        }
        else
        {
            return new WcBettingItem
            {
                User = user,
            };
        }
    }
}
