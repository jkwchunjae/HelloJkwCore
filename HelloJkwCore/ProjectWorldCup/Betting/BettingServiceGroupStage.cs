namespace ProjectWorldCup;

public partial class BettingService : IBettingService
{
    public ValueTask<List<WcBettingItem<GroupTeam>>> GetAllGroupStageBettingsAsync(BettingType bettingType)
    {
        throw new NotImplementedException();
    }

    public Task<WcBettingItem<GroupTeam>> GetGroupStageBettingAsync(BettingUser user)
    {
        throw new NotImplementedException();
    }

    public Task<WcBettingItem<GroupTeam>> PickTeamGroupStageAsync(BettingUser user, GroupTeam team)
    {
        throw new NotImplementedException();
    }

    public Task<WcBettingItem<GroupTeam>> UnpickTeamGroupStageAsync(BettingUser user, GroupTeam team)
    {
        throw new NotImplementedException();
    }
}
