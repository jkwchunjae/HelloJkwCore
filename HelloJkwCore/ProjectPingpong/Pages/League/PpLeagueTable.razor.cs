namespace ProjectPingpong.Pages.League;

public class PlayerResult
{
    public int WinCount { get; set; }
    public int LoseCount { get; set; }
    public int WinPoint { get; set; }
    public int Rank { get; set; }

    public PlayerResult(Player player, List<MatchData>? matches)
    {
        WinCount = matches?
            .Count(m => m.Winner?.Name == player.Name) ?? default;
        LoseCount = matches?
            .Count(m => m.Loser?.Name == player.Name) ?? default;
        WinPoint = WinCount * 2 + LoseCount * 1;
    }
}

public partial class PpLeagueTable : JkwPageBase
{
    [Parameter] public LeagueData League { get; set; }

    private Dictionary<PlayerName, PlayerResult> ResultSet = new();
    private Dictionary<PlayerName, Dictionary<PlayerName, MatchData>> MatchMap = new();

    protected override void OnPageParametersSet()
    {
        ResultSet = League?.PlayerList
            ?.ToDictionary(p => p.Name, p => new PlayerResult(p, League.MatchList)) ?? new();

        if (League?.MatchList?.Any() ?? false)
        {
            MatchMap = League.MatchList
                .SelectMany(match =>
                {
                    return new[]
                    {
                    (match.LeftPlayer, match.RightPlayer, match),
                    (match.RightPlayer, match.LeftPlayer, match),
                    };
                })
                .GroupBy(x => x.Item1!.Name)
                .ToDictionary(x => x.Key, x => x.GroupBy(e => e.Item2!.Name).ToDictionary(e => e.Key, e => e.First().match));
        }
    }
}
