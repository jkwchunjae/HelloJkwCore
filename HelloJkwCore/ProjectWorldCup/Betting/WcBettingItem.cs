namespace ProjectWorldCup;

public interface IWcBettingItem<out TTeam> : IBettingResultItem where TTeam : Team
{
    public AppUser User { get; }
    public IEnumerable<TTeam> Picked { get; }
    public IEnumerable<TTeam> Fixed { get; }
}

public class WcBettingItem<TTeam> : IWcBettingItem<TTeam> where TTeam : Team
{
    public AppUser User { get; set; }
    public int Reward { get; set; }

    [JsonIgnore]
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
    public IEnumerable<TTeam> Picked { get; set; } = new List<TTeam>();
    public IEnumerable<TTeam> Fixed { get; set; } = new List<TTeam>();
    [JsonIgnore]
    public List<TTeam> Success => Picked.Where(s => Fixed.Contains(s)).ToList();
    [JsonIgnore]
    public List<TTeam> Fail => Picked.Where(s => !Fixed.Contains(s)).ToList();
}
