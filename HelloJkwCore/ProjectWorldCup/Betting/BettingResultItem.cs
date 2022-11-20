namespace ProjectWorldCup;

public interface IBettingResultItem
{
    string Id { get; set; }
    int Score { get; set; }
    int Reward { get; set; }
    int Rank { get; set; }
}

public class BettingResultItem : IBettingResultItem
{
    public string Id { get; set; }
    public int Score { get; set; }
    public int Reward { get; set; }
    public int Rank { get; set; }

    public BettingResultItem() { }
    public BettingResultItem(string userId, int score)
    {
        Id = userId;
        Score = score;
    }
}
