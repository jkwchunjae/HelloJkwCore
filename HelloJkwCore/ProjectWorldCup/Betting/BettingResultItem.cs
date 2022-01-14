namespace ProjectWorldCup;

public interface IBettingResultItem
{
    string Id { get; set; }
    int Score { get; set; }
}

public interface IBettingResultItemResult : IBettingResultItem
{
    int Reward { get; set; }
}

public class BettingResultItem : IBettingResultItemResult
{
    public string Id { get; set; }
    public int Score { get; set; }
    public int Reward { get; set; }

    public BettingResultItem() { }
    public BettingResultItem(string userId, int score)
    {
        Id = userId;
        Score = score;
    }
}
