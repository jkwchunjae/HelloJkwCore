using JkwExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSuFc
{
    public class FullRandomTeamMaker : ITeamMaker
    {
        public TeamResult MakeTeam(List<MemberName> members, int teamCount)
        {
            var alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var teams = Enumerable.Range(0, teamCount).Select(x => new TeamName { Id = alpha.Substring(x, 1) }).ToList();

            var shuffled = members
                .RandomShuffle()
                .Select((x, i) => new { Name = x, Team = teams[i % teamCount], })
                .Select(x => (x.Name, x.Team))
                .OrderBy(x => x.Name)
                .ToList();

            return new TeamResult
            {
                TeamNames = teams,
                Players = shuffled,
            };
        }
    }
}
