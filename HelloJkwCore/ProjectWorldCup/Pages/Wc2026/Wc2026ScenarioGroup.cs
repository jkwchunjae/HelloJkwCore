namespace ProjectWorldCup.Pages.Wc2026;

public class Wc2026ScenarioGroup
{
    public string Name { get; init; } = "";
    public int SortOrder { get; init; }
    public IReadOnlyList<Wc2026ScenarioTeam> Teams { get; init; } = new List<Wc2026ScenarioTeam>();
    public IReadOnlyList<Wc2026ScenarioMatch> Matches { get; init; } = new List<Wc2026ScenarioMatch>();
    public IEnumerable<Wc2026ScenarioMatch> RemainingMatches => Matches.Where(match => !match.HasOfficialResult);
    public IReadOnlyList<Wc2026ScenarioStanding> Standings => CalculateStandings();

    public static List<Wc2026ScenarioGroup> CreateGroups(IEnumerable<FifaMatchData> matches)
    {
        return (matches ?? Enumerable.Empty<FifaMatchData>())
            .Where(match => match?.Home != null && match.Away != null)
            .GroupBy(match => string.IsNullOrWhiteSpace(match.IdGroup) ? GetRawGroupName(match) : match.IdGroup)
            .Select(CreateGroup)
            .OrderBy(group => group.SortOrder)
            .ThenBy(group => group.Name)
            .ToList();
    }

    private static Wc2026ScenarioGroup CreateGroup(IGrouping<string, FifaMatchData> groupMatches)
    {
        var matches = groupMatches
            .OrderBy(match => match.MatchNumber ?? int.MaxValue)
            .ThenBy(match => match.Date)
            .ToList();

        var rawGroupName = GetRawGroupName(matches.FirstOrDefault()) ?? groupMatches.Key;
        var name = NormalizeGroupName(rawGroupName);
        var teams = new Dictionary<string, Wc2026ScenarioTeam>();

        foreach (var match in matches)
        {
            AddTeam(match.Home);
            AddTeam(match.Away);
        }

        var scenarioMatches = matches
            .Select(match => CreateScenarioMatch(match, teams))
            .Where(match => match != null)
            .OrderBy(match => match.MatchNumber ?? int.MaxValue)
            .ThenBy(match => match.KickoffKst)
            .ToList();

        return new Wc2026ScenarioGroup
        {
            Name = name,
            SortOrder = GetGroupSortOrder(name),
            Teams = teams.Values.ToList(),
            Matches = scenarioMatches
        };

        void AddTeam(FifaMatchTeam team)
        {
            var key = GetTeamKey(team);
            if (!teams.ContainsKey(key))
            {
                teams[key] = new Wc2026ScenarioTeam
                {
                    Id = key,
                    Name = GetTeamName(team),
                    Flag = team.PictureUrl,
                };
            }
        }
    }

    private static Wc2026ScenarioMatch CreateScenarioMatch(
        FifaMatchData match,
        IReadOnlyDictionary<string, Wc2026ScenarioTeam> teams)
    {
        var homeScore = GetHomeScore(match);
        var awayScore = GetAwayScore(match);

        return new Wc2026ScenarioMatch
        {
            Id = match.IdMatch ?? match.MatchNumber?.ToString() ?? Guid.NewGuid().ToString(),
            MatchNumber = match.MatchNumber,
            KickoffKst = ToKst(match.Date),
            HomeTeam = teams[GetTeamKey(match.Home)],
            AwayTeam = teams[GetTeamKey(match.Away)],
            HomeScore = homeScore ?? 0,
            AwayScore = awayScore ?? 0,
            HasOfficialResult = homeScore != null && awayScore != null,
        };
    }

    private IReadOnlyList<Wc2026ScenarioStanding> CalculateStandings()
    {
        var standings = Teams
            .Select(team => new Wc2026ScenarioStanding { Team = team })
            .ToDictionary(standing => standing.Team.Id);

        foreach (var match in Matches)
        {
            var home = standings[match.HomeTeam.Id];
            var away = standings[match.AwayTeam.Id];

            home.GoalsFor += match.HomeScore;
            home.GoalsAgainst += match.AwayScore;
            away.GoalsFor += match.AwayScore;
            away.GoalsAgainst += match.HomeScore;

            if (match.HomeScore > match.AwayScore)
            {
                home.Won++;
                away.Lost++;
            }
            else if (match.HomeScore < match.AwayScore)
            {
                away.Won++;
                home.Lost++;
            }
            else
            {
                home.Drawn++;
                away.Drawn++;
            }
        }

        var teamOrder = Teams
            .Select((team, index) => (team.Id, Index: index))
            .ToDictionary(team => team.Id, team => team.Index);

        var sortedStandings = standings.Values
            .GroupBy(standing => standing.Points)
            .OrderByDescending(group => group.Key)
            .SelectMany(group => SortTiedStandings(group.ToList(), teamOrder))
            .ToList();

        for (var i = 0; i < sortedStandings.Count; i++)
        {
            sortedStandings[i].Rank = i + 1;
        }

        return sortedStandings;
    }

