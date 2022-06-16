namespace ProjectPingpong;

internal class CompetitionName : StringName
{
    public static readonly CompetitionName Default = new CompetitionName(string.Empty);
    public CompetitionName(string name)
        : base(name)
    {
    }
}
internal class CompetitionData
{
    public CompetitionName Name { get; set; } = CompetitionName.Default;
    public List<Player>? PlayerList { get; set; }
    public List<LeagueId>? LeagueIdList { get; set; }
    public List<KnockoutId>? KnockoutIdList { get; set; }
    [JsonIgnore] public List<LeagueData>? LeagueList { get; set; }
    [JsonIgnore] public List<KnockoutData>? KnockoutList { get; set; }
}
