using Microsoft.Extensions.Caching.Memory;

namespace ProjectPingpong;

public interface IPpService
{
    void ClearCache();

    Task<List<CompetitionName>> GetAllCompetitionsAsync();
    Task<CompetitionData?> CreateCompetitionAsync(CompetitionName competitionName);
    Task<CompetitionData?> GetCompetitionDataAsync(CompetitionName competitionName);
    Task<CompetitionData?> UpdateCompetitionAsync(CompetitionName competitionName, Func<CompetitionData, CompetitionData> funcUpdate);

    Task<LeagueData?> CreateLeagueAsync(LeagueId leagueId);
    Task<LeagueData?> GetLeagueDataAsync(LeagueId leagueId);
    Task<LeagueData?> UpdateLeagueAsync(LeagueId leagueId, Func<LeagueData, LeagueData> funcUpdate);

    Task<KnockoutData?> CreateKnockoutAsync(KnockoutId knockoutId);
    Task<KnockoutData?> GetKnockoutDataAsync(KnockoutId knockoutId);
    Task<KnockoutData?> UpdateKnockoutAsync(KnockoutId knockoutId, Func<KnockoutData, KnockoutData> funcUpdate);
}

public class PpService : IPpService
{
    private IFileSystem _fs;
    private IPpMatchService _matchService;

    private IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

    public PpService(
        PingpongOption option,
        IFileSystemService fsService,
        IPpMatchService matchService)
    {
        _fs = fsService.GetFileSystem(option.FileSystemSelect, option.Path);
        _matchService = matchService;
    }

    public void ClearCache()
    {
        var cache = _cache;
        _cache = new MemoryCache(new MemoryCacheOptions());
        cache.Dispose();
    }

    #region Competition
    public async Task<List<CompetitionName>> GetAllCompetitionsAsync()
    {
        Func<Paths, string> filepath = path => path[PingpongPathType.CompetitionListFilePath];

        if (await _fs.FileExistsAsync(filepath))
        {
            return await _fs.ReadJsonAsync<List<CompetitionName>>(filepath);
        }
        else
        {
            return new();
        }
    }

    private string GetCompetitionFilePath(Paths path, CompetitionName competitionName)
    {
        return $"{path[PingpongPathType.CompetitionPath]}/{competitionName}.json";
    }

    public async Task<CompetitionData?> CreateCompetitionAsync(CompetitionName competitionName)
    {
        if (competitionName.HasInvalidFileNameChar())
        {
            return null;
        }
        if (await _fs.FileExistsAsync(path => GetCompetitionFilePath(path, competitionName)))
        {
            return null;
        }

        var competitionData = new CompetitionData
        {
            Name = competitionName,
        };

        await _fs.WriteJsonAsync(path => GetCompetitionFilePath(path, competitionName), competitionData);
        _cache.Set(competitionName, competitionData);

        var competitions = await GetAllCompetitionsAsync();
        competitions.Add(competitionName);
        await _fs.WriteJsonAsync(path => path[PingpongPathType.CompetitionListFilePath], competitions);

        return competitionData;
    }

    public async Task<CompetitionData?> GetCompetitionDataAsync(CompetitionName competitionName)
    {
        CompetitionData? competitionData = null;
        if (_cache.TryGetValue(competitionName, out CompetitionData cached))
        {
            competitionData = cached;
        }
        else if (await _fs.FileExistsAsync(path => GetCompetitionFilePath(path, competitionName)))
        {
            competitionData = await _fs.ReadJsonAsync<CompetitionData>(path => GetCompetitionFilePath(path, competitionName));
        }

        if (competitionData == null)
            return null;

        if (competitionData.LeagueIdList?.Any() ?? false)
        {
            var leagues = await competitionData.LeagueIdList
                .Select(async leagueId => await GetLeagueDataAsync(leagueId))
                .WhenAll();
            competitionData.LeagueList = leagues
                .Where(league => league != null)
                .Select(league => league!)
                .ToList();
        }
        if (competitionData.KnockoutIdList?.Any() ?? false)
        {
            var knockouts = await competitionData.KnockoutIdList
                .Select(async knockoutId => await GetKnockoutDataAsync(knockoutId))
                .WhenAll();
            competitionData.KnockoutList = knockouts
                .Where(knockout => knockout != null)
                .Select(knockout => knockout!)
                .ToList();
        }

        _cache.Set(competitionName, competitionData);
        return competitionData;
    }

    public async Task<CompetitionData?> UpdateCompetitionAsync(CompetitionName competitionName, Func<CompetitionData, CompetitionData> funcUpdate)
    {
        var competitionData = await GetCompetitionDataAsync(competitionName);

        if (competitionData == null)
            return null;

        var updated = funcUpdate(competitionData);

        await _fs.WriteJsonAsync(path => GetCompetitionFilePath(path, competitionName), updated);
        _cache.Set(competitionName, updated);
        return updated;
    }
    #endregion

