namespace ProjectWorldCup;

public interface IBettingService
{
    Task<List<WcBettingItem<Team>>> Get2018GroupStageBettingResult();
    Task<List<WcBettingItem<Team>>> Get2018Round16BettingResult();
    Task<List<WcFinalBettingItem<Team>>> Get2018FinalBettingResult();

    Task<BettingUser> JoinBettingAsync(BettingUser user, BettingType bettingType);
    ValueTask<List<WcBettingItem<GroupTeam>>> GetAllGroupStageBettingsAsync(BettingType bettingType);
    Task<WcBettingItem<GroupTeam>> GetGroupStageBettingAsync(BettingUser user);
    Task<WcBettingItem<GroupTeam>> PickTeamGroupStageAsync(BettingUser user, GroupTeam team);
    Task<WcBettingItem<GroupTeam>> UnpickTeamGroupStageAsync(BettingUser user, GroupTeam team);

    #region Users
    Task<BettingUser> GetBettingUserAsync(AppUser appUser);
    Task<BettingUser> MakeJoinRequestAsync(AppUser appUser);
    Task ApproveUserAsync(BettingUser user);
    Task CancelJoinRequestAsync(AppUser appUser);
    Task<IEnumerable<BettingUser>> GetBettingUsersAsync();
    #endregion
}
