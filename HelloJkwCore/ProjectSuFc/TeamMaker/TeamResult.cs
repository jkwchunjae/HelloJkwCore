using JkwExtensions;
using System.Collections.Generic;
using System.Linq;

namespace ProjectSuFc
{
    public class TeamResult
    {
        public List<string> TeamNames { get; set; } = new();
        public List<(Member Player, string TeamName)> Players { get; set; } = new();

        public Dictionary<string, List<Member>> GroupByTeam => Players
            .GroupBy(x => x.TeamName)
            .Select(x => new { TeamName = x.Key, List = x.Select(e => e.Player).ToList() })
            .ToDictionary(x => x.TeamName, x => x.List);

        public int MaximumTeamSize => GroupByTeam.MaxOrNull(x => x.Value.Count) ?? 0;
    }
}
