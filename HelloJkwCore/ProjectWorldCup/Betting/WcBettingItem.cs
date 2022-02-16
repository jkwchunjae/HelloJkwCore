namespace ProjectWorldCup;

public class WcBettingItem : IBettingResultItem
{
    public AppUser User { get; init; }
    public List<Team> Picked { get; set; }
    public List<Team> Fixed { get; set; }
    public int Reward { get; set; }

    public string Id
    {
        get => User.Id.ToString();
        set { }
    }
    public virtual int Score
    {
        get => Success.Count;
        set { }
    }
    public List<Team> Success => Picked?.Where(s => Fixed?.Contains(s) ?? false).ToList() ?? new();
    public List<Team> Fail => Picked?.Where(s => !(Fixed?.Contains(s) ?? false)).ToList() ?? new();
}
