using ProjectPingpong;

namespace Tests.Pingpong;

public class CompetitionTest
{
    IPpService _service;
    public CompetitionTest()
    {
        var pathOption = new PathMap
        {
            InMemory = new()
            {
                [PingpongPathType.CompetitionPath] = "/db/com",
                [PingpongPathType.CompetitionListFilePath] = "/db/competitions.json",
                [PingpongPathType.FreeMatchPath] = "/db/free-match",
            },
        };

        var pingpongOption = new PingpongOption
        {
            FileSystemSelect = new FileSystemSelectOption
            {
                FileSystemType = FileSystemType.InMemory,
            },
            Path = pathOption,
        };

        var fsOption = new FileSystemOption();
        IFileSystemService fsService = new FileSystemService(fsOption, null, null);
        IPpMatchService matchService = new PpMatchService(pingpongOption, fsService);
        _service = new PpService(pingpongOption, fsService, matchService);
    }

    [Fact]
    public async Task 대회가생성되어야한다()
    {
        var competitionName = new CompetitionName("test-competition");
        var competitionData = await _service.CreateCompetitionAsync(competitionName);

        Assert.Equal(competitionName, competitionData?.Name);
    }

    [Fact]
    public async Task 대회가생성되면대회목록에나와야한다()
    {
        var competitionName1 = new CompetitionName("test-competition1");
        var competitionData1 = await _service.CreateCompetitionAsync(competitionName1);
        var competitionName2 = new CompetitionName("test-competition2");
        var competitionData2 = await _service.CreateCompetitionAsync(competitionName2);

        var competitions = await _service.GetAllCompetitionsAsync();

        Assert.Equal(2, competitions.Count);
        Assert.Contains(competitions, x => x == competitionName1);
        Assert.Contains(competitions, x => x == competitionName2);
    }

    [Fact]
    public async Task 대회를수정할수있어야한다()
    {
        var competitionName = new CompetitionName("test-competition");
        var competitionData = await _service.CreateCompetitionAsync(competitionName);

        var startTime = DateTime.Now;

        var updated1 = await _service.UpdateCompetitionAsync(competitionName, data =>
        {
            data.StartTime = startTime;
            return data;
        });

        var updated2 = await _service.GetCompetitionDataAsync(competitionName);

        Assert.Equal(startTime, updated1.StartTime);
        Assert.Equal(startTime, updated2.StartTime);
    }

    [Fact]
    public async Task 대회에선수를추가할수있어야한다()
    {
        var competitionName = new CompetitionName("test-competition");
        var competitionData = await _service.CreateCompetitionAsync(competitionName);
        var competitionUpdator = new CompetitionUpdator(competitionData, _service);

        competitionData = await competitionUpdator.AddPlayers(new[]
        {
            new Player { Name = new PlayerName("p1"), Class = 3 },
            new Player { Name = new PlayerName("p2"), Class = 1 },
        });

        Assert.NotNull(competitionData.PlayerList);
        Assert.Equal(2, competitionData.PlayerList.Count);
        Assert.Equal("p1", competitionData.PlayerList[0].Name.Name);
        Assert.Equal(3, competitionData.PlayerList[0].Class);
        Assert.Equal("p2", competitionData.PlayerList[1].Name.Name);
        Assert.Equal(1, competitionData.PlayerList[1].Class);
    }

    [Fact]
    public async Task 대회에선수를수정할수있어야한다()
    {
        var competitionName = new CompetitionName("test-competition");
        var competitionData = await _service.CreateCompetitionAsync(competitionName);
        var competitionUpdator = new CompetitionUpdator(competitionData, _service);

        competitionData = await competitionUpdator.AddPlayers(new[]
        {
            new Player { Name = new PlayerName("p1"), Class = 3 },
            new Player { Name = new PlayerName("p2"), Class = 1 },
        });
        competitionData = await competitionUpdator.AddPlayers(new[]
        {
            new Player { Name = new PlayerName("p1"), Class = 5 },
        });
        competitionData = await competitionUpdator.AddPlayers(new[]
        {
            new Player { Name = new PlayerName("p2"), Class = 9 },
            new Player { Name = new PlayerName("p3"), Class = 1 },
        });

        Assert.NotNull(competitionData.PlayerList);
        Assert.Equal(3, competitionData.PlayerList.Count);
        Assert.Equal("p1", competitionData.PlayerList[0].Name.Name);
        Assert.Equal(5, competitionData.PlayerList[0].Class);
        Assert.Equal("p2", competitionData.PlayerList[1].Name.Name);
        Assert.Equal(9, competitionData.PlayerList[1].Class);
    }

    [Fact]
    public async Task 대회에선수를삭제할수있어야한다()
    {
        var competitionName = new CompetitionName("test-competition");
        var competitionData = await _service.CreateCompetitionAsync(competitionName);
        var competitionUpdator = new CompetitionUpdator(competitionData, _service);

        competitionData = await competitionUpdator.AddPlayers(new[]
        {
            new Player { Name = new PlayerName("p1"), Class = 3 },
            new Player { Name = new PlayerName("p2"), Class = 1 },
        });
        competitionData = await competitionUpdator.RemovePlayer(new PlayerName("p2"));

        Assert.NotNull(competitionData.PlayerList);
        Assert.Single(competitionData.PlayerList);
        Assert.Equal("p1", competitionData.PlayerList[0].Name.Name);
        Assert.Equal(3, competitionData.PlayerList[0].Class);
    }

    [Fact]
    public async Task 대회에리그를추가할수있어야한다()
    {
        var competitionName = new CompetitionName("test-competition");
        var competitionData = await _service.CreateCompetitionAsync(competitionName);
        var competitionUpdator = new CompetitionUpdator(competitionData, _service);

        var leagueData = await _service.CreateLeagueAsync(new LeagueId(competitionName, "A"));
        competitionData = await competitionUpdator.AddLeague(leagueData);

        Assert.Contains(leagueData.Id, competitionData.LeagueIdList);
        Assert.Contains(leagueData, competitionData.LeagueList);
    }
    [Fact]
    public async Task 대회에리그를추가할수있어야한다_여러개()
    {
        var competitionName = new CompetitionName("test-competition");
        var competitionData = await _service.CreateCompetitionAsync(competitionName);
        var competitionUpdator = new CompetitionUpdator(competitionData, _service);

        var leagueData1 = await _service.CreateLeagueAsync(new LeagueId(competitionName, "A"));
        var leagueData2 = await _service.CreateLeagueAsync(new LeagueId(competitionName, "B"));
        competitionData = await competitionUpdator.AddLeagues(new[] { leagueData1, leagueData2 });

        Assert.Contains(leagueData1.Id, competitionData.LeagueIdList);
        Assert.Contains(leagueData1, competitionData.LeagueList);
        Assert.Contains(leagueData2.Id, competitionData.LeagueIdList);
        Assert.Contains(leagueData2, competitionData.LeagueList);
    }


    [Fact]
    public async Task 대회에리그를삭제할수있어야한다()
    {
        var competitionName = new CompetitionName("test-competition");
        var competitionData = await _service.CreateCompetitionAsync(competitionName);
        var competitionUpdator = new CompetitionUpdator(competitionData, _service);

        var leagueData = await _service.CreateLeagueAsync(new LeagueId(competitionName, "A"));
        await competitionUpdator.AddLeague(leagueData);
        await competitionUpdator.RemoveLeague(leagueData.Id);

        competitionData = await _service.GetCompetitionDataAsync(competitionName);
        Assert.Empty(competitionData.LeagueIdList);
    }
}
