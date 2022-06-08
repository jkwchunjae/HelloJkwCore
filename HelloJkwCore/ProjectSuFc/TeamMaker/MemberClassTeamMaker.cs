namespace ProjectSuFc;

public class MemberClassTeamMaker : TeamMaker
{
    public override Task<TeamResult> MakeTeamAsync(List<MemberName> names, int teamCount, TeamSettingOption option)
    {
        TeamResult result = null;
        int resultScore = 0;

        foreach (var _ in Enumerable.Range(1, 100))
        {
            var randomResult = MakeClassedTeam(names, teamCount, option);
            var randomScore = CalcTeamResult(randomResult, option);

            if (result == null || resultScore < randomScore)
            {
                result = randomResult;
                resultScore = randomScore;
            }
        }

        return Task.FromResult(result);
    }

    private static TeamResult MakeClassedTeam(List<MemberName> names, int teamCount, TeamSettingOption option)
    {
        var classNames = (option.ClassOptions ?? Enumerable.Empty<MergeSplitOption>())
            .Select((option, index) => option.Names.RandomShuffle().Select(name => (name, index)))
            .SelectMany(names => names)
            .GroupBy(x => x.name, (key, names) => (name: key, index: names.Min(x => x.index)))
            .ToList();

        var filterdClassNames = classNames
            .Where(name => names.Contains(name.name))
            .ToList();
        var notClassedNames = names // names 중에 class 정의가 안 되어있는 회원
            .Where(name => !filterdClassNames.Any(x => x.name == name))
            .RandomShuffle()
            .ToList();

        var result = new TeamResult(teamCount);

        var teamIndex = 0;
        foreach (var (name, index) in filterdClassNames)
        {
            var teamName = result.TeamNames[teamIndex % result.TeamNames.Count];
            result.Players.Add((name, teamName));

            teamIndex++;
        }
        foreach (var name in notClassedNames)
        {
            var teamName = result.TeamNames[teamIndex % result.TeamNames.Count];
            result.Players.Add((name, teamName));

            teamIndex++;
        }

        return result;
    }

    private static int CalcTeamResult(TeamResult result, TeamSettingOption option)
    {
        // 점수가 높으면 잘 됐다는 뜻.
        // 분리해야 하는데 합쳐져 있으면 -1
        // 합쳐야 하는데 안 합쳐져 있으면 -1

        var score = 0;

        if (option?.SplitOptions?.Any() ?? false)
        {
            foreach (var splitOption in option.SplitOptions.Where(x => x.Filled))
            {
                var teams = splitOption.Names
                    .Select(name => result.Players.FirstOrDefault(x => x.MemberName == name).TeamName)
                    .ToList();
                var splitScore = teams.Count - teams.Distinct().Count();

                score -= splitScore;
            }
        }

        if (option?.MergeOptions?.Any() ?? false)
        {
            foreach (var mergeOption in option.MergeOptions.Where(x => x.Filled))
            {
                var teams = mergeOption.Names
                    .Select(name => result.Players.FirstOrDefault(x => x.MemberName == name).TeamName)
                    .ToList();

                var mergeScore = teams.Distinct().Count() - 1;

                score -= mergeScore;
            }
        }

        return score;
    }
}
