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

        var fsOption = new FileSystemOption
        {
            MainFileSystem = new MainFileSystemOption
            {
                UseBackup = false,
                MainFileSystem = FileSystemType.InMemory,
            },
        };

        var pingpongOption = new PingpongOption
        {
            FileSystemSelect = new FileSystemSelectOption
            {
                UseMainFileSystem = true,
            },
            Path = pathOption,
        };

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

        await _service.UpdateCompetitionAsync(competitionName, data =>
        {
            data.StartTime = startTime;
            return data;
        });

        var updated = await _service.GetCompetitionDataAsync(competitionName);

        Assert.Equal(startTime, updated.StartTime);
    }
}
