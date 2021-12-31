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

        public bool IsDraw => HomeScore == AwayScore && HomePenaltyScore == AwayPenaltyScore;
        public (Team Team, int Score, int PenaltyScore) Winner => HomeScore != AwayScore
            ? (HomeScore > AwayScore ? (HomeTeam, HomeScore, HomePenaltyScore) : (AwayTeam, AwayScore, AwayPenaltyScore))
            : (HomePenaltyScore > AwayPenaltyScore ? (HomeTeam, HomeScore, HomePenaltyScore) : (AwayTeam, AwayScore, AwayPenaltyScore));
        public (Team Team, int Score, int PenaltyScore) Looser => HomeScore != AwayScore
            ? (HomeScore < AwayScore ? (HomeTeam, HomeScore, HomePenaltyScore) : (AwayTeam, AwayScore, AwayPenaltyScore))
            : (HomePenaltyScore < AwayPenaltyScore ? (HomeTeam, HomeScore, HomePenaltyScore) : (AwayTeam, AwayScore, AwayPenaltyScore));
    }
}
