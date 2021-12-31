using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectWorldCup
{
    public class Match
    {
        public Team HomeTeam { get; set; } = new();
        public Team AwayTeam { get; set; } = new();
        public int HomeScore { get; set; }
        public int AwayScore { get; set; }
        public int HomePenaltyScore { get; set; }
        public int AwayPenaltyScore { get; set; }
    }
}
