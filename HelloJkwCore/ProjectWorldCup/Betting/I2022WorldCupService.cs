namespace ProjectWorldCup;

public interface I2022WorldCupService
{
    Task<List<WcBettingItem<Team>>> Get2022GroupStageBettingResultAsync();
    Task<List<WcBettingItem<Team>>> Get2022Round16BettingResultAsync();
    Task<List<WcFinalBettingItem<Team>>> Get2022FinalBettingResultAsync();
    Task<List<User2022Result>> Get2022BettingSummaryAsync();
}
