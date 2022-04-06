namespace ProjectWorldCup;

public class League<TMatch, TTeam> where TMatch : Match<TTeam> where TTeam : Team
{
    public string Name { get; set; }
    public IReadOnlyList<TTeam> Teams => _teams;
    public IReadOnlyList<TMatch> Matches => _matches;
    [JsonIgnore]
    public IReadOnlyList<TeamStanding<TTeam>> Stands
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

    private List<TTeam> _teams = new();
    private List<TMatch> _matches = new();
    private List<TeamStanding<TTeam>> _standings;

    private List<TeamStanding<TTeam>> CalcStandings()
    {
        var list = Teams.Select(team => new TeamStanding<TTeam> { Team = team }).ToList();

        foreach (var match in Matches.Where(x => x.Status == MatchStatus.Done))
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

        var sortedTeamList = list
            .OrderByDescending(x => x.Point)
            .ThenByDescending(x => x.Gd)
            .ThenByDescending(x => x.Gf)
            .ToList();

        var rank = 1;
        foreach (var team in sortedTeamList)
        {
            team.Rank = rank++;
        }

        return sortedTeamList;
    }

    public bool AddTeam(TTeam team)
    {
        if (_teams.Empty(x => x.Id == team.Id))
        {
            _teams.Add(team);
            return true;
        }
        return false;
    }

    public bool AddMatch(TMatch match)
    {
        var homeTeam = Teams.FirstOrDefault(x => x.Id == match.HomeTeam.Id);
        var awayTeam = Teams.FirstOrDefault(x => x.Id == match.AwayTeam.Id);

        if (homeTeam != null && awayTeam != null)
        {
            _matches.Add(match);
            return true;
        }
        return false;
    }
}
