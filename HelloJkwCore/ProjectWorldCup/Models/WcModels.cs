namespace ProjectWorldCup;

public class GroupTeam : Team
{
    public string GroupName { get; set; }
    public string Placement { get; set; }

    public static GroupTeam CreateFromFifaMatchTeam(FifaMatchTeam matchTeam, string groupName, string placeholder)
    {
        return new GroupTeam
        {
            GroupName = groupName,
            //Placeholder = placeholder,
            Id = matchTeam?.Abbreviation ?? placeholder,
            Name = matchTeam?.TeamName ?? placeholder,
            Flag = matchTeam?.PictureUrl.Replace("{format}", "sq").Replace("{size}", "2"),
        };
    }
}

public class GroupMatch : Match<GroupTeam>
{
    public string GroupName { get; set; }

    public static GroupMatch CreateFromFifaMatchData(FifaMatchData matchData, List<GroupTeam> teams)
    {
        try
        {
            var homeTeam = teams.First(x => x.Id == matchData.Home.Abbreviation);
            var awayTeam = teams.First(x => x.Id == matchData.Away.Abbreviation);

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
        catch
        {
            var homeTeam = teams.FirstOrDefault(x => x.Id == matchData.Home.CountryId);
            var awayTeam = teams.FirstOrDefault(x => x.Id == matchData.Away.CountryId);

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
}

public class KnMatch : Match<Team>
{
    public static KnMatch CreateFromFifaMatchData(FifaMatchData fifaMatchData)
    {
        return new KnMatch
        {
            Time = fifaMatchData.Date,
            Status = MatchStatus.Before,
            HomeTeam = new Team { Id = fifaMatchData.Home?.CountryId, Name = fifaMatchData.Home?.TeamName, Flag = fifaMatchData.Home.PictureUrl },
            AwayTeam = new Team { Id = fifaMatchData.Away?.CountryId, Name = fifaMatchData.Away?.TeamName, Flag = fifaMatchData.Away.PictureUrl },
            Info = new()
            {
                [MatchInfoType.MatchNumber] = fifaMatchData.MatchNumber.ToString(),
            },
        };
    }

}
