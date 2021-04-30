using JkwExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSuFc
{
    public class FullRandomTeamMaker : TeamMaker
    {
        public FullRandomTeamMaker(ISuFcService service) : base(service)
        {
        }

        public override Task<TeamResult> MakeTeamAsync(List<MemberName> members, int teamCount)
        {
            var teamResult = new TeamResult(teamCount);

            var shuffled = RandomShuffle(members, teamResult.TeamNames);

            teamResult.Players = shuffled;
            return Task.FromResult(teamResult);
        }
    }
}
