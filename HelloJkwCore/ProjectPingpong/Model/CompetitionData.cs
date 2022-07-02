namespace ProjectPingpong;

[JsonConverter(typeof(StringIdJsonConverter<CompetitionName>))]
public class CompetitionName : StringName
{
    public static readonly CompetitionName Default = new CompetitionName(string.Empty);
    public CompetitionName() { }
    public CompetitionName(string name)
        : base(name)
    {
    }
}
public class CompetitionData
{
    public CompetitionName Name { get; set; } = CompetitionName.Default;
    public UserId? Owner { get; set; }
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

    private async Task<CompetitionData> UpdateAsync(Func<CompetitionData, CompetitionData> funcUpdate)
    {
        var competitionData = await _service.UpdateCompetitionAsync(
            _competitionData.Name, funcUpdate);

        if (competitionData != null)
        {
            _competitionData = competitionData;
        }
        return _competitionData;

    }

    public Task<CompetitionData> AddLeague(LeagueData leagueData)
    {
        return UpdateAsync(competitionData =>
        {
            competitionData.LeagueIdList ??= new();
            competitionData.LeagueIdList.Add(leagueData.Id);
            competitionData.LeagueList ??= new();
            competitionData.LeagueList.Add(leagueData);

            return competitionData;
        });
    }
    public Task<CompetitionData> AddLeagues(IEnumerable<LeagueData> leagues)
    {
        return UpdateAsync(competitionData =>
        {
            competitionData.LeagueIdList ??= new();
            competitionData.LeagueIdList.AddRange(leagues.Select(l => l.Id));
            competitionData.LeagueList ??= new();
            competitionData.LeagueList.AddRange(leagues);

            return competitionData;
        });
    }
    public Task<CompetitionData> RemoveLeague(LeagueId leagueId)
    {
        return UpdateAsync(competitionData =>
        {
            competitionData.LeagueIdList ??= new();
            competitionData.LeagueIdList.RemoveAll(id => id == leagueId);
            competitionData.LeagueList ??= new();
            competitionData.LeagueList.RemoveAll(x => x.Id == leagueId);

            return competitionData;
        });
    }

    public Task<CompetitionData> AddKnockout(KnockoutData knockoutData)
    {
        return UpdateAsync(competitionData =>
        {
            competitionData.KnockoutIdList ??= new();
            competitionData.KnockoutIdList.Add(knockoutData.Id);
            competitionData.KnockoutList ??= new();
            competitionData.KnockoutList.Add(knockoutData);

            return competitionData;
        });
    }
    public Task<CompetitionData> RemoveKnockout(KnockoutId knockoutId)
    {
        return UpdateAsync(competitionData =>
        {
            competitionData.KnockoutIdList ??= new();
            competitionData.KnockoutIdList.RemoveAll(id => id == knockoutId);
            competitionData.KnockoutList ??= new();
            competitionData.KnockoutList.RemoveAll(x => x.Id == knockoutId);

            return competitionData;
        });
    }

    public Task<CompetitionData> AddPlayers(IEnumerable<Player> players)
    {
        return UpdateAsync(competitionData =>
        {
            competitionData.PlayerList ??= new();

            // 중복된 항목 처리
            var duplicated = competitionData.PlayerList.Join(
                players,
                prev => prev.Name,
                next => next.Name,
                (prev, next) => (prev, next)).ToList();
            foreach (var (prev, next) in duplicated)
            {
                prev.Class = next.Class;
            }

            // 진짜 추가된 유저 추가
            var newPlayers = players.Where(p => competitionData.PlayerList.Empty(pp => pp.Name == p.Name)).ToList();
            competitionData.PlayerList = competitionData.PlayerList
                .Concat(newPlayers)
                .ToList();

            return competitionData;
        });
    }
    public Task<CompetitionData> RemovePlayer(PlayerName playerName)
    {
        return UpdateAsync(competitionData =>
        {
            competitionData.PlayerList ??= new();
            competitionData.PlayerList.RemoveAll(p => p.Name == playerName);

            return competitionData;
        });
    }
}
