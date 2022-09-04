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

    public async Task<List<KnMatch>> GetKnockOutStageMatchesAsync()
    {
        var matches = await _fifa.GetKnockoutStageMatchesAsync();

        return matches
            .Select(x => KnMatch.CreateFromFifaMatchData(x)).ToList();
    }

    public async Task<List<KnMatch>> GetRound16MatchesAsync()
    {
        var matches = await _fifa.GetKnockoutStageMatchesAsync();
        var standing = await _fifa.GetStandingDataAsync();
        return matches
            .Where(match => match.IdStage == Fifa.Round16StageId)
            .Select(m =>
            {
                var match = KnMatch.CreateFromFifaMatchData(m);
                if (m.Home == null && !string.IsNullOrEmpty(m.PlaceHolderA))
                {
                    var standingData = standing.FirstOrDefault(s => s.GroupName.Contains(m.PlaceHolderA.Right(1)) && s.Position == m.PlaceHolderA.Left(1).ToInt());
                    if (standingData != null)
                    {
                        match.HomeTeam = new Team
                        {
                            Name = standingData.TeamName,
                            Id = standingData.TeamLogo.Right(3),
                            Flag = standingData.TeamLogo.Replace("{format}", "sq").Replace("{size}", "2"),
                        };
                    }
                }
                if (m.Away == null && !string.IsNullOrEmpty(m.PlaceHolderB))
                {
                    var standingData = standing.FirstOrDefault(s => s.GroupName.Contains(m.PlaceHolderB.Right(1)) && s.Position == m.PlaceHolderB.Left(1).ToInt());
                    if (standingData != null)
                    {
                        match.AwayTeam = new Team
                        {
                            Name = standingData.TeamName,
                            Id = standingData.TeamLogo.Right(3),
                            Flag = standingData.TeamLogo.Replace("{format}", "sq").Replace("{size}", "2"),
                        };
                    }
                }
                return match;
            })
            .ToList();
    }
}