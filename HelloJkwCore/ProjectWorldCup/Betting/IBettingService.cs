namespace ProjectWorldCup;

public interface IBettingService
{
    Task<List<WcBettingItem<Team>>> Get2018GroupStageBettingResult();
    Task<List<WcBettingItem<Team>>> Get2018Round16BettingResult();
    Task<List<WcFinalBettingItem<Team>>> Get2018FinalBettingResult();

    Task<WcBettingItem<GroupTeam>> GetBettingItemAsync(BettingType bettingType, AppUser user);
    Task SaveBettingItemAsync(BettingType bettingType, WcBettingItem<GroupTeam> item);
    ValueTask<List<WcBettingItem<GroupTeam>>> GetAllBettingItemsAsync(BettingType bettingType);

    #region Users
    Task<BettingUser> GetBettingUserAsync(AppUser appUser);
    Task<BettingUser> MakeJoinRequestAsync(AppUser appUser);
    Task ApproveUserAsync(BettingUser user);
    Task CancelJoinRequestAsync(AppUser appUser);
    Task<IEnumerable<BettingUser>> GetBettingUsersAsync();
    #endregion
}
