namespace ProjectPingpong;

internal class MatchId : StringId
{
    public static readonly MatchId Default = new MatchId(string.Empty);
    public MatchId(string id)
        : base(id)
    {
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


