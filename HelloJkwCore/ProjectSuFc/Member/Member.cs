using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
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

    [JsonConverter(typeof(StringEnumConverter))]
    public enum MemberSpecType
    {
        AttendProb,
    }

    public class Member
    {
        public int No { get; set; }
        public MemberName Name { get; set; }
        public DateTime RegisterDate { get; set; }
        public DateTime Birthday { get; set; }
        public List<string> ConnectIdList { get; set; } = new();
        public List<SeasonId> JoinedSeason { get; set; } = new();
        public Dictionary<MemberSpecType, double> Spec { get; set; } = new();

        private static Dictionary<MemberSpecType, double> DefaultSpecValue = new Dictionary<MemberSpecType, double>
        {
            [MemberSpecType.AttendProb] = 0.7,
        };

        public double GetSpecValue(MemberSpecType specType)
        {
            if (Spec.TryGetValue(specType, out var value))
            {
                return value;
            }
            else if (DefaultSpecValue.TryGetValue(specType, out var defaultValue))
            {
                return defaultValue;
            }
            else
            {
                return 0;
            }
        }
    }
}
