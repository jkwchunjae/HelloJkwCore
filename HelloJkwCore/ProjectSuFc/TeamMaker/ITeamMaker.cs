using JkwExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSuFc
{
    interface ITeamMaker
    {
        Task<TeamResult> MakeTeamAsync(List<MemberName> names, int teamCount, TeamSettingOption option);
    }

    public abstract class TeamMaker : ITeamMaker
    {
        public abstract Task<TeamResult> MakeTeamAsync(List<MemberName> names, int teamCount, TeamSettingOption option);

        protected List<(MemberName Name, TeamName Team)> RandomShuffle(List<MemberName> names, List<TeamName> teams)
        {
            var shuffled = names
                .RandomShuffle()
                .Select((x, i) => new { Name = x, Team = teams[i % teams.Count], })
                .Select(x => (x.Name, x.Team))
                .OrderBy(x => x.Name)
                .ToList();

            return shuffled;
        }

        protected List<TeamName> MakeTeamNameList(int teamCount)
        {
            var alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var teams = Enumerable.Range(0, teamCount).Select(x => new TeamName { Id = alpha.Substring(x, 1) }).ToList();
            return teams;
        }

        protected List<TeamName> MakeRandomTeam(int userCount, int teamCount)
        {
            var teams = MakeTeamNameList(teamCount);

            var result = Enumerable.Range(0, userCount)
                .Select((_, i) => teams[i % teams.Count])
                .RandomShuffle()
                .ToList();

            return result;
        }
    }
}
