namespace ProjectWorldCup;

public partial class WorldCupService : IWorldCupService
{
    private readonly IFileSystem _fs;
    private IFifa _fifa;

    public WorldCupService(
        IFileSystemService fsService,
        WorldCupOption option,
        IFifa fifa)
    {
        _fifa = fifa;
        _fs = fsService.GetFileSystem(option.FileSystemSelect, option.Path);
    }

    public async Task<List<Team>> Get2022QualifiedTeamsAsync()
    {
        var qualifiedTeams = await _fifa.GetQualifiedTeamsAsync();
        var rankings = await _fifa.GetLastRankingAsync(Gender.Men);

        var teams = qualifiedTeams
            .Select(team => new { Team = team, Ranking = rankings.FirstOrDefault(x => x.RankingItem.Name == team.Name) })
            .Select(x => new Team
            {
                Id = x.Team.Id,
                Name = x.Team.Name,
                Flag = x.Team.Flag?.Src,
                FifaRank = x.Ranking?.RankingItem.Rank ?? 0,
                Region = x.Ranking?.Region.Text,
            })
            .ToList();

        return teams;
    }

    public Task<List<RankingTeamData>> GetLastRankingTeamDataAsync(Gender gender)
    {
        return _fifa.GetLastRankingAsync(gender);
    }

    //public async Task<KnockoutStageData> GetKnockoutStageDataAsync()
    //{
    //    var knockoutMatches = await _fifa.GetKnockoutStageMatchesAsync();
    //    return new KnockoutStageData
    //    {
    //        Round16 = knockoutMatches.Where(x => x.StageName == "Round of 16").Select(x => KnMatch.CreateFromFifaMatchData(x)).ToList(),
    //        QuarterFinals = knockoutMatches.Where(x => x.StageName == "Quarter-finals").Select(x => KnMatch.CreateFromFifaMatchData(x)).ToList(),
    //        SemiFinals = knockoutMatches.Where(x => x.StageName == "Semi-finals").Select(x => KnMatch.CreateFromFifaMatchData(x)).ToList(),
    //        ThirdPlacePlayOff = knockoutMatches.Where(x => x.StageName == "Play-off for third place").Select(x => KnMatch.CreateFromFifaMatchData(x)).First(),
    //        Final = knockoutMatches.Where(x => x.StageName == "Final").Select(x => KnMatch.CreateFromFifaMatchData(x)).First(),
    //    };
    //}

    private async Task<KnockoutStageData> CreateDummyKnockoutDataAsync()
    {
        var teams = await Get2022QualifiedTeamsAsync();

        teams = teams.Concat(teams).Concat(teams).ToList();
        var index = 0;
        Func<KnMatch> createMatch = () =>
        {
            return new KnMatch
            {
                //HomeTeam = teams[index++],
                //AwayTeam = teams[index++],
                HomeTeam = null,
                AwayTeam = null,
            };
        };
        var data = new KnockoutStageData
        {
            Final = createMatch(),
            ThirdPlacePlayOff = createMatch(),
            SemiFinals = new List<KnMatch> { createMatch(), createMatch() },
            QuarterFinals = new List<KnMatch> { createMatch(), createMatch(), createMatch(), createMatch() },
            Round16 = new List<KnMatch> { createMatch(), createMatch(), createMatch(), createMatch(), createMatch(), createMatch(), createMatch(), createMatch() },
        };

        return data;
    }

    public async Task<List<KnMatch>> GetKnockOutStageMatchesAsync()
    {
        var matches = await _fifa.GetKnockoutStageMatchesAsync();

        return matches.Select(x => KnMatch.CreateFromFifaMatchData(x)).ToList();
    }
}