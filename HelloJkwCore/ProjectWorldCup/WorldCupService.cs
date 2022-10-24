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

        return matches
            .Where(match => match.IdStage == Fifa.Round16StageId)
            .Select(match => KnMatch.CreateFromFifaMatchData(match))
            .ToList();
    }

    public async Task<List<KnMatch>> GetQuarterFinalMatchesAsync()
    {
        var matches = await _fifa.GetFinalMatchesAsync();

        return matches
            .Where(match => match.IdStage == Fifa.Round8StageId)
            .Select(match => KnMatch.CreateFromFifaMatchData(match))
            .ToList();
    }

    public async Task<List<KnMatch>> GetFinalMatchesAsync()
    {
        var matches = await _fifa.GetFinalMatchesAsync();

        return matches
            .Where(match => match.IdStage != Fifa.Round16StageId)
            .Select(match => KnMatch.CreateFromFifaMatchData(match))
            .ToList();
    }
}