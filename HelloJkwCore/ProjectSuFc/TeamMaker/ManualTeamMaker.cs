using JkwExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSuFc
{
    public class ManualTeamMaker : ITeamMaker
    {
        public TeamResult MakeTeam(List<MemberName> members, int teamCount)
        {
            var alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var teams = Enumerable.Range(0, teamCount).Select(x => new TeamName { Id = alpha.Substring(x, 1) }).ToList();

            return new TeamResult
            {
                TeamNames = teams,
                Players = new(),
            };
        }
    }
}
