namespace ProjectWorldCup;

public interface IWcBettingItem<out TTeam> : IBettingResultItem where TTeam : ITeam
{
    AppUser User { get; set; }
    IEnumerable<TTeam> Picked { get; }
    IEnumerable<TTeam> Fixed { get; }
    IEnumerable<TTeam> Failed { get; }
    [JsonIgnore]
    IEnumerable<TTeam> Success { get; }
    [JsonIgnore]
    IEnumerable<TTeam> Fail { get; }
    [JsonIgnore]
    IEnumerable<TTeam> UpComing { get; }
    bool IsRandom { get; }
    bool IsAi { get; }
}

public class WcBettingItem<TTeam> : IWcBettingItem<TTeam> where TTeam : ITeam
{
    public AppUser User { get; set; }
    public int Reward { get; set; }
    public int DetailReward { get; set; }
    public bool IsRandom { get; set; }
    public bool IsAi { get; set; }
    public int Rank { get; set; }

    [JsonIgnore]
    public string Id
    {
        get => User.Id.ToString();
        set { }
    }
    public virtual int Score
    {
        get => Success.Count();
        set { }
    }
    public IEnumerable<TTeam> Picked { get; set; } = new List<TTeam>();
    public IEnumerable<TTeam> Fixed { get; set; } = new List<TTeam>();
    public IEnumerable<TTeam> Failed { get; set; } = new List<TTeam>();
    [JsonIgnore]
    public IEnumerable<TTeam> Success => Picked.Where(s => Fixed.Contains(s)).ToList();
    [JsonIgnore]
    public IEnumerable<TTeam> Fail
    {
        get
        {
            var failList = Failed.Any()
                ? Picked.Where(s => Failed.Contains(s)).ToList()
                : Picked.Where(s => !Fixed.Contains(s)).ToList();

            var isSameFailAndUpComing = failList.Count() == UpComing.Count() && !failList.Except(UpComing).Any();

            if (isSameFailAndUpComing)
            {
                return new List<TTeam>();
            }
            else
            {
                return failList;
            }
        }
    }
    [JsonIgnore]
    public IEnumerable<TTeam> UpComing => Picked.Where(s => !Fixed.Contains(s) && !Failed.Contains(s)).ToList();
}
