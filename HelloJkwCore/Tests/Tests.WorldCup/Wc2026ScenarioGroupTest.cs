using ProjectWorldCup.FifaLibrary;
using ProjectWorldCup.Pages.Wc2026;

namespace Tests.WorldCup;

public class Wc2026ScenarioGroupTest
{
    [Fact]
    public void CreateGroups_uses_official_results_and_entered_scores_for_standings()
    {
        var matches = new List<FifaMatchData>
        {
            CreateMatch("A조", 1, "A", "B", 1, 0),
            CreateMatch("A조", 2, "C", "D"),
            CreateMatch("A조", 3, "A", "C"),
            CreateMatch("A조", 4, "B", "D"),
            CreateMatch("A조", 5, "A", "D"),
            CreateMatch("A조", 6, "B", "C"),
        };

        var group = Wc2026ScenarioGroup.CreateGroups(matches).Single();
        var remaining = group.RemainingMatches.ToList();

        remaining[0].SetHomeScore(2);
        remaining[0].SetAwayScore(0);
        remaining[1].SetHomeScore(0);
        remaining[1].SetAwayScore(3);
        remaining[2].SetHomeScore(1);
        remaining[2].SetAwayScore(1);
        remaining[3].SetHomeScore(2);
        remaining[3].SetAwayScore(0);
        remaining[4].SetHomeScore(2);
        remaining[4].SetAwayScore(2);

        var standings = group.Standings;

        Assert.Equal(5, remaining.Count);
        Assert.Equal("C", standings[0].Team.Id);
        Assert.Equal(2, standings[0].Won);
        Assert.Equal(1, standings[0].Drawn);
        Assert.Equal(0, standings[0].Lost);
        Assert.Equal("2 / 1 / 0", standings[0].RecordText);
        Assert.Equal(7, standings[0].Points);
        Assert.Equal(5, standings[0].GoalDifference);
        Assert.Equal(7, standings[0].GoalsFor);
        Assert.Equal(2, standings[0].GoalsAgainst);
    }

    [Fact]
    public void Scenario_scores_are_clamped_to_single_digits()
    {
        var group = Wc2026ScenarioGroup
            .CreateGroups(new[] { CreateMatch("A조", 1, "A", "B") })
            .Single();
        var match = group.RemainingMatches.Single();

        match.SetHomeScore(14);
        match.SetAwayScore(-1);

        Assert.Equal(9, match.HomeScore);
        Assert.Equal(0, match.AwayScore);
    }

    [Fact]
    public void Groups_are_sorted_from_a_to_l()
    {
        var groups = Wc2026ScenarioGroup.CreateGroups(new[]
        {
            CreateMatch("Group L", 1, "L1", "L2"),
            CreateMatch("Group A", 2, "A1", "A2"),
        });

        Assert.Equal("A조", groups[0].Name);
        Assert.Equal("L조", groups[1].Name);
    }

    [Fact]
    public void Standings_use_head_to_head_before_goal_difference()
    {
        var group = Wc2026ScenarioGroup.CreateGroups(new[]
        {
            CreateMatch("A조", 1, "A", "B", 1, 0),
            CreateMatch("A조", 2, "A", "C", 0, 5),
            CreateMatch("A조", 3, "A", "D", 1, 0),
            CreateMatch("A조", 4, "B", "C", 5, 0),
            CreateMatch("A조", 5, "B", "D", 1, 0),
            CreateMatch("A조", 6, "C", "D", 0, 1),
        }).Single();

        var standings = group.Standings;

        Assert.Equal(6, standings[0].Points);
        Assert.Equal(6, standings[1].Points);
        Assert.Equal("A", standings[0].Team.Id);
        Assert.Equal("B", standings[1].Team.Id);
        Assert.True(standings[1].GoalDifference > standings[0].GoalDifference);
    }

    [Fact]
    public void Standings_ignore_head_to_head_when_head_to_head_is_drawn()
    {
        var group = Wc2026ScenarioGroup.CreateGroups(new[]
        {
            CreateMatch("A조", 1, "A", "B", 0, 0),
            CreateMatch("A조", 2, "A", "C", 1, 0),
            CreateMatch("A조", 3, "A", "D", 1, 0),
            CreateMatch("A조", 4, "B", "C", 3, 0),
            CreateMatch("A조", 5, "B", "D", 3, 0),
            CreateMatch("A조", 6, "C", "D", 0, 0),
        }).Single();

        var standings = group.Standings;

        Assert.Equal(7, standings[0].Points);
        Assert.Equal(7, standings[1].Points);
        Assert.Equal("B", standings[0].Team.Id);
        Assert.Equal("A", standings[1].Team.Id);
        Assert.True(standings[0].GoalDifference > standings[1].GoalDifference);
    }

    [Fact]
    public void Standings_use_tied_team_matches_for_three_way_ties()
    {
        var group = Wc2026ScenarioGroup.CreateGroups(new[]
        {
            CreateMatch("A조", 1, "A", "B", 1, 0),
            CreateMatch("A조", 2, "B", "C", 2, 0),
            CreateMatch("A조", 3, "C", "A", 3, 0),
            CreateMatch("A조", 4, "A", "D", 1, 0),
            CreateMatch("A조", 5, "B", "D", 5, 0),
            CreateMatch("A조", 6, "C", "D", 1, 0),
        }).Single();

        var standings = group.Standings;

        Assert.Equal(new[] { "C", "B", "A", "D" }, standings.Select(standing => standing.Team.Id).ToArray());
        Assert.Equal(6, standings[0].Points);
        Assert.Equal(6, standings[1].Points);
        Assert.Equal(6, standings[2].Points);
        Assert.True(standings[1].GoalDifference > standings[0].GoalDifference);
    }

    private static FifaMatchData CreateMatch(
        string groupName,
        int matchNumber,
        string homeId,
        string awayId,
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
            IdTeam = id,
            IdCountry = id,
            PictureUrl = $"flag-{id}",
            TeamName = new List<FifaIdName>
            {
                new FifaIdName { Locale = "ko-KR", Description = id }
            },
        };
    }
}
