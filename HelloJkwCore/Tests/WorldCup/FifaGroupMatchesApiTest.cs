using ProjectWorldCup.FifaLibrary;

namespace Tests.WorldCup;

public class FifaGroupMatchesApiTest
{
    IFileSystemService _fsService;
    WorldCupOption _option;
    public FifaGroupMatchesApiTest()
    {
        var fsOption = new FileSystemOption
        {
            MainFileSystem = new MainFileSystemOption
            {
                UseBackup = false,
                MainFileSystem = FileSystemType.InMemory,
            },
        };

        _fsService = new FileSystemService(fsOption, null, null);

        _option = new WorldCupOption
        {
            FileSystemSelect = new FileSystemSelectOption
            {
                UseMainFileSystem = true,
            },
            Path = new PathMap
            {
                InMemory = new Dictionary<string, string>
                {
                    ["Cache"] = "fs/cache",
                    ["FifaData"] = "fs/fifadata",
                }
            },
        };
    }

    [Fact]
    public async Task 조별리그가_잘_불러와져야함()
    {
        IFifa fifa = new Fifa(_fsService, _option);
        var groups = await fifa.GetGroupOverview();

        Assert.Equal(8, groups.Count);
        Assert.Equal(32, groups.SelectMany(x => x.Teams).Count());
    }

    [Fact]
    public async Task FifaGroupStageMatches_should_return_48_matches()
    {
        IFifa fifa = new Fifa(_fsService, _option);

        var matches = await fifa.GetGroupStageMatchesAsync();

        Assert.Equal(48, matches.Count());
    }

    [Fact]
    public async Task FifaKnockoutStageMatches_should_return_16_matches()
    {
        IFifa fifa = new Fifa(_fsService, _option);

        var matches = await fifa.GetKnockoutStageMatchesAsync();

        Assert.Equal(16, matches.Count());
    }
}