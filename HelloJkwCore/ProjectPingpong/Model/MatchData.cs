namespace ProjectPingpong;

internal class MatchId : StringId
{
    public static readonly MatchId Default = new MatchId(string.Empty);
    public static class Types
    {
        public static readonly string LeagueMatch = "league.match";
        public static readonly string KnockoutMatch = "knockout.match";
        public static readonly string FreeMatch = "free.match";
    }

    public string Type { get; private set; }
    public CompetitionName CompetitionName { get; private set; }

    public MatchId(string id)
        : base(id)
    {
        (Type, CompetitionName) = Parse(id);
    }
    public MatchId(CompetitionName competitionName, LeagueId leagueId, PlayerName playerName1, PlayerName playerName2)
        : this($"{Types.LeagueMatch}-{competitionName}-{leagueId}-{playerName1}-{playerName2}")
    {
    }
    public MatchId(CompetitionName competitionName, KnockoutId knockoutId, KnockoutDepth knockoutDepth, int index)
        : this($"{Types.KnockoutMatch}-{competitionName}-{knockoutId}-{knockoutDepth}-{index}")
    {
    }

    private (string type, CompetitionName cName) Parse(string id)
    {
        if (id == string.Empty)
        {
            return (string.Empty, CompetitionName.Default);
        }
        else
        {
            var arr = id.Split('-');
            var type = arr[0];
            var cName = new CompetitionName(arr[1]);
            return (type, cName);
        }
    }
}
internal class MatchData
{
    public MatchId Id { get; set; } = MatchId.Default;
    public Player? LeftPlayer { get; set; }
    public Player? RightPlayer { get; set; }
    public int LeftSetScore { get; set; } = default;
    public int RightSetScore { get; set; } = default;

    [JsonIgnore] public IEnumerable<Player> PlayerList => new[] { LeftPlayer, RightPlayer }.Where(p => p != null).Select(p => p!);
    [JsonIgnore] public Player? Winner => 
        LeftSetScore == RightSetScore ? null :
        LeftSetScore > RightSetScore ? LeftPlayer :
                                        RightPlayer;
    [JsonIgnore] public Player? Loser => 
        LeftSetScore == RightSetScore ? null :
        LeftSetScore > RightSetScore ? RightPlayer :
                                        LeftPlayer;
    [JsonIgnore] public bool LeftWin => LeftPlayer == Winner;
    [JsonIgnore] public bool RightWin => RightPlayer == Winner;
}
internal class KnockoutMatchData : MatchData
{
    public KnockoutDepth Depth { get; set; } = KnockoutDepth.None;
    public int Index { get; set; } = default;
    public MatchId? LeftChildMatchId { get; set; }
    public MatchId? RightChildMatchId { get; set; }

    [JsonIgnore] public KnockoutMatchData? LeftMatch { get; set; }
    [JsonIgnore] public KnockoutMatchData? RightMatch { get; set; }
}


