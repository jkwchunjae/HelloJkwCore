namespace ProjectWorldCup;

public interface IBettingRound16Service
{
    ValueTask<List<WcBettingItem<Team>>> GetAllBettingsAsync();
    Task<WcBettingItem<Team>> GetBettingAsync(BettingUser user);
    Task<WcBettingItem<Team>> PickTeamAsync(BettingUser user, Team team);
    Task<WcBettingItem<Team>> PickRandomAsync(BettingUser user);
}
