namespace ProjectWorldCup;

public interface IBettingService
{
    Task<List<WcBettingItem<Team>>> Get2018GroupStageBettingResult();
    Task<List<WcBettingItem<Team>>> Get2018Round16BettingResult();
    Task<List<WcFinalBettingItem<Team>>> Get2018FinalBettingResult();

    Task<BettingUser> JoinBettingAsync(BettingUser user, BettingType bettingType);

    #region Users
    Task<BettingUser> GetBettingUserAsync(AppUser appUser);
    Task<BettingUser> MakeJoinRequestAsync(AppUser appUser);
    Task ApproveUserAsync(BettingUser user, int initValue, AppUser approveBy);
    Task RejectUserAsync(BettingUser user, AppUser rejectBy);
    Task CancelJoinRequestAsync(AppUser appUser);
    Task<IEnumerable<BettingUser>> GetBettingUsersAsync();
    void ClearUserCache();
    #endregion
}
