﻿namespace ProjectWorldCup;

public interface IBettingService
{
    Task<List<WcBettingItem<Team>>> Get2018GroupStageBettingResult();
    Task<List<WcBettingItem<Team>>> Get2018Round16BettingResult();
    Task<List<WcFinalBettingItem<Team>>> Get2018FinalBettingResult();

    Task<BettingUser> JoinBettingAsync(BettingUser user, BettingType bettingType);

    #region Users
    Task<BettingUser> GetBettingUserAsync(AppUser appUser);
    Task<BettingUser> MakeJoinRequestAsync(AppUser appUser);
    Task SetRequestStateAsync(BettingUser user, AppUser operateBy);
    Task ApproveUserAsync(BettingUser user, int initValue, AppUser approveBy);
    Task RejectUserAsync(BettingUser user, AppUser rejectBy);
    Task CancelJoinRequestAsync(AppUser appUser);
    Task<BettingUser> AddHistoryAsync(BettingUser user, BettingHistory history);
    Task<BettingUser> DeleteHistoryAsync(BettingUser user, BettingHistory history);
    Task<IEnumerable<BettingUser>> GetBettingUsersAsync(bool updateAppUser = false);
    Task<BettingUser> AddRewardAsync(BettingUser user, HistoryType rewardType, long reward);
    void ClearUserCache();
    #endregion
}
