namespace ProjectWorldCup.Models;

public class UserResult
{
    public int Rank { get; set; }
    public string Name { get; set; }
    public UserId UserId { get; set; }
    public int JoinCount { get; set; }
    public long Reward1 { get; set; }
    public long Reward2 { get; set; }
    public long Reward3 { get; set; }
    public long Reward4 { get; set; }
    public long Total => Reward1 + Reward2 + Reward3 + Reward4;
    public long Profit => 0 - JoinCount * 10000 + Total;
}
