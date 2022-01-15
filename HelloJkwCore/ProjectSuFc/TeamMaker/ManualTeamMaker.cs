using JkwExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSuFc;

public class ManualTeamMaker : TeamMaker
{
    public override Task<TeamResult> MakeTeamAsync(List<MemberName> members, int teamCount, TeamSettingOption option)
    {
        return Task.FromResult(new TeamResult(teamCount));
    }
}