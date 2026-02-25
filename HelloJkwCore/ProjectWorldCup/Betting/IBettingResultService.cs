namespace ProjectWorldCup;

public interface IBettingResultService
{
    Task<List<WcBettingItem<Team>>> GetGroupStageBettingResultAsync();
    Task<List<WcBettingItem<Team>>> GetRound32BettingResultAsync();
    Task<List<WcBettingItem<Team>>> GetRound16BettingResultAsync();
    Task<List<WcFinalBettingItem<Team>>> GetFinalBettingResultAsync();
    Task<List<UserResult>> GetBettingSummaryAsync();
}
