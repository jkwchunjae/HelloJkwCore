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

    public async Task<List<KnMatch>> GetKnockoutStageMatchesAsync()
    {
        var matches = await _fifa.GetKnockoutStageMatchesAsync();

        return matches
            .Select(match => KnMatch.CreateFromFifaMatchData(match))
            .ToList();
    }

    public async Task<List<KnMatch>> GetRound16MatchesAsync()
    {
        var matches = await _fifa.GetRound16MatchesAsync();
        var groupStanding = await _fifa.GetStandingDataAsync();

        return matches
            .Where(match => match.IdStage == Fifa.Round16StageId)
            .Select(match => KnMatch.CreateFromFifaMatchData(match, groupStanding))
            .ToList();
    }

    public async Task<List<KnMatch>> GetQuarterFinalMatchesAsync()
    {
        var match16 = await _fifa.GetRound16MatchesAsync();
        var matches = await _fifa.GetFinalMatchesAsync();

        return matches
            .Where(match => match.IdStage == Fifa.Round8StageId)
            .Select(match => KnMatch.CreateFromFifaMatchData(match, match16))
            .ToList();
    }

    public async Task<List<KnMatch>> GetFinalMatchesAsync()
    {
        var match16 = await _fifa.GetRound16MatchesAsync();
        var matches = await _fifa.GetFinalMatchesAsync();

        return matches
            .Where(match => match.IdStage != Fifa.Round16StageId)
            .Select(match => KnMatch.CreateFromFifaMatchData(match, match16))
            .ToList();
    }

    public async Task<List<WcGroup>> GetGroupsFromStandingAsync()
    {
        var standings = await _fifa.GetStandingDataAsync();

        var groups = standings
            .GroupBy(s => s.IdGroup)
            .Select(groupStandings =>
            {
                var groupName = groupStandings.First().Group.First().Description;
                var league = new WcGroup { Name = groupName };

                var teams = groupStandings
                    .OrderBy(s => s.Position)
                    .Select(s => new GroupTeam
                    {
                        Id = s.Team.IdCountry,
                        GroupName = groupName,
                        Placement = s.Position.ToString(),
                        Name = s.Team.Name.First().Description,
                        Flag = s.Team.PictureUrl,
                    })
                    .ToList();

                teams.ForEach(team => league.AddTeam(team));
                league.WriteStanding(standings.ToList());

                return league;
            })
            .OrderBy(g => g.Name)
            .ToList();

        return groups;
    }

    public async Task<List<KnMatch>> GetRound32MatchesAsync()
    {
        var matches = await _fifa.GetRound32MatchesAsync();
        var groupStanding = await _fifa.GetStandingDataAsync();

        return matches
            .Where(match => match.IdStage == Fifa.Round32StageId)
            .Select(match => KnMatch.CreateFromFifaMatchData(match, groupStanding))
            .ToList();
    }
}