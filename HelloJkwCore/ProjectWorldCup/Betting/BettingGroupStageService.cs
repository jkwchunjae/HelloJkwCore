namespace ProjectWorldCup;

public class BettingGroupStageService : IBettingGroupStageService
{
    private readonly IFileSystem _fs;
    private object _lock = new object();
    private List<WcBettingItem<GroupTeam>> _cache = null;
    public BettingGroupStageService(
        IFileSystemService fsService,
        WorldCupOption option)
    {
        _fs = fsService.GetFileSystem(option.FileSystemSelect, option.Path);
    }

    public async ValueTask<List<WcBettingItem<GroupTeam>>> GetAllBettingsAsync()
    {
        lock (_lock)
        {
            if (_cache != null)
            {
                return _cache;
            }
        }

        var bettingItems = await _fs.ReadAllBettingItemsAsync<GroupTeam>(BettingType.GroupStage);
        lock (_lock)
        {
            _cache = bettingItems;
        }
        return bettingItems;
    }

    public async Task<WcBettingItem<GroupTeam>> GetBettingAsync(BettingUser user)
    {
        var bettingItem = await _fs.ReadBettingItemAsync<GroupTeam>(BettingType.GroupStage, user.AppUser);
        return bettingItem;
    }

    public async Task<WcBettingItem<GroupTeam>> PickTeamAsync(BettingUser user, GroupTeam team)
    {
        var bettingItem = await GetBettingAsync(user);
        if (bettingItem == null)
            return null;

        if (bettingItem.Picked.Empty(picked => picked == team))
        {
            bettingItem.Picked = bettingItem.Picked
                .Concat(new[] { team })
                .OrderBy(x => x.GroupName)
                .ThenBy(x => x.Placement)
                .ToList();
            await SaveBettingItemAsync(bettingItem);
        }
        return bettingItem;
    }

    public async Task<WcBettingItem<GroupTeam>> UnpickTeamAsync(BettingUser user, GroupTeam team)
    {
        var bettingItem = await GetBettingAsync(user);
        if (bettingItem == null)
            return null;

        if (bettingItem.Picked.Any(picked => picked == team))
        {
            bettingItem.Picked = bettingItem.Picked
                .Where(picked => picked != team)
                .OrderBy(x => x.GroupName)
                .ThenBy(x => x.Placement)
                .ToList();
            await SaveBettingItemAsync(bettingItem);
        }
        return bettingItem;
    }

    private async Task SaveBettingItemAsync(IWcBettingItem<GroupTeam> item)
    {
        await _fs.WriteBettingItemAsync(BettingType.GroupStage, item);
    }
}
