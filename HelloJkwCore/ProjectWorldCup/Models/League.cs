using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectWorldCup
{
    public class League
    {
        public string Name { get; set; }
        public List<Team> Teams { get; set; }
        public List<Match> Matches { get; set; }
        [JsonIgnore]
        public List<TeamStanding> Stands
        {
            get
            {
                if (_standings == null)
                {
                    _standings = CalcStandings();
                }

                return _standings;
            }
        }

        private List<TeamStanding> _standings;

        private List<TeamStanding> CalcStandings()
        {
            var list = Teams.Select(team => new TeamStanding { Team = team }).ToList();

            foreach (var match in Matches)
            {
                if (match.IsDraw)
                {
                    var team1 = list.FirstOrDefault(x => x.Team.Id == match.HomeTeam.Id);
                    var team2 = list.FirstOrDefault(x => x.Team.Id == match.AwayTeam.Id);

                    if (team1 != null && team2 != null)
                    {
                        team1.Drawn++;
                        team2.Drawn++;

                        team1.Gf += match.HomeScore;
                        team2.Gf += match.AwayScore;
                        team1.Ga += match.AwayScore;
                        team2.Ga += match.HomeScore;
                    }
                }
                else
                {
                    var winner = list.FirstOrDefault(x => x.Team.Id == match.Winner.Team.Id);
                    var looser = list.FirstOrDefault(x => x.Team.Id == match.Looser.Team.Id);

                    if (winner != null && looser != null)
                    {
                        winner.Won++;
                        looser.Lost++;

                        winner.Gf += match.Winner.Score;
                        looser.Gf += match.Looser.Score;
                        winner.Ga += match.Looser.Score;
                        looser.Ga += match.Winner.Score;
                    }
                }
            }

            list = list.OrderByDescending(x => x.Point)
                .ThenByDescending(x => x.Gd)
                .ThenByDescending(x => x.Gf)
                .ToList();

            var rank = 1;
            foreach (var team in list)
            {
                team.Rank = rank++;
            }

            return list;
        }
    }
}
