using ProjectWorldCup.FifaLibrary;

namespace Tests.WorldCup;

public class FinalBettings
{
    IFileSystemService _fsService;
    WorldCupOption _option;

    public FinalBettings()
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

    List<KnMatch> _finalMatches = new List<KnMatch>
    {
        KnMatch.CreateFromFifaMatchData(new FifaMatchData { IdStage = Fifa.Round8StageId }),
        KnMatch.CreateFromFifaMatchData(new FifaMatchData { IdStage = Fifa.Round8StageId }),
        KnMatch.CreateFromFifaMatchData(new FifaMatchData { IdStage = Fifa.Round8StageId }),
        KnMatch.CreateFromFifaMatchData(new FifaMatchData { IdStage = Fifa.Round8StageId }),
        KnMatch.CreateFromFifaMatchData(new FifaMatchData { IdStage = Fifa.Round4StageId }),
        KnMatch.CreateFromFifaMatchData(new FifaMatchData { IdStage = Fifa.Round4StageId }),
        KnMatch.CreateFromFifaMatchData(new FifaMatchData { IdStage = Fifa.ThirdStageId }),
        KnMatch.CreateFromFifaMatchData(new FifaMatchData { IdStage = Fifa.FinalStageId }),
    };
    [Fact]
    public void EvaluateUserBetting_유저가고른것이없을경우()
    {
        var service = new BettingFinalService(_fsService, null, null, _option);

        var quarters = new List<KnMatch>
        {
            new KnMatch { HomeTeam = new Team { Name = "A" }, AwayTeam = new Team { Name = "B" }, },
            new KnMatch { HomeTeam = new Team { Name = "C" }, AwayTeam = new Team { Name = "D" }, },
            new KnMatch { HomeTeam = new Team { Name = "E" }, AwayTeam = new Team { Name = "F" }, },
            new KnMatch { HomeTeam = new Team { Name = "G" }, AwayTeam = new Team { Name = "H" }, },
        };
        var userBetting = new WcFinalBettingItem<Team>
        {
            Picked = new List<Team> { },
        };

        var result = service.EvaluateUserBetting(quarters, userBetting, _finalMatches);

        Assert.Single(result);
    }

    [Fact]
    public void EvaluateUserBetting_유저의선택이있는경우_4강()
    {
        var service = new BettingFinalService(_fsService, null, null, _option);

        var quarters = new List<KnMatch>
        {
            new KnMatch { HomeTeam = new Team { Name = "A" }, AwayTeam = new Team { Name = "B" }, },
            new KnMatch { HomeTeam = new Team { Name = "C" }, AwayTeam = new Team { Name = "D" }, },
            new KnMatch { HomeTeam = new Team { Name = "E" }, AwayTeam = new Team { Name = "F" }, },
            new KnMatch { HomeTeam = new Team { Name = "G" }, AwayTeam = new Team { Name = "H" }, },
        };
        var userBetting = new WcFinalBettingItem<Team>
        {
            Picked = new List<Team>
            {
                new Team { Name = "F" },
                new Team { Name = "B" },
                new Team { Name = "C" },
                new Team { Name = "H" },
            }
        };

        var result = service.EvaluateUserBetting(quarters, userBetting, _finalMatches);

        Assert.Equal(4, result.Count);

        var semi = result.First(x => x.StageId == Fifa.Round4StageId);
        Assert.Equal(2, semi.Matches.Count);
        Assert.Equal("C", semi.Matches[0].HomeTeam.Name);
        Assert.Equal("B", semi.Matches[0].AwayTeam.Name);
        Assert.Equal("H", semi.Matches[1].HomeTeam.Name);
        Assert.Equal("F", semi.Matches[1].AwayTeam.Name);
    }

    [Fact]
    public void EvaluateUserBetting_유저의선택이있는경우_34위전()
    {
        var service = new BettingFinalService(_fsService, null, null, _option);

        var quarters = new List<KnMatch>
        {
            new KnMatch { HomeTeam = new Team { Name = "A" }, AwayTeam = new Team { Name = "B" }, },
            new KnMatch { HomeTeam = new Team { Name = "C" }, AwayTeam = new Team { Name = "D" }, },
            new KnMatch { HomeTeam = new Team { Name = "E" }, AwayTeam = new Team { Name = "F" }, },
            new KnMatch { HomeTeam = new Team { Name = "G" }, AwayTeam = new Team { Name = "H" }, },
        };
        var userBetting = new WcFinalBettingItem<Team>
        {
            Picked = new List<Team>
            {
                new Team { Name = "F" },
                new Team { Name = "B" },
                new Team { Name = "C" },
                new Team { Name = "H" },
            }
        };

        var result = service.EvaluateUserBetting(quarters, userBetting, _finalMatches);

        Assert.Equal(4, result.Count);

        var third = result.First(x => x.StageId == Fifa.ThirdStageId);
        Assert.Single(third.Matches);
        Assert.Equal("C", third.Matches[0].HomeTeam.Name);
        Assert.Equal("H", third.Matches[0].AwayTeam.Name);
    }

    [Fact]
    public void EvaluateUserBetting_유저의선택이있는경우_결승()
    {
        var service = new BettingFinalService(_fsService, null, null, _option);

        var quarters = new List<KnMatch>
        {
            new KnMatch { HomeTeam = new Team { Name = "A" }, AwayTeam = new Team { Name = "B" }, },
            new KnMatch { HomeTeam = new Team { Name = "C" }, AwayTeam = new Team { Name = "D" }, },
            new KnMatch { HomeTeam = new Team { Name = "E" }, AwayTeam = new Team { Name = "F" }, },
            new KnMatch { HomeTeam = new Team { Name = "G" }, AwayTeam = new Team { Name = "H" }, },
        };
        var userBetting = new WcFinalBettingItem<Team>
        {
            Picked = new List<Team>
            {
                new Team { Name = "F" },
                new Team { Name = "B" },
                new Team { Name = "C" },
                new Team { Name = "H" },
            }
        };

        var result = service.EvaluateUserBetting(quarters, userBetting, _finalMatches);

        Assert.Equal(4, result.Count);

        var final = result.First(x => x.StageId == Fifa.FinalStageId);
        Assert.Single(final.Matches);
        Assert.Equal("B", final.Matches[0].HomeTeam.Name);
        Assert.Equal("F", final.Matches[0].AwayTeam.Name);
    }
}
