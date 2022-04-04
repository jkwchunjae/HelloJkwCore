namespace ProjectWorldCup;

public interface IBettingService
{
    Task<List<WcBettingItem>> Get2018GroupStageBettingResult();
    Task<List<WcBettingItem>> Get2018Round16BettingResult();
    Task<List<WcFinalBettingItem>> Get2018FinalBettingResult();

    Task<WcBettingItem> GetBettingItemAsync(BettingType bettingType, AppUser user);
    Task SaveBettingItemAsync(BettingType bettingType, WcBettingItem item);
}