    #region League
    private string GetLeagueFilePath(Paths path, LeagueId leagueId)
    {
        return $"{path[PingpongPathType.CompetitionPath]}/{leagueId.CompetitionName}/league/{leagueId}.json";
    }

    public async Task<LeagueData?> CreateLeagueAsync(LeagueId leagueId)
    {
        if (leagueId.HasInvalidFileNameChar() || string.IsNullOrWhiteSpace(leagueId.Id))
            return null;

        var competitionData = await GetCompetitionDataAsync(leagueId.CompetitionName);

        if (competitionData == null)
            return null;

        var leagueData = new LeagueData(leagueId);

        await _fs.WriteJsonAsync(path => GetLeagueFilePath(path, leagueId), leagueData);
        _cache.Set(leagueId, leagueData);

        return leagueData;
    }

    public async Task<LeagueData?> GetLeagueDataAsync(LeagueId leagueId)
    {
        LeagueData? leagueData = null;
        if (_cache.TryGetValue(leagueId, out LeagueData cached))
        {
            leagueData = cached;
        }
        else if (await _fs.FileExistsAsync(path => GetLeagueFilePath(path, leagueId)))
        {
            leagueData = await _fs.ReadJsonAsync<LeagueData>(path => GetLeagueFilePath(path, leagueId));
        }

        if (leagueData == null)
            return null;

        if (leagueData.MatchIdList?.Any() ?? false)
        {
            var matches = await leagueData.MatchIdList
                .Select(async matchId => await _matchService.GetMatchDataAsync<MatchData>(matchId))
                .WhenAll();

            leagueData.MatchList = matches.ToList();
        }

        _cache.Set(leagueId, leagueData);
        return leagueData;
    }

    public async Task<LeagueData?> UpdateLeagueAsync(LeagueId leagueId, Func<LeagueData, LeagueData> funcUpdate)
    {
        var leagueData = await GetLeagueDataAsync(leagueId);

        if (leagueData == null)
            return null;

        var updated = funcUpdate(leagueData);

        await _fs.WriteJsonAsync(path => GetLeagueFilePath(path, leagueId), updated);
        _cache.Set(leagueId, leagueData);
        return updated;
    }
    #endregion

    #region Knockout
    private string GetKnockoutFilePath(Paths path, KnockoutId knockoutId)
    {
        return $"{path[PingpongPathType.CompetitionPath]}/{knockoutId.CompetitionName}/knockout/{knockoutId}.json";
    }

    public async Task<KnockoutData?> CreateKnockoutAsync(KnockoutId knockoutId)
    {
        if (knockoutId.HasInvalidFileNameChar() || string.IsNullOrWhiteSpace(knockoutId.Id))
            return null;

        var competitionData = await GetCompetitionDataAsync(knockoutId.CompetitionName);

        if (competitionData == null)
            return null;

        var knockoutData = new KnockoutData(knockoutId);

        await _fs.WriteJsonAsync(path => GetKnockoutFilePath(path, knockoutId), knockoutData);
        _cache.Set(knockoutId, knockoutData);

        return knockoutData;
    }

    public async Task<KnockoutData?> GetKnockoutDataAsync(KnockoutId knockoutId)
    {
        KnockoutData? knockoutData = null;
        if (_cache.TryGetValue(knockoutId, out KnockoutData cached))
        {
            knockoutData = cached;
        }
        else if (await _fs.FileExistsAsync(path => GetKnockoutFilePath(path, knockoutId)))
        {
            knockoutData = await _fs.ReadJsonAsync<KnockoutData>(path => GetKnockoutFilePath(path, knockoutId));
        }

        if (knockoutData == null)
            return null;

        if (knockoutData.MatchIdList?.Any() ?? false)
        {
            var matches = await knockoutData.MatchIdList
                .Select(async matchId => await _matchService.GetMatchDataAsync<KnockoutMatchData>(matchId))
                .WhenAll();

            knockoutData.MatchList = matches.ToList();
        }

        _cache.Set(knockoutId, knockoutData);
        return knockoutData;
    }

    public async Task<KnockoutData?> UpdateKnockoutAsync(KnockoutId knockoutId, Func<KnockoutData, KnockoutData> funcUpdate)
    {
        var knockoutData = await GetKnockoutDataAsync(knockoutId);

        if (knockoutData == null)
            return null;

        var updated = funcUpdate(knockoutData);

        await _fs.WriteJsonAsync(path => GetKnockoutFilePath(path, knockoutId), updated);
        _cache.Set(knockoutId, updated);
        return updated;
    }
    #endregion
}
