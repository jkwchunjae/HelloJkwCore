using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSuFc
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ScheduleMemberStatus
    {
        None,
        Yes,
        No,
        NotYet,
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ScheduleStatus
    {
        Feature,
        Active,
        Done,
    }

    public class ScheduleMember
    {
        public MemberName Name { get; set; }
        public ScheduleMemberStatus Status { get; set; } = ScheduleMemberStatus.None;

        public ScheduleMember() { }

        public ScheduleMember(MemberName name)
        {
            Name = name;
        }
    }

    public class ScheduleData
    {
        public string Title { get; set; } = "정기모임";
        public DateTime Date { get; set; }
        public string Location { get; set; } = "석수체육공원";
        public string Time { get; set; }
        public ScheduleStatus Status { get; set; } = ScheduleStatus.Feature;
        public List<ScheduleMember> Members { get; set; } = new();
        public string TeamTitle { get; set; }
    }
}
