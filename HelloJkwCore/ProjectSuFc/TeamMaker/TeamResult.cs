namespace ProjectSuFc;

[JsonConverter(typeof(StringIdJsonConverter<TeamName>))]
public class TeamName : StringId
{
}

public class TeamResult
{
    public string Title { get; set; }
    public List<TeamName> TeamNames { get; set; } = new();
    public List<(MemberName MemberName, TeamName TeamName)> Players { get; set; } = new();
    public Dictionary<MemberName, double> Score { get; set; } = new();

    [JsonIgnore]
    public Dictionary<TeamName, List<MemberName>> GroupByTeam => Players
        .GroupBy(x => x.TeamName)
        .Select(x => new { TeamName = x.Key, List = x.Select(e => e.MemberName).OrderBy(x => x).ToList() })
        .ToDictionary(x => x.TeamName, x => x.List);

    [JsonIgnore]
    public int MaximumTeamSize => GroupByTeam.MaxOrNull(x => x.Value.Count) ?? 0;

    public TeamResult()
    {
    }

    public TeamResult(int teamCount)
    {
        var alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var teams = Enumerable.Range(0, teamCount).Select(x => new TeamName { Id = alpha.Substring(x, 1) }).ToList();
        TeamNames = teams;
    }
}