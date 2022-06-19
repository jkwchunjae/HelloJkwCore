namespace ProjectPingpong;

public interface IPpService
{
    Task<List<CompetitionName>> GetAllCompetitionsAsync();
    Task<CompetitionData?> CreateCompetitionAsync(CompetitionName competitionName);
    Task<CompetitionData?> GetCompetitionDataAsync(CompetitionName competitionName);
    Task<CompetitionData?> UpdateCompetitionAsync(CompetitionName competitionName, Func<CompetitionData, CompetitionData> funcUpdate);

    Task<(CompetitionData? CompetitionData, LeagueData? LeagueData)> CreateLeagueAsync(LeagueId leagueId);
    Task<LeagueData?> GetLeagueDataAsync(LeagueId leagueId);
    Task<LeagueData?> UpdateLeagueAsync(LeagueId leagueId, Func<LeagueData, LeagueData> funcUpdate);

    Task<(CompetitionData? CompetitionData, KnockoutData? KnockoutData)> CreateKnockoutAsync(KnockoutId knockoutId);
    Task<KnockoutData?> GetKnockoutDataAsync(KnockoutId knockoutId);
    Task<KnockoutData?> UpdateKnockoutAsync(KnockoutId knockoutId, Func<KnockoutData, KnockoutData> funcUpdate);
}

public class PpService : IPpService
{
    private IFileSystem _fs;
    private IPpMatchService _matchService;

    public PpService(
        PingpongOption option,
        IFileSystemService fsService,
        IPpMatchService matchService)
    {
        _fs = fsService.GetFileSystem(option.FileSystemSelect, option.Path);
        _matchService = matchService;
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

        var success = await _fs.WriteJsonAsync(path => GetCompetitionFilePath(path, competitionName), competitionData);

        var competitions = await GetAllCompetitionsAsync();
        competitions.Add(competitionName);
        await _fs.WriteJsonAsync(path => path[PingpongPathType.CompetitionListFilePath], competitions);

        if (success)
        {
            return competitionData;
        }
        else
        {
            return null;
        }
    }

    public async Task<CompetitionData?> GetCompetitionDataAsync(CompetitionName competitionName)
    {
        if (await _fs.FileExistsAsync(path => GetCompetitionFilePath(path, competitionName)))
        {
            var competitionData = await _fs.ReadJsonAsync<CompetitionData>(path => GetCompetitionFilePath(path, competitionName));

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

            return competitionData;
        }
        else
        {
            return null;
        }
    }

    public async Task<CompetitionData?> UpdateCompetitionAsync(CompetitionName competitionName, Func<CompetitionData, CompetitionData> funcUpdate)
    {
        var competitionData = await GetCompetitionDataAsync(competitionName);

        if (competitionData == null)
            return null;

        var updated = funcUpdate(competitionData);

        await _fs.WriteJsonAsync(path => GetCompetitionFilePath(path, competitionName), updated);
        return updated;
    }
    #endregion

    #region League
    private string GetLeagueFilePath(Paths path, LeagueId leagueId)
    {
        return $"{path[PingpongPathType.CompetitionPath]}/{leagueId.CompetitionName}/league/{leagueId}.json";
    }

    public async Task<(CompetitionData? CompetitionData, LeagueData? LeagueData)> CreateLeagueAsync(LeagueId leagueId)
    {
        if (leagueId.HasInvalidFileNameChar() || string.IsNullOrWhiteSpace(leagueId.Id))
            return (null, null);

        var competitionData = await GetCompetitionDataAsync(leagueId.CompetitionName);

        if (competitionData == null)
            return (null, null);

        var leagueData = new LeagueData
        {
            Id = leagueId,
        };

        competitionData = await CompetitionData.AddLeague(this, competitionData.Name, leagueData);

        if (competitionData == null)
            return (null, null);

        await _fs.WriteJsonAsync(path => GetLeagueFilePath(path, leagueId), leagueData);

        return (competitionData, leagueData);
    }

    public async Task<LeagueData?> GetLeagueDataAsync(LeagueId leagueId)
    {
        if (await _fs.FileExistsAsync(path => GetLeagueFilePath(path, leagueId)))
        {
            var leagueData = await _fs.ReadJsonAsync<LeagueData>(path => GetLeagueFilePath(path, leagueId));

            if (leagueData.MatchIdList?.Any() ?? false)
            {
                var matches = await leagueData.MatchIdList
                    .Select(async matchId => await _matchService.GetMatchDataAsync<MatchData>(matchId))
                    .WhenAll();

                leagueData.MatchList = matches.ToList();
            }
            return leagueData;
        }
        else
        {
            return null;
        }
    }

    public async Task<LeagueData?> UpdateLeagueAsync(LeagueId leagueId, Func<LeagueData, LeagueData> funcUpdate)
    {
        var leagueData = await GetLeagueDataAsync(leagueId);

        if (leagueData == null)
            return null;

        var updated = funcUpdate(leagueData);

        await _fs.WriteJsonAsync(path => GetLeagueFilePath(path, leagueId), updated);
        return updated;
    }
    #endregion

    #region Knockout
    private string GetKnockoutFilePath(Paths path, KnockoutId knockoutId)
    {
        return $"{path[PingpongPathType.CompetitionPath]}/{knockoutId.CompetitionName}/knockout/{knockoutId}.json";
    }

    public async Task<(CompetitionData? CompetitionData, KnockoutData? KnockoutData)> CreateKnockoutAsync(KnockoutId knockoutId)
    {
        if (knockoutId.HasInvalidFileNameChar() || string.IsNullOrWhiteSpace(knockoutId.Id))
            return (null, null);

        var competitionData = await GetCompetitionDataAsync(knockoutId.CompetitionName);

        if (competitionData == null)
            return (null, null);

        var knockoutData = new KnockoutData
        {
            Id = knockoutId,
        };

        competitionData = await CompetitionData.AddKnockout(this, competitionData.Name, knockoutData);

        if (competitionData == null)
            return (null, null);

        await _fs.WriteJsonAsync(path => GetKnockoutFilePath(path, knockoutId), knockoutData);

        return (competitionData, knockoutData);
    }

    public async Task<KnockoutData?> GetKnockoutDataAsync(KnockoutId knockoutId)
    {
        if (await _fs.FileExistsAsync(path => GetKnockoutFilePath(path, knockoutId)))
        {
            var knockoutData = await _fs.ReadJsonAsync<KnockoutData>(path => GetKnockoutFilePath(path, knockoutId));

            if (knockoutData.MatchIdList?.Any() ?? false)
            {
                var matches = await knockoutData.MatchIdList
                    .Select(async matchId => await _matchService.GetMatchDataAsync<KnockoutMatchData>(matchId))
                    .WhenAll();

                knockoutData.MatchList = matches.ToList();
            }
            return knockoutData;
        }
        else
        {
            return null;
        }
    }

    public async Task<KnockoutData?> UpdateKnockoutAsync(KnockoutId knockoutId, Func<KnockoutData, KnockoutData> funcUpdate)
    {
        var knockoutData = await GetKnockoutDataAsync(knockoutId);

        if (knockoutData == null)
            return null;

        var updated = funcUpdate(knockoutData);

        await _fs.WriteJsonAsync(path => GetKnockoutFilePath(path, knockoutId), updated);
        return updated;
    }
    #endregion
}
