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
}
public class CompetitionUpdator
{
    CompetitionData _competitionData;
    IPpService _service;

    public CompetitionUpdator(CompetitionData competitionData, IPpService service)
    {
        _competitionData = competitionData;
        _service = service;
    }

    public async Task<CompetitionData> AddLeague(LeagueData leagueData)
    {
        var competitionData = await _service.UpdateCompetitionAsync(_competitionData.Name, competitionData =>
        {
            competitionData.LeagueIdList ??= new();
            competitionData.LeagueIdList.Add(leagueData.Id);
            competitionData.LeagueList ??= new();
            competitionData.LeagueList.Add(leagueData);

            return competitionData;
        });

        if (competitionData != null)
        {
            _competitionData = competitionData;
        }
        return _competitionData;
    }

    public async Task<CompetitionData> AddKnockout(KnockoutData knockoutData)
    {
        var competitionData = await _service.UpdateCompetitionAsync(_competitionData.Name, competitionData =>
        {
            competitionData.KnockoutIdList ??= new();
            competitionData.KnockoutIdList.Add(knockoutData.Id);
            competitionData.KnockoutList ??= new();
            competitionData.KnockoutList.Add(knockoutData);

            return competitionData;
        });

        if (competitionData != null)
        {
            _competitionData = competitionData;
        }
        return _competitionData;
    }
}
