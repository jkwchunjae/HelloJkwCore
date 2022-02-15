namespace Tests.WorldCup;

public class Result2018Test
{
    IWorldCupService _worldCupService;

    public Result2018Test()
    {
        IFifa fifa = new Fifa();

        var pathOption = new PathMap
        {
            Local = new()
            {
                [WorldCupPath.Result2018GroupStage] = "C:/Users/jkwch/Dropbox/hellojkw/jkw/games/Worldcup/BettingData/groupstage.json",
                [WorldCupPath.Result2018Round16] = "C:/Users/jkwch/Dropbox/hellojkw/jkw/games/Worldcup/BettingData/round16.json",
                [WorldCupPath.Result2018Final] = "C:/Users/jkwch/Dropbox/hellojkw/jkw/games/Worldcup/BettingData/final.json",
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

        var worldcupOption = new WorldCupOption
        {
            FileSystemSelect = new FileSystemSelectOption
            {
                UseMainFileSystem = false,
                FileSystemType = FileSystemType.Local,
            },
            FileSystemSelect2018 = new FileSystemSelectOption
            {
                UseMainFileSystem = false,
                FileSystemType = FileSystemType.Local,
            },
            Path = pathOption,
        };

        var fsService = new FileSystemService(fsOption, null, null);

        _worldCupService = new WorldCupService(fsService, worldcupOption, fifa);
    }

    [Fact]
    public async Task LoadGroupStageTest()
    {
        var list = await _worldCupService.Get2018GroupStageBettingResult();

        Assert.Equal(11, list.Count);
    }

    [Fact]
    public async Task CalcGroupStageTest()
    {
        var list = await _worldCupService.Get2018GroupStageBettingResult();

        var result = new BettingResultTable<WcBettingItem>(list, new BettingTableOption
        {
            RewardForUser = reward =>
            {
                return (reward / 10) * 10; // 1의 자리 내림
            },
        });

        Assert.Equal(11170, result.First(x => x.User.UserName == "jkwchunjae@gmail.com").Reward);
        Assert.Equal(8590, result.First(x => x.User.UserName == "jkw4289@gmail.com").Reward);
        Assert.Equal(10310, result.First(x => x.User.UserName == "tmdgus7960@gmail.com").Reward);
        Assert.Equal(10310, result.First(x => x.User.UserName == "sunin1206@gmail.com").Reward);
        Assert.Equal(8590, result.First(x => x.User.UserName == "hyunjong.lee.s@gmail.com").Reward);
        Assert.Equal(10310, result.First(x => x.User.UserName == "utaeyeon@gmail.com").Reward);
        Assert.Equal(10310, result.First(x => x.User.UserName == "ginasuace@gmail.com").Reward);
        Assert.Equal(8590, result.First(x => x.User.UserName == "lucia").Reward);
        Assert.Equal(10310, result.First(x => x.User.UserName == "haesolwon@gmail.com").Reward);
        Assert.Equal(11170, result.First(x => x.User.UserName == "seojisu205@gmail.com").Reward);
        Assert.Equal(10310, result.First(x => x.User.UserName == "taeheeyu@gmail.com").Reward);
    }
}
