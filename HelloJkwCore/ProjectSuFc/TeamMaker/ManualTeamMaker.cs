namespace ProjectSuFc;

public class ManualTeamMaker : TeamMaker
{
    public override Task<TeamResult> MakeTeamAsync(List<MemberName> members, int teamCount, TeamSettingOption option)
    {
        return Task.FromResult(new TeamResult(teamCount));
    }
}