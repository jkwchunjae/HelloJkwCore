namespace ProjectSuFc;

public class SettingOptionTeamMaker : TeamMaker
{
    public override Task<TeamResult> MakeTeamAsync(List<MemberName> names, int teamCount, TeamSettingOption option)
    {
        var result = MakeTeam_Internal(names, teamCount, option);
        return Task.FromResult(result);
    }

    public TeamResult MakeTeam_Internal(List<MemberName> names, int teamCount, TeamSettingOption option)
    {
        var teamResult = new TeamResult(teamCount);

        foreach (var splitOption in option.SplitOptions)
        {
            var remainNames = splitOption.Names.Where(x => teamResult.Players.Empty(e => e.MemberName == x)).ToList();
            var userCount = remainNames.Count; // userCount = 5, teamCount = 3
            var teamSetCount = userCount / teamCount; // teamSetCount = 1
            var teamNames = Enumerable.Range(1, teamSetCount)
                .SelectMany(_ => MakeTeamNameList(teamCount))
                .ToList(); // ABC
            var additionalTeamNames = MakeTeamNameList(teamCount)
                .RandomShuffle()
                .Take(userCount % teamCount); // BC
            teamNames.AddRange(additionalTeamNames); // ABCBC

            var splitResult = remainNames.Zip(teamNames.RandomShuffle(), (n, t) => (n, t)).ToList();

            teamResult.Players.AddRange(splitResult);
        }

        var remains = names
            .Where(x => teamResult.Players.Empty(e => e.MemberName == x))
            .RandomShuffle()
            .ToList();

        foreach (var name in remains)
        {
            var newTeam = GetNextTeamName(teamResult);

            teamResult.Players.Add((name, newTeam));
        }

        teamResult.Score = teamResult.Players
            .ToDictionary(x => x.MemberName, x => WhereIs(option.SplitOptions, x.MemberName));

        return teamResult;
    }

    private TeamName GetNextTeamName(TeamResult teamResult)
    {
        var ordered = teamResult.TeamNames
            .Select(x => new { TeamName = x, Count = teamResult.Players.Count(e => e.TeamName == x) })
            .OrderBy(x => x.Count)
            .ToList();

        if (ordered.First().Count == ordered.Last().Count)
        {
            // 모든 팀의 멤버 수가 다 같은 상태
            // 아무거나 골라 주자
            var result = ordered
                .RandomShuffle()
                .First()
                .TeamName;
            return result;
        }
        else
        {
            var result = ordered.First().TeamName;
            return result;
        }
    }

    private double WhereIs(List<MergeSplitOption> options, MemberName name)
    {
        return options.Select((x, i) => new { Option = x, Index = i + 1 })
            .FirstOrDefault(x => x.Option.Names.Contains(name))
            ?.Index ?? 0;

    }
}