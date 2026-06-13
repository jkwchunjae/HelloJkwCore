using ProjectWorldCup.FifaLibrary;
using ProjectWorldCup.Pages.Wc2026;

namespace Tests.WorldCup;

public class Wc2026ScenarioStorageTest
{
    [Fact]
    public async Task Saved_scores_are_applied_only_to_remaining_matches()
    {
        var fs = CreateFileSystem();
        var storage = new Wc2026ScenarioStorage(fs);
        var user = new AppUser
        {
            Id = new UserId("user-1"),
            UserName = "login-user",
            NickName = "테스터",
        };

        var originalGroups = Wc2026ScenarioGroup.CreateGroups(new[]
        {
            CreateMatch("Group A", 1, "MEX", "KOR"),
            CreateMatch("Group A", 2, "CAN", "USA"),
        });
        originalGroups[0].Matches[0].SetHomeScore(9);
        originalGroups[0].Matches[0].SetAwayScore(0);
        originalGroups[0].Matches[1].SetHomeScore(4);
        originalGroups[0].Matches[1].SetAwayScore(2);

        await storage.SaveAsync(user, originalGroups);

        var refreshedGroups = Wc2026ScenarioGroup.CreateGroups(new[]
        {
            CreateMatch("Group A", 1, "MEX", "KOR", 1, 1),
            CreateMatch("Group A", 2, "CAN", "USA"),
        });

        var savedScenario = await storage.LoadAsync(user);
        Wc2026ScenarioGroup.ApplySavedScenario(refreshedGroups, savedScenario);

        var officialMatch = refreshedGroups[0].Matches[0];
        var remainingMatch = refreshedGroups[0].Matches[1];

        Assert.Equal(1, officialMatch.HomeScore);
        Assert.Equal(1, officialMatch.AwayScore);
        Assert.Equal(4, remainingMatch.HomeScore);
        Assert.Equal(2, remainingMatch.AwayScore);

        await storage.SaveAsync(user, refreshedGroups);

        var savedData = await fs.ReadJsonAsync<Wc2026ScenarioSaveData>(ScenarioPath(user.Id));
        var savedMatch = Assert.Single(savedData.Matches);

        Assert.Equal("user-1", savedData.User.Id);
        Assert.Equal("login-user", savedData.User.UserName);
        Assert.Equal("테스터", savedData.User.NickName);
        Assert.Equal("A", savedMatch.Group);
        Assert.Equal(2, savedMatch.MatchNumber);
        Assert.Equal("CAN", savedMatch.HomeTeam);
        Assert.Equal("USA", savedMatch.AwayTeam);
        Assert.Equal(4, savedMatch.HomeScore);
        Assert.Equal(2, savedMatch.AwayScore);
    }

    private static InMemoryFileSystem CreateFileSystem()
    {
        var pathMap = new PathMap
        {
            InMemory = new Dictionary<string, string>
            {
                [WorldCupPath.Betting2026Scenarios] = "/scenarios",
            },
        };

        var paths = new Paths(pathMap, FileSystemType.InMemory);
        var serializer = new Json(Enumerable.Empty<System.Text.Json.Serialization.JsonConverter>());
        return new InMemoryFileSystem(paths, serializer);
    }

    private static Func<Paths, string> ScenarioPath(StringId userId)
        => path => path[WorldCupPath.Betting2026Scenarios] + $"/{userId}.json";

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
            Abbreviation = id,
            PictureUrl = $"flag-{id}",
            TeamName = new List<FifaIdName>
            {
                new FifaIdName { Locale = "ko-KR", Description = id }
            },
        };
    }
}
