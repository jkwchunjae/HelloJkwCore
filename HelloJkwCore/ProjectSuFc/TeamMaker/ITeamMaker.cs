using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectSuFc
{
    interface ITeamMaker
    {
        TeamResult MakeTeam(List<MemberName> members, int teamCount);
    }
}
