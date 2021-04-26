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
        public TeamResult MakeTeam(List<Member> members, int teamCount)
        {
            var alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var teams = Enumerable.Range(0, teamCount).Select(x => alpha.Substring(x, 1)).ToList();

            var shuffled = members
                .RandomShuffle()
                .Select((x, i) => new { Player = x, Team = teams[i % teamCount], })
                .Select(x => (x.Player, x.Team))
                .ToList();

            return new TeamResult
            {
                TeamNames = teams,
                Players = shuffled,
            };
        }
    }
}
