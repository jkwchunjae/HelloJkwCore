namespace ProjectWorldCup;

public class WcBettingItem<TTeam> : IBettingResultItem where TTeam : Team
{
    public AppUser User { get; set; }
    public List<TTeam> Picked { get; set; } = new();
    public List<TTeam> Fixed { get; set; } = new();
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
    public List<TTeam> Success => Picked.Where(s => Fixed.Contains(s)).ToList();
    public List<TTeam> Fail => Picked.Where(s => !Fixed.Contains(s)).ToList();
}
