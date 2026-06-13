using ProjectWorldCup.FifaLibrary;
using ProjectWorldCup.Pages.Wc2026;

namespace Tests.WorldCup;

public class Wc2026ScenarioRound32SimulatorTest
{
    [Fact]
    public void Third_place_combination_table_contains_all_495_rows()
    {
        Assert.Equal(495, Wc2026ScenarioRound32Simulator.ThirdPlaceCombinationCount);
    }

    [Fact]
    public void Simulation_uses_wiki_combination_for_top_eight_third_place_groups()
    {
        var groups = Wc2026ScenarioGroup.CreateGroups(
            Enumerable.Range(0, 12)
                .SelectMany(index => CreateGroupMatches((char)('A' + index))));

        var simulation = Wc2026ScenarioRound32Simulator.CreateSimulation(groups);

        Assert.True(simulation.CanSimulate);
        Assert.Equal(495, simulation.CombinationNumber);
        Assert.Equal("ABCDEFGH", simulation.ThirdPlaceGroupKey);
        Assert.Equal(16, simulation.Matches.Count);

        var match73 = simulation.Matches.Single(match => match.MatchNumber == 73);
        Assert.Equal("A2", match73.HomeTeam.Id);
        Assert.Equal("B2", match73.AwayTeam.Id);
        Assert.Equal("2A", match73.HomeSlot);
        Assert.Equal("2B", match73.AwaySlot);

        var match74 = simulation.Matches.Single(match => match.MatchNumber == 74);
        Assert.Equal("E1", match74.HomeTeam.Id);
        Assert.Equal("C3", match74.AwayTeam.Id);
        Assert.Equal("1E", match74.HomeSlot);
        Assert.Equal("3C", match74.AwaySlot);

        var match79 = simulation.Matches.Single(match => match.MatchNumber == 79);
        Assert.Equal("A1", match79.HomeTeam.Id);
        Assert.Equal("H3", match79.AwayTeam.Id);
        Assert.Equal("1A", match79.HomeSlot);
        Assert.Equal("3H", match79.AwaySlot);
    }

    [Fact]
    public void Simulation_does_not_run_when_any_group_is_all_three_draws()
    {
        var groups = Wc2026ScenarioGroup.CreateGroups(
            Enumerable.Range(0, 12)
                .SelectMany(index => CreateGroupMatches((char)('A' + index), blankScores: index == 2)));

        var simulation = Wc2026ScenarioRound32Simulator.CreateSimulation(groups);

        Assert.False(simulation.CanSimulate);
        Assert.Empty(simulation.Matches);
        Assert.Contains("C조", simulation.BlockReason);
    }

    private static IEnumerable<FifaMatchData> CreateGroupMatches(char group, bool blankScores = false)
    {
        var scores = blankScores
            ? new (int? HomeScore, int? AwayScore)[]
            {
                (null, null),
                (null, null),
                (null, null),
                (null, null),
                (null, null),
                (null, null),
            }
            : new (int? HomeScore, int? AwayScore)[]
            {
                (3, 0),
                (1, 0),
                (3, 0),
                (0, 2),
                (3, 0),
                (2, 0),
            };

        var matchups = new[]
        {
            ("1", "2"),
            ("3", "4"),
            ("1", "3"),
            ("4", "2"),
            ("1", "4"),
            ("2", "3"),
        };

        return matchups.Select((matchup, index) =>
        {
            var matchNumber = (group - 'A') * 6 + index + 1;
            return CreateMatch(
                $"{group}조",
                matchNumber,
                $"{group}{matchup.Item1}",
                $"{group}{matchup.Item2}",
                scores[index].HomeScore,
                scores[index].AwayScore);
        });
    }

    private static FifaMatchData CreateMatch(
        string groupName,
        int matchNumber,
        string homeId,
        string awayId,
        int? homeScore,
        int? awayScore)
    {
        return new FifaMatchData
        {
            IdGroup = groupName,
            GroupName = new List<FifaIdName>
            {
                new FifaIdName { Locale = "ko-KR", Description = groupName },
            },
            IdMatch = $"match-{matchNumber}",
            MatchNumber = matchNumber,
            Date = new DateTime(2026, 6, 12, 0, 0, 0, DateTimeKind.Utc).AddHours(matchNumber),
            Home = CreateTeam(homeId),
            Away = CreateTeam(awayId),
            HomeTeamScore = homeScore,
            AwayTeamScore = awayScore,
        };
    }

    private static FifaMatchTeam CreateTeam(string id)
    {
        return new FifaMatchTeam
        {
            IdTeam = $"fifa-{id}",
            IdCountry = id,
            Abbreviation = id,
            PictureUrl = $"flag-{id}",
            TeamName = new List<FifaIdName>
            {
                new FifaIdName { Locale = "ko-KR", Description = id },
            },
        };
    }
}
