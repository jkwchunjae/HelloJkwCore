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

    //[Fact]
    public async Task LoadGroupStageTest()
    {
        var list = await _worldCupService.Get2018GroupStageBettingResult();

        Assert.Equal(11, list.Count);
    }

    //[Fact]
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

    //[Fact]
    public async Task CalcFinalResultTest()
    {
        var list = await _worldCupService.Get2018FinalBettingResult();

        var result = new BettingResultTable<WcFinalBettingItem>(list, new BettingTableOption
        {
            RewardForUser = reward =>
            {
                return (reward / 10) * 10; // 1의 자리 내림
            },
        });


        Assert.Equal(26, list.Count);

        Assert.Equal(470, result.First(x => x.User.UserName == "jkwchunjae@gmail.com").Reward);
        Assert.Equal(1410, result.First(x => x.User.UserName == "ginasuace@gmail.com").Reward);
        Assert.Equal(18430, result.First(x => x.User.UserName == "janfirsy@gmail.com").Reward);
        Assert.Equal(1410, result.First(x => x.User.UserName == "skatpgusskat@gmail.com").Reward);
        Assert.Equal(26000, result.First(x => x.User.UserName == "kyh9052@gmail.com").Reward);
        Assert.Equal(24580, result.First(x => x.User.UserName == "haesolwon@gmail.com").Reward);
        Assert.Equal(2830, result.First(x => x.User.UserName == "dsjun12@gmail.com").Reward);
        Assert.Equal(18900, result.First(x => x.User.UserName == "seojisu205@gmail.com").Reward);
        Assert.Equal(20800, result.First(x => x.User.UserName == "tmdgus7960@gmail.com").Reward);
        Assert.Equal(18900, result.First(x => x.User.UserName == "sunin1206@gmail.com").Reward);
        Assert.Equal(3300, result.First(x => x.User.UserName == "solientcg@gmail.com").Reward);
        Assert.Equal(7090, result.First(x => x.User.UserName == "utaeyeon@gmail.com").Reward);
        Assert.Equal(1410, result.First(x => x.User.UserName == "sikinmettugi@gmail.com").Reward);
        Assert.Equal(940, result.First(x => x.User.UserName == "301386@gmail.com").Reward);
        Assert.Equal(18430, result.First(x => x.User.UserName == "khs500kr@gmail.com").Reward);
        Assert.Equal(940, result.First(x => x.User.UserName == "care3728@gmail.com").Reward);
        Assert.Equal(1410, result.First(x => x.User.UserName == "axaxax11222@gmail.com").Reward);
        Assert.Equal(18430, result.First(x => x.User.UserName == "mooming.go@gmail.com").Reward);
        Assert.Equal(8980, result.First(x => x.User.UserName == "trouvailley@gmail.com").Reward);
        Assert.Equal(20800, result.First(x => x.User.UserName == "flckddyd1@gmail.com").Reward);
        Assert.Equal(20800, result.First(x => x.User.UserName == "gpdiadora3@gmail.com").Reward);
        Assert.Equal(17960, result.First(x => x.User.UserName == "junghoonkim").Reward);
        Assert.Equal(3300, result.First(x => x.User.UserName == "breaknow@bluehole.net").Reward);
        Assert.Equal(940, result.First(x => x.User.UserName == "ldh1450@gmail.com").Reward);
        Assert.Equal(470, result.First(x => x.User.UserName == "hinolja83@gmail.com").Reward);
        Assert.Equal(940, result.First(x => x.User.UserName == "lucia").Reward);
    }
}
