namespace ProjectWorldCup.Pages.Wc2026;

public class FifaApiMatchRow
{
    public int Number { get; private set; }
    public int? FifaMatchNumber { get; init; }
    public DateTime KickoffKst { get; init; }
    public string ShortKickoffText => KickoffKst.ToString("d일 H시");
    public string StageGroupText { get; init; } = string.Empty;
    public string HomeName { get; init; } = string.Empty;
    public string HomeFlag { get; init; }
    public bool HomeIsPlaceholder { get; init; }
    public string ResultText { get; init; } = string.Empty;
    public string AwayName { get; init; } = string.Empty;
    public string AwayFlag { get; init; }
    public bool AwayIsPlaceholder { get; init; }

    public static FifaApiMatchRow Create(FifaMatchData match)
    {
        return new FifaApiMatchRow
        {
            FifaMatchNumber = match.MatchNumber,
            KickoffKst = ToKst(match.Date),
            StageGroupText = GetStageGroupText(match),
            HomeName = GetTeamName(match.Home, match.PlaceHolderA),
            HomeFlag = match.Home?.PictureUrl,
            HomeIsPlaceholder = match.Home == null,
            ResultText = GetResultText(match),
            AwayName = GetTeamName(match.Away, match.PlaceHolderB),
            AwayFlag = match.Away?.PictureUrl,
            AwayIsPlaceholder = match.Away == null,
        };
    }

    public static List<FifaApiMatchRow> CreateRows(IEnumerable<FifaMatchData> matches)
    {
        var rows = (matches ?? Enumerable.Empty<FifaMatchData>())
            .Select(Create)
            .OrderBy(row => row.KickoffKst)
            .ThenBy(row => row.FifaMatchNumber)
            .ToList();

        for (var i = 0; i < rows.Count; i++)
        {
            rows[i].Number = i + 1;
        }

        return rows;
    }

    public static string GetResultText(FifaMatchData match)
    {
        var homeScore = match.HomeTeamScore ?? match.Home?.Score;
        var awayScore = match.AwayTeamScore ?? match.Away?.Score;

        if (homeScore == null || awayScore == null)
        {
            return "예정";
        }

        var result = $"{homeScore} - {awayScore}";

        if (match.HomeTeamPenaltyScore != null && match.AwayTeamPenaltyScore != null)
        {
            result += $" (PK {match.HomeTeamPenaltyScore} - {match.AwayTeamPenaltyScore})";
        }

        return result;
    }

    private static DateTime ToKst(DateTime fifaUtcDate)
    {
        var utcDate = fifaUtcDate.Kind == DateTimeKind.Local
            ? fifaUtcDate.ToUniversalTime()
            : DateTime.SpecifyKind(fifaUtcDate, DateTimeKind.Utc);

        return utcDate.AddHours(9);
    }

    private static string GetStageGroupText(FifaMatchData match)
    {
        var stage = GetName(match.StageName);
        var group = GetName(match.GroupName);

        if (!string.IsNullOrWhiteSpace(group))
        {
            return group;
        }

        return stage;
    }

    private static string GetTeamName(FifaMatchTeam team, string placeholder)
    {
        if (team != null)
        {
            var name = GetName(team.TeamName);
            if (!string.IsNullOrWhiteSpace(name))
            {
                return name;
            }

            if (!string.IsNullOrWhiteSpace(team.Abbreviation))
            {
                return team.Abbreviation;
            }
        }

        return string.IsNullOrWhiteSpace(placeholder) ? "TBD" : placeholder;
    }

    private static string GetName(IList<FifaIdName> names)
    {
        return names?
            .FirstOrDefault(name => name.Locale == "ko-KR")?
            .Description
            ?? names?.FirstOrDefault()?.Description
            ?? string.Empty;
    }
}
