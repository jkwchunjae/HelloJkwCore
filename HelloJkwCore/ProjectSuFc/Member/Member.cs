using System;
using System.Collections.Generic;

namespace ProjectSuFc
{
    public class Member
    {
        public int No { get; set; }
        public string Name { get; set; }
        public DateTime RegisterDate { get; set; }
        public DateTime Birthday { get; set; }
        public List<string> ConnectIdList { get; set; } = new();
        public List<SeasonId> JoinedSeason { get; set; } = new();
    }
}
