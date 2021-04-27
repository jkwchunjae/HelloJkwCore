using Common;
using JkwExtensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectSuFc
{
    [JsonConverter(typeof(StringIdJsonConverter<TeamName>))]
    public class TeamName : StringId
    {
    }

    public class TeamResult
    {
        public string Title { get; set; }
        public List<TeamName> TeamNames { get; set; } = new();
        public List<(MemberName MemberName, TeamName TeamName)> Players { get; set; } = new();

        [JsonIgnore]
        public Dictionary<TeamName, List<MemberName>> GroupByTeam => Players
            .GroupBy(x => x.TeamName)
            .Select(x => new { TeamName = x.Key, List = x.Select(e => e.MemberName).ToList() })
            .ToDictionary(x => x.TeamName, x => x.List);

        [JsonIgnore]
        public int MaximumTeamSize => GroupByTeam.MaxOrNull(x => x.Value.Count) ?? 0;
    }
}
