namespace ProjectPingpong;

[JsonConverter(typeof(StringIdJsonConverter<MatchId>))]
public class MatchId : StringId
{
    public static readonly MatchId Default = new MatchId(string.Empty);
    public static class Types
    {
        public static readonly string LeagueMatch = "leaguematch";
        public static readonly string KnockoutMatch = "knockoutmatch";
        public static readonly string FreeMatch = "freematch";
    }

    private string _type = string.Empty;
    private CompetitionName _competitionName = CompetitionName.Default;

    public string Type
    {
        get
        {
            if (string.IsNullOrEmpty(_competitionName?.Id))
            {
                (_type, _competitionName) = Parse(Id);
            }
            return _type;
        }
        set
        {
            _type = value;
        }
    }
    public CompetitionName CompetitionName
    {
        get
        {
            if (string.IsNullOrEmpty(_competitionName?.Id))
            {
                (_type, _competitionName) = Parse(Id);
            }
            return _competitionName;
        }
        set
        {
            _competitionName = value;
        }
    }

    public MatchId() { }
    public MatchId(string id)
        : base(id)
    {
        (Type, CompetitionName) = Parse(id);
    }
    public MatchId(LeagueId leagueId, PlayerName playerName1, PlayerName playerName2)
        : this($"{Types.LeagueMatch}.{leagueId}.{playerName1}.{playerName2}")
    {
    }
    public MatchId(KnockoutId knockoutId, KnockoutDepth knockoutDepth, int index)
        : this($"{Types.KnockoutMatch}.{knockoutId}.{knockoutDepth}.{index}")
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
            var arr = id.Split('.');
            var type = arr[0];
            var cName = new CompetitionName(arr[1]);
            return (type, cName);
        }
    }
}
public class MatchData
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
public class KnockoutMatchData : MatchData
{
    public KnockoutDepth Depth { get; set; } = KnockoutDepth.None;
    public int Index { get; set; } = default;
    public MatchId? LeftChildMatchId { get; set; }
    public MatchId? RightChildMatchId { get; set; }

    [JsonIgnore] public KnockoutMatchData? LeftMatch { get; set; }
    [JsonIgnore] public KnockoutMatchData? RightMatch { get; set; }
}


