namespace ProjectWorldCup.Models;

public struct RewardInfo
{
    public int Value { get; set; }
    public bool IsRandom { get; set; }
    public bool IsAi { get; set; }
}

public class UserResult
{
    public int Rank { get; set; }
    public string Name { get; set; }
    public UserId UserId { get; set; }
    public int JoinCount { get; set; }
    public RewardInfo? Reward1 { get; set; }
    public RewardInfo? Reward2 { get; set; }
    public RewardInfo? Reward3 { get; set; }
    public RewardInfo? Reward4 { get; set; }
    public long Total => (Reward1?.Value ?? 0) + (Reward2?.Value ?? 0) + (Reward3?.Value ?? 0) + (Reward4?.Value ?? 0);
    public long Profit => 0 - JoinCount * 10000 + Total;
}
