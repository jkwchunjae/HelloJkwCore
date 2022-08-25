namespace ProjectWorldCup;

public partial class BettingService : IBettingService
{
    public Task ApproveUserAsync(BettingUser user)
    {
        throw new NotImplementedException();
    }

    public Task CancelJoinRequestAsync(AppUser appUser)
    {
        throw new NotImplementedException();
    }

    public Task<BettingUser> GetBettingUserAsync(AppUser appUser)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<BettingUser>> GetBettingUsersAsync()
    {
        throw new NotImplementedException();
    }

    public Task<BettingUser> MakeJoinRequestAsync(AppUser appUser)
    {
        throw new NotImplementedException();
    }
}
