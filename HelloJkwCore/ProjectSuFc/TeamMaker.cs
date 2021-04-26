using System;
using System.Collections.Generic;
using System.Text;
using JkwExtensions;

namespace ProjectSuFc
{

    public class TeamMakerService
    {
        private readonly Dictionary<TeamMakerStrategy, ITeamMaker> _strategy = new();

        public TeamMakerService()
        {
            _strategy.Add(TeamMakerStrategy.FullRandom, new FullRandomTeamMaker());
        }
        public TeamResult MakeTeam(List<Member> player, int teamCount, TeamMakerStrategy strategy)
        {
            if (_strategy.ContainsKey(strategy))
            {
                var teamMaker = _strategy[strategy];
                return teamMaker.MakeTeam(player, teamCount);
            }
            return null;
        }
    }
}
