using ProjectWorldCup.FifaLibrary;
using ProjectWorldCup.Pages.Wc2026;

namespace Tests.WorldCup;

public class FifaApiMatchRowTest
{
    [Fact]
    public void Group_stage_scheduled_match_shows_team_names_and_scheduled_result()
    {
        var match = new FifaMatchData
        {
            Date = new DateTime(2026, 6, 11, 19, 0, 0, DateTimeKind.Utc),
            MatchNumber = 1,
            StageName = Names("1차 예선"),
            GroupName = Names("A조"),
            Home = Team("멕시코", "MEX"),
            Away = Team("남아프리카공화국", "RSA"),
        };

        var row = FifaApiMatchRow.Create(match);

        Assert.Equal("멕시코", row.HomeName);
        Assert.Equal("남아프리카공화국", row.AwayName);
        Assert.Equal("예정", row.ResultText);
        Assert.Equal("A조", row.StageGroupText);
        Assert.Equal(new DateTime(2026, 6, 12, 4, 0, 0, DateTimeKind.Utc), row.KickoffKst);
    }

    [Fact]
    public void Knockout_scheduled_match_shows_fifa_placeholders()
    {
        var match = new FifaMatchData
        {
            Date = new DateTime(2026, 6, 28, 19, 0, 0, DateTimeKind.Utc),
            MatchNumber = 73,
            StageName = Names("32강"),
            GroupName = new List<FifaIdName>(),
            PlaceHolderA = "2A",
            PlaceHolderB = "2B",
        };

        var row = FifaApiMatchRow.Create(match);

        Assert.Equal("2A", row.HomeName);
        Assert.Equal("2B", row.AwayName);
        Assert.True(row.HomeIsPlaceholder);
        Assert.True(row.AwayIsPlaceholder);
        Assert.Equal("예정", row.ResultText);
    }

    [Fact]
    public void Finished_match_shows_regular_and_penalty_scores()
    {
        var match = new FifaMatchData
        {
            Date = new DateTime(2026, 7, 19, 19, 0, 0, DateTimeKind.Utc),
            MatchNumber = 104,
            StageName = Names("결승전"),
            GroupName = new List<FifaIdName>(),
            Home = Team("홈", "HOM"),
            Away = Team("원정", "AWY"),
            HomeTeamScore = 1,
            AwayTeamScore = 1,
            HomeTeamPenaltyScore = 4,
            AwayTeamPenaltyScore = 3,
        };

        var row = FifaApiMatchRow.Create(match);

        Assert.Equal("1 - 1 (PK 4 - 3)", row.ResultText);
    }

    [Fact]
    public void Rows_are_numbered_by_display_order_after_time_sort()
    {
        var matches = new List<FifaMatchData>
        {
            new FifaMatchData
            {
                Date = new DateTime(2026, 6, 13, 1, 0, 0, DateTimeKind.Utc),
                MatchNumber = 40,
                StageName = Names("1차 예선"),
                GroupName = Names("A조"),
                Home = Team("늦은 홈", "LAT"),
                Away = Team("늦은 원정", "LAA"),
            },
            new FifaMatchData
            {
                Date = new DateTime(2026, 6, 11, 19, 0, 0, DateTimeKind.Utc),
                MatchNumber = 99,
                StageName = Names("1차 예선"),
                GroupName = Names("A조"),
                Home = Team("빠른 홈", "EAR"),
                Away = Team("빠른 원정", "EAA"),
            },
        };

        var rows = FifaApiMatchRow.CreateRows(matches);

        Assert.Equal(1, rows[0].Number);
        Assert.Equal(99, rows[0].FifaMatchNumber);
        Assert.Equal(2, rows[1].Number);
        Assert.Equal(40, rows[1].FifaMatchNumber);
    }

    private static List<FifaIdName> Names(string description)
    {
        return new List<FifaIdName>
        {
            new FifaIdName
            {
                Locale = "ko-KR",
                Description = description,
            },
        };
    }

    private static FifaMatchTeam Team(string name, string country)
    {
        return new FifaMatchTeam
        {
            IdCountry = country,
            IdTeam = country,
            PictureUrl = $"https://example.com/{country}.png",
            TeamName = Names(name),
            Abbreviation = country,
        };
    }
}
