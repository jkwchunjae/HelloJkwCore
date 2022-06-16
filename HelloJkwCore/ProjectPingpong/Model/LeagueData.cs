namespace ProjectPingpong;

internal class LeagueId : StringId
{
    public static readonly LeagueId Default = new LeagueId(string.Empty);
    public LeagueId(string id)
        : base(id)
    {
    }
}
internal class LeagueData
{
    public LeagueId Id { get; set; } = LeagueId.Default;
    public CompetitionName CompetitionName { get; set; } = CompetitionName.Default;
    public List<Player>? PlayerList { get; set; }
    public List<MatchId>? MatchIdList { get; set; }
    [JsonIgnore] public List<MatchData>? MatchList { get; set; }
}
