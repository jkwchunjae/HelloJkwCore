namespace ProjectPingpong;

public class CompetitionName : StringName
{
    public static readonly CompetitionName Default = new CompetitionName(string.Empty);
    public CompetitionName(string name)
        : base(name)
    {
    }
}
public class CompetitionData
{
    public CompetitionName Name { get; set; } = CompetitionName.Default;
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public List<Player>? PlayerList { get; set; }
    public List<LeagueId>? LeagueIdList { get; set; }
    public List<KnockoutId>? KnockoutIdList { get; set; }
    [JsonIgnore] public List<LeagueData>? LeagueList { get; set; }
    [JsonIgnore] public List<KnockoutData>? KnockoutList { get; set; }

    public static async Task<CompetitionData?> AddLeague(IPpService service, CompetitionName competitionName, LeagueData leagueData)
    {
        return await service.UpdateCompetitionAsync(competitionName, competitionData =>
        {
            competitionData.LeagueIdList ??= new();
            competitionData.LeagueIdList.Add(leagueData.Id);
            competitionData.LeagueList ??= new();
            competitionData.LeagueList.Add(leagueData);

            return competitionData;
        });
    }

    public static async Task<CompetitionData?> AddKnockout(IPpService service, CompetitionName competitionName, KnockoutData knockoutData)
    {
        return await service.UpdateCompetitionAsync(competitionName, competitionData =>
        {
            competitionData.KnockoutIdList ??= new();
            competitionData.KnockoutIdList.Add(knockoutData.Id);
            competitionData.KnockoutList ??= new();
            competitionData.KnockoutList.Add(knockoutData);

            return competitionData;
        });
    }
}
