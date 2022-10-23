namespace ProjectWorldCup;

public class GroupTeam : Team
{
    public string GroupName { get; set; }
    public string Placement { get; set; }
}

public class GroupMatch : Match<GroupTeam>
{
    public string GroupName { get; set; }

    public static GroupMatch CreateFromFifaMatchData(FifaMatchData matchData, List<GroupTeam> teams)
    {
        try
        {
            var homeTeam = teams.First(x => x.Id == matchData.Home.IdCountry);
            var awayTeam = teams.First(x => x.Id == matchData.Away.IdCountry);

            var match = new GroupMatch()
            {
                GroupName = homeTeam.GroupName,
                HomeTeam = homeTeam,
                AwayTeam = awayTeam,
                HomeScore = matchData.HomeTeamScore ?? 0,
                AwayScore = matchData.HomeTeamScore ?? 0,
                Status = MatchStatus.Before,
                Time = matchData.Date,
                Info = new()
                {
                    [MatchInfoType.MatchNumber] = matchData.MatchNumber.ToString(),
                },
            };

            return match;
        }
        catch
        {
            var homeTeam = teams.FirstOrDefault(x => x.Id == matchData.Home.IdCountry);
            var awayTeam = teams.FirstOrDefault(x => x.Id == matchData.Away.IdCountry);

            var match = new GroupMatch()
            {
                GroupName = homeTeam.GroupName,
                HomeTeam = homeTeam,
                AwayTeam = awayTeam,
                HomeScore = matchData.Home?.Score ?? 0,
                AwayScore = matchData.Away?.Score ?? 0,
                Status = MatchStatus.Before,
                Time = matchData.Date,
                Info = new()
                {
                    [MatchInfoType.MatchNumber] = matchData.MatchNumber.ToString(),
                },
            };

            return match;
        }
    }
}

public class WcGroup : League<GroupMatch, GroupTeam>
{
    public void WriteStanding(List<FifaStandingData> fifaStandings)
    {
        var standings = Teams
            .Select(team =>
            {
                var fifaStanding = fifaStandings.FirstOrDefault(s => s.Team.IdCountry == team.Id);
                var standing = new TeamStanding<GroupTeam> { Team = team };
                standing.Rank = fifaStanding?.Position ?? default;
                standing.Won = fifaStanding?.Won ?? default;
                standing.Drawn = fifaStanding?.Drawn ?? default;
                standing.Lost = fifaStanding?.Lost ?? default;
                standing.Gf = fifaStanding?.For ?? default;
                standing.Ga = fifaStanding?.Against ?? default;
                return standing;
            })
            .OrderBy(s => s.Rank)
            .ToList();

        _standings = standings;
    }
}

public class KnMatch : Match<Team>
{
    public string StageId { get; private set; }
    public string MatchId { get; private set; }
    public static KnMatch CreateFromFifaMatchData(FifaMatchData fifaMatchData)
    {
        return new KnMatch
        {
            StageId = fifaMatchData.IdStage,
            MatchId = fifaMatchData.IdMatch,
            Time = fifaMatchData.Date,
            Status = MatchStatus.Before,
            HomeTeam = new Team { Id = fifaMatchData.Home?.IdCountry, Name = fifaMatchData.Home?.TeamName[0].Description, Flag = fifaMatchData.Home?.PictureUrl },
            AwayTeam = new Team { Id = fifaMatchData.Away?.IdCountry, Name = fifaMatchData.Away?.TeamName[0].Description, Flag = fifaMatchData.Away?.PictureUrl },
            Info = new()
            {
                [MatchInfoType.MatchNumber] = fifaMatchData.MatchNumber.ToString(),
            },
        };
    }

    public KnMatch()
    {
    }

    public KnMatch(KnMatch match)
        : base(match)
    {
        StageId = match.StageId;
        MatchId = match.MatchId;
    }
}
