namespace ProjectWorldCup;

public partial class BettingService : IBettingService
{
    /// <summary> _cacheGroupBettings에 접근하는 lock object </summary>
    private object _groupLock = new object();
    private List<WcBettingItem<GroupTeam>> _cacheGroupBettings = null;

    public async ValueTask<List<WcBettingItem<GroupTeam>>> GetAllGroupStageBettingsAsync(BettingType bettingType)
    {
        lock (_groupLock)
        {
            if (_cacheGroupBettings != null)
            {
                return _cacheGroupBettings;
            }
        }

        var bettingItems = await _fs.ReadAllBettingItemsAsync<GroupTeam>(bettingType);
        lock (_groupLock)
        {
            _cacheGroupBettings = bettingItems;
        }
        return bettingItems;
    }

    public async Task<WcBettingItem<GroupTeam>> GetGroupStageBettingAsync(BettingUser user)
    {
        var bettingItem = await _fs.ReadBettingItemAsync<GroupTeam>(BettingType.GroupStage, user.AppUser);
        return bettingItem;
    }

    public async Task<WcBettingItem<GroupTeam>> PickTeamGroupStageAsync(BettingUser user, GroupTeam team)
    {
        var bettingItem = await GetGroupStageBettingAsync(user);
        if (bettingItem == null)
            return null;

        if (bettingItem.Picked.Empty(picked => picked == team))
        {
            bettingItem.Picked = bettingItem.Picked
                .Concat(new[] { team })
                .OrderBy(x => x.GroupName)
                .ThenBy(x => x.Placement)
                .ToList();
            await SaveBettingItemAsync(BettingType.GroupStage, bettingItem);
        }
        return bettingItem;
    }

    public async Task<WcBettingItem<GroupTeam>> UnpickTeamGroupStageAsync(BettingUser user, GroupTeam team)
    {
        var bettingItem = await GetGroupStageBettingAsync(user);
        if (bettingItem == null)
            return null;

        if (bettingItem.Picked.Any(picked => picked == team))
        {
            bettingItem.Picked = bettingItem.Picked
                .Where(picked => picked != team)
                .OrderBy(x => x.GroupName)
                .ThenBy(x => x.Placement)
                .ToList();
            await SaveGroupBettingItemAsync(BettingType.GroupStage, bettingItem);
        }
        return bettingItem;

    }

    private Task SaveGroupBettingItemAsync(IWcBettingItem<GroupTeam> item)
    {

    }
}
