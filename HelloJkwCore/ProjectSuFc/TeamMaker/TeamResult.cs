using JkwExtensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectSuFc
{
    public class TeamResult
    {
        public List<string> TeamNames { get; set; } = new();
        public List<(Member Player, string TeamName)> Players { get; set; } = new();

        [JsonIgnore]
        public Dictionary<string, List<Member>> GroupByTeam => Players
            .GroupBy(x => x.TeamName)
            .Select(x => new { TeamName = x.Key, List = x.Select(e => e.Player).ToList() })
            .ToDictionary(x => x.TeamName, x => x.List);

        [JsonIgnore]
        public int MaximumTeamSize => GroupByTeam.MaxOrNull(x => x.Value.Count) ?? 0;
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum TeamResultStatus
    {
        Feature,
        Active,
        Deprecated,
    }

    public class TeamResultSaveFile
    {
        public string Title { get; set; }
        public DateTime CreateTime { get; set; }
        public TeamResultStatus Status { get; set; }
        public TeamResult Result { get; set; }
    }
}
