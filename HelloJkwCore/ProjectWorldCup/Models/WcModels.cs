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

    public static KnMatch CreateFromFifaMatchData(FifaMatchData fifaMatchData, List<FifaStandingData> standings)
    {
        fifaMatchData.Home ??= GetTeam(fifaMatchData.PlaceHolderA);
        fifaMatchData.Away ??= GetTeam(fifaMatchData.PlaceHolderB);

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

        FifaMatchTeam GetTeam(string placeholder)
        {
            var position = placeholder?.Left(1).ToInt();
            var group = placeholder?.Right(1);

            var sTeam = standings
                .FirstOrDefault(x => x.Position == position && x.Group.First().Description.Right(1) == group)
                ?.Team;

            if (sTeam == null)
                return default;

            var matchTeam = new FifaMatchTeam
            {
                IdTeam = sTeam.IdTeam,
                IdCountry = sTeam.IdCountry,
                TeamName = sTeam.Name,
                PictureUrl = sTeam.PictureUrl,
            };
            return matchTeam;
        }
    }
    public static KnMatch CreateFromFifaMatchData(FifaMatchData fifaMatchData, List<FifaMatchData> prevMatches)
    {
        fifaMatchData.Home ??= GetTeam(fifaMatchData.PlaceHolderA);
        fifaMatchData.Away ??= GetTeam(fifaMatchData.PlaceHolderB);

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

        FifaMatchTeam GetTeam(string placeholder)
        {
            var matchNumber = placeholder?.Right(2).ToInt();
            var type = placeholder?.Left(1); // W, L
            var match = prevMatches.FirstOrDefault(m => m.MatchNumber == matchNumber);
            if (match == null)
                return default;

            var homeScore = match.HomeTeamScore * 1000 + match.HomeTeamPenaltyScore;
            var awayScore = match.AwayTeamScore * 1000 + match.AwayTeamPenaltyScore;

            if (type == "W")
            {
                return homeScore > awayScore ? match.Home : match.Away;
            }
            else
            {
                return homeScore > awayScore ? match.Away : match.Home;
            }
        }
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
