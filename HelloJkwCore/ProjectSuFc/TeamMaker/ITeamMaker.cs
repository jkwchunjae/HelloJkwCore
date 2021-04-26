using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectSuFc
{
    interface ITeamMaker
    {
        TeamResult MakeTeam(List<Member> members, int teamCount);
    }
}
