namespace ProjectWorldCup;

public class GroupTeam : Team
{
    public string Placeholder { get; set; }
    public string GroupName { get; set; }

    public static GroupTeam CreateFromFifaMatchTeam(FifaMatchTeam matchTeam, string placeholder)
    {
        return new GroupTeam
        {
            GroupName = GetGroupName(placeholder),
            Placeholder = GetPlaceholder(placeholder),
            Id = matchTeam?.Abbreviation ?? placeholder,
            Name = matchTeam?.TeamName ?? placeholder,
            Flag = matchTeam?.PictureUrl.Replace("{format}", "sq").Replace("{size}", "2"),
        };

        string GetPlaceholder(string ph)
        {
            if (ph == "EUR")
                return "B4";
            if (ph == "ICP 1")
                return "D2";
            if (ph == "ICP 2")
                return "E2";
            return ph;
        }

        string GetGroupName(string ph)
        {
            return $"{GetPlaceholder(ph).Left(1)}";
        }
    }
}

public class GroupMatch : Match<GroupTeam>
{
    public string GroupName { get; set; }

    public static GroupMatch CreateFromFifaMatchData(FifaMatchData matchData, List<GroupTeam> teams)
    {
        var homePlaceholder = GetPlaceholder(matchData.PlaceholderA);
        var awayPlaceholder = GetPlaceholder(matchData.PlaceholderB);

        var homeTeam = teams.First(x => x.Placeholder == homePlaceholder);
        var awayTeam = teams.First(x => x.Placeholder == awayPlaceholder);

        var placeholder = GetPlaceholder(matchData.PlaceholderA);

        var match = new GroupMatch()
        {
            GroupName = homeTeam.GroupName,
            HomeTeam = homeTeam,
            AwayTeam = awayTeam,
            HomeScore = matchData.HomeTeam?.Score ?? 0,
            AwayScore = matchData.AwayTeam?.Score ?? 0,
            Status = MatchStatus.Before,
            Time = matchData.Date,
            Info = new()
            {
                [MatchInfoType.MatchNumber] = matchData.MatchNumber.ToString(),
            },
        };

        return match;

        string GetPlaceholder(string ph)
        {
            if (ph == "EUR")
                return "B4";
            if (ph == "ICP 1")
                return "D2";
            if (ph == "ICP 2")
                return "E2";
            return ph;
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
            HomeTeam = new Team { Id = fifaMatchData.PlaceholderA, Name = fifaMatchData.PlaceholderA },
            AwayTeam = new Team { Id = fifaMatchData.PlaceholderB, Name = fifaMatchData.PlaceholderB },
            Info = new()
            {
                [MatchInfoType.MatchNumber] = fifaMatchData.MatchNumber.ToString(),
            },
        };
    }

}
