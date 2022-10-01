namespace ProjectWorldCup;

public interface IBettingGroupStageService
{
    int GetRemainSeconds();
    ValueTask<List<WcBettingItem<GroupTeam>>> GetAllBettingsAsync();
    Task<WcBettingItem<GroupTeam>> GetBettingAsync(BettingUser user);
    Task<WcBettingItem<GroupTeam>> PickTeamAsync(BettingUser user, GroupTeam team);
    Task<WcBettingItem<GroupTeam>> UnpickTeamAsync(BettingUser user, GroupTeam team);
}