    private IReadOnlyList<Wc2026ScenarioStanding> SortTiedStandings(
        List<Wc2026ScenarioStanding> tiedStandings,
        IReadOnlyDictionary<string, int> teamOrder)
    {
        if (tiedStandings.Count == 1)
        {
            return tiedStandings;
        }

        var tiedTeamIds = tiedStandings
            .Select(standing => standing.Team.Id)
            .ToHashSet();
        var tiedStats = tiedStandings.ToDictionary(
            standing => standing.Team.Id,
            _ => new Wc2026ScenarioTieStanding());

        foreach (var match in Matches.Where(match =>
            tiedTeamIds.Contains(match.HomeTeam.Id) && tiedTeamIds.Contains(match.AwayTeam.Id)))
        {
            ApplyTiedMatch(tiedStats[match.HomeTeam.Id], tiedStats[match.AwayTeam.Id], match);
        }

        return tiedStandings
            .OrderByDescending(standing => tiedStats[standing.Team.Id].Points)
            .ThenByDescending(standing => tiedStats[standing.Team.Id].GoalDifference)
            .ThenByDescending(standing => tiedStats[standing.Team.Id].GoalsFor)
            .ThenByDescending(standing => standing.GoalDifference)
            .ThenByDescending(standing => standing.GoalsFor)
            .ThenBy(standing => teamOrder[standing.Team.Id])
            .ToList();
    }

    private static void ApplyTiedMatch(
        Wc2026ScenarioTieStanding home,
        Wc2026ScenarioTieStanding away,
        Wc2026ScenarioMatch match)
    {
        home.GoalsFor += match.HomeScore;
        home.GoalsAgainst += match.AwayScore;
        away.GoalsFor += match.AwayScore;
        away.GoalsAgainst += match.HomeScore;

        if (match.HomeScore > match.AwayScore)
        {
            home.Won++;
            away.Lost++;
        }
        else if (match.HomeScore < match.AwayScore)
        {
            away.Won++;
            home.Lost++;
        }
        else
        {
            home.Drawn++;
            away.Drawn++;
        }
    }

    private static int? GetHomeScore(FifaMatchData match)
    {
        return match.HomeTeamScore ?? match.Home?.Score;
    }

    private static int? GetAwayScore(FifaMatchData match)
    {
        return match.AwayTeamScore ?? match.Away?.Score;
    }

    private static string GetRawGroupName(FifaMatchData match)
    {
        return GetName(match?.GroupName);
    }

    private static string NormalizeGroupName(string groupName)
    {
        var letter = GetGroupLetter(groupName);
        return letter == null ? groupName : $"{letter}조";
    }

    private static int GetGroupSortOrder(string groupName)
    {
        var letter = GetGroupLetter(groupName);
        return letter == null ? 99 : letter.Value - 'A';
    }

    private static char? GetGroupLetter(string groupName)
    {
        if (string.IsNullOrWhiteSpace(groupName))
        {
            return null;
        }

        var match = Regex.Match(groupName, "([A-L])\\s*조|Group\\s*([A-L])|\\b([A-L])\\b", RegexOptions.IgnoreCase);
        if (!match.Success)
        {
            return null;
        }

        var value = match.Groups
            .Cast<Group>()
            .Skip(1)
            .FirstOrDefault(group => group.Success)
            ?.Value;

        return string.IsNullOrWhiteSpace(value)
            ? null
            : char.ToUpperInvariant(value[0]);
    }

    private static string GetTeamKey(FifaMatchTeam team)
    {
        return team.IdTeam
            ?? team.IdCountry
            ?? GetTeamName(team);
    }

    private static string GetTeamName(FifaMatchTeam team)
    {
        return GetName(team.TeamName)
            ?? team.ShortClubName
            ?? team.Abbreviation
            ?? "TBD";
    }

    private static string GetName(IList<FifaIdName> names)
    {
        return names?
            .FirstOrDefault(name => name.Locale == "ko-KR")?
            .Description
            ?? names?.FirstOrDefault()?.Description;
    }

    private static DateTime ToKst(DateTime fifaUtcDate)
    {
        var utcDate = fifaUtcDate.Kind == DateTimeKind.Local
            ? fifaUtcDate.ToUniversalTime()
            : DateTime.SpecifyKind(fifaUtcDate, DateTimeKind.Utc);

        return utcDate.AddHours(9);
    }
}

public class Wc2026ScenarioTeam
{
    public string Id { get; init; } = "";
    public string Name { get; init; } = "";
    public string Flag { get; init; }
}

public class Wc2026ScenarioMatch
{
    public string Id { get; init; } = "";
    public int? MatchNumber { get; init; }
    public DateTime KickoffKst { get; init; }
    public Wc2026ScenarioTeam HomeTeam { get; init; }
    public Wc2026ScenarioTeam AwayTeam { get; init; }
    public int HomeScore { get; set; }
    public int AwayScore { get; set; }
    public bool HasOfficialResult { get; init; }

    public void SetHomeScore(int score)
    {
        HomeScore = ClampScore(score);
    }

    public void SetAwayScore(int score)
    {
        AwayScore = ClampScore(score);
    }

    private static int ClampScore(int score)
    {
        return Math.Clamp(score, 0, 9);
    }
}

public class Wc2026ScenarioStanding
{
    public int Rank { get; set; }
    public Wc2026ScenarioTeam Team { get; init; }
    public int Won { get; set; }
    public int Drawn { get; set; }
    public int Lost { get; set; }
    public string RecordText => $"{Won} / {Drawn} / {Lost}";
    public int GoalsFor { get; set; }
    public int GoalsAgainst { get; set; }
    public int GoalDifference => GoalsFor - GoalsAgainst;
    public int Points => Won * 3 + Drawn;
}

public class Wc2026ScenarioTieStanding
{
    public int Won { get; set; }
    public int Drawn { get; set; }
    public int Lost { get; set; }
    public int GoalsFor { get; set; }
    public int GoalsAgainst { get; set; }
    public int GoalDifference => GoalsFor - GoalsAgainst;
    public int Points => Won * 3 + Drawn;
}
