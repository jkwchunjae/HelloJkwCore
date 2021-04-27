using Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ProjectSuFc
{
    [JsonConverter(typeof(StringIdJsonConverter<MemberName>))]
    public class MemberName : StringId
    {
        [JsonIgnore]
        public string Name => Id;

        public MemberName() { }
        public MemberName(string name)
            : base(name)
        {
        }
    }

    public class Member
    {
        public int No { get; set; }
        public MemberName Name { get; set; }
        public DateTime RegisterDate { get; set; }
        public DateTime Birthday { get; set; }
        public List<string> ConnectIdList { get; set; } = new();
        public List<SeasonId> JoinedSeason { get; set; } = new();
    }
}
