using ProjectWorldCup.FifaLibrary;
using ProjectWorldCup.Pages.Wc2026;

namespace Tests.WorldCup;

public class Wc2026ScenarioBettingSimulatorTest
{
    [Fact]
    public void Simulation_uses_current_scenario_top_two_without_mutating_original_bettings()
    {
        var groups = Wc2026ScenarioGroup.CreateGroups(new[]
        {
            CreateMatch("A조", 1, "A", "B", 1, 0),
            CreateMatch("A조", 2, "C", "D", 1, 0),
            CreateMatch("A조", 3, "C", "A", 1, 0),
            CreateMatch("A조", 4, "D", "B", 1, 0),
            CreateMatch("A조", 5, "C", "B", 1, 0),
            CreateMatch("A조", 6, "D", "A", 1, 0),
        });
        var originalBettingItem = new WcBettingItem<GroupTeam>
        {
            User = new AppUser { Id = new UserId("user-1"), UserName = "tester" },
            Rank = 9,
            Reward = 123,
            Picked = new[] { CreateGroupTeam("A"), CreateGroupTeam("C") },
            Fixed = new[] { CreateGroupTeam("A") },
        };

        var simulationItems = Wc2026ScenarioBettingSimulator.CreateSimulationItems(
            new[] { originalBettingItem },
            groups);
        var result = new BettingResultTable<IWcBettingItem<ITeam>>(simulationItems).Single();

        Assert.Equal(1, result.Score);
        Assert.Equal(new[] { "C", "D" }, result.Fixed.Select(team => team.Id).ToArray());
        Assert.Equal("A", Assert.Single(originalBettingItem.Fixed).Id);
        Assert.Equal(9, originalBettingItem.Rank);
        Assert.Equal(123, originalBettingItem.Reward);
    }

    [Fact]
    public void Fixed_teams_use_country_id_for_betting_comparison_when_fifa_team_id_differs()
    {
        var groups = Wc2026ScenarioGroup.CreateGroups(new[]
        {
            CreateMatch("A조", 1, "fifa-a", "A", "AAA", "fifa-b", "B", "BBB", 1, 0),
        });
        var bettingItem = new WcBettingItem<GroupTeam>
        {
            User = new AppUser { Id = new UserId("user-1"), UserName = "tester" },
            Picked = new[] { CreateGroupTeam("A") },
        };

        var result = new BettingResultTable<IWcBettingItem<ITeam>>(
            Wc2026ScenarioBettingSimulator.CreateSimulationItems(new[] { bettingItem }, groups))
            .Single();

        Assert.Equal(1, result.Score);
        Assert.Equal("A", result.Fixed.First().Id);
        Assert.Equal("fifa-a", ((GroupTeam)result.Fixed.First()).FifaTeamId);
    }

    private static GroupTeam CreateGroupTeam(string id)
    {
        return new GroupTeam
        {
            Id = id,
            GroupName = "A조",
            Name = id,
            Flag = $"flag-{id}",
        };
    }

    private static FifaMatchData CreateMatch(
        string groupName,
        int matchNumber,
        string homeId,
        string awayId,
        int? homeScore = null,
        int? awayScore = null)
    {
        return CreateMatch(
            groupName,
            matchNumber,
            homeId,
            homeId,
            homeId,
            awayId,
            awayId,
            awayId,
            homeScore,
            awayScore);
    }

    private static FifaMatchData CreateMatch(
        string groupName,
        int matchNumber,
        string homeTeamId,
        string homeCountryId,
        string homeAbbreviation,
        string awayTeamId,
        string awayCountryId,
        string awayAbbreviation,
        int? homeScore = null,
        int? awayScore = null)
    {
        return new FifaMatchData
        {
            IdGroup = groupName,
            GroupName = new List<FifaIdName>
            {
                new FifaIdName { Locale = "ko-KR", Description = groupName }
            },
            IdMatch = $"match-{matchNumber}",
            MatchNumber = matchNumber,
            Date = new DateTime(2026, 6, 12, 0, 0, 0, DateTimeKind.Utc).AddHours(matchNumber),
            Home = CreateTeam(homeTeamId, homeCountryId, homeAbbreviation),
            Away = CreateTeam(awayTeamId, awayCountryId, awayAbbreviation),
            HomeTeamScore = homeScore,
            AwayTeamScore = awayScore,
        };
    }

    private static FifaMatchTeam CreateTeam(
        string teamId,
        string countryId,
        string abbreviation)
    {
        return new FifaMatchTeam
        {
            IdTeam = teamId,
            IdCountry = countryId,
            Abbreviation = abbreviation,
            PictureUrl = $"flag-{countryId}",
            TeamName = new List<FifaIdName>
            {
                new FifaIdName { Locale = "ko-KR", Description = countryId }
            },
        };
    }
}
