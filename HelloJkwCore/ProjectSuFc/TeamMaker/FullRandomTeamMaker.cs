namespace ProjectSuFc;

public class FullRandomTeamMaker : TeamMaker
{
    public override Task<TeamResult> MakeTeamAsync(List<MemberName> members, int teamCount, TeamSettingOption option)
    {
        var teamResult = new TeamResult(teamCount);

        var shuffled = RandomShuffle(members, teamResult.TeamNames);

        teamResult.Players = shuffled;
        return Task.FromResult(teamResult);
    }
}