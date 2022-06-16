namespace ProjectPingpong;

internal class KnockoutId : StringId
{
    public static readonly KnockoutId Default = new KnockoutId(string.Empty);
    public KnockoutId(string id)
        : base(id)
    {
    }
}
internal class KnockoutData
{
    public KnockoutId Id { get; set; } = KnockoutId.Default;
    public CompetitionName CompetitionName { get; set; } = CompetitionName.Default;
    public List<Player>? PlayerList { get; set; }
    public List<MatchId>? MatchIdList { get; set; }
    [JsonIgnore] public List<KnockoutMatchData>? MatchList { get; set; }
}
