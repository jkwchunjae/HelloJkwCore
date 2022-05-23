namespace ProjectSuFc;

public class SettingOptionTeamMaker : TeamMaker
{
    public override async Task<TeamResult> MakeTeamAsync(List<MemberName> names, int teamCount, TeamSettingOption option)
    {
        var result = await MakeTeam_Internal(names, teamCount, option);
        return result;
    }

    public async Task<TeamResult> MakeTeam_Internal(List<MemberName> names, int teamCount, TeamSettingOption option)
    {
        var ramdomTeamMaker = new FullRandomTeamMaker();

        TeamResult result = null;
        int resultScore = 0;

        foreach (var _ in Enumerable.Range(1, 100))
        {
            var randomResult = await ramdomTeamMaker.MakeTeamAsync(names, teamCount, option);
            var randomScore = CalcTeamResult(randomResult, option);

            if (result == null || resultScore < randomScore)
            {
                result = randomResult;
                resultScore = randomScore;
            }
        }

        return result;
    }

    private int CalcTeamResult(TeamResult result, TeamSettingOption option)
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