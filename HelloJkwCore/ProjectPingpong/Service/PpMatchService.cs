namespace ProjectPingpong;

public interface IPpMatchService
{
    Task<T?> CreateMatchAsync<T>(MatchId matchId) where T : MatchData, new();
    Task<T> GetMatchDataAsync<T>(MatchId matchId) where T : MatchData;
    Task<T> UpdateMatchDataAsync<T>(MatchId matchId, Func<T, T> funcUpdate) where T : MatchData;
    Task DeleteMatchDataAsync(MatchId matchId);
}

public class PpMatchService : IPpMatchService
{
    private IFileSystem _fs;

    public PpMatchService(
        PingpongOption option,
        IFileSystemService fsService)
    {
        _fs = fsService.GetFileSystem(option.FileSystemSelect, option.Path);
    }

    private string GetMatchFilePath(Paths path, MatchId matchId)
    {
        if (matchId.Type == MatchId.Types.LeagueMatch || matchId.Type == MatchId.Types.KnockoutMatch)
        {
            return $"{path[PingpongPathType.CompetitionPath]}/{matchId.CompetitionName}/match/{matchId}.json";
        }
        else // if free
        {
            return $"{path[PingpongPathType.FreeMatchPath]}/{matchId}.json";
        }
    }

    public async Task<T?> CreateMatchAsync<T>(MatchId matchId) where T : MatchData, new()
    {
        if (matchId.HasInvalidFileNameChar() || string.IsNullOrEmpty(matchId.Id))
        {
            return null;
        }

        var matchData = new T
        {
            Id = matchId,
        };

        var success = await _fs.WriteJsonAsync(path => GetMatchFilePath(path, matchId), matchData);

        if (success)
        {
            return matchData;
        }
        else
        {
            return default;
        }
    }

    public async Task<T> GetMatchDataAsync<T>(MatchId matchId) where T : MatchData
    {
        var matchData = await _fs.ReadJsonAsync<T>(path => GetMatchFilePath(path, matchId));
        return matchData;
    }

    public async Task<T> UpdateMatchDataAsync<T>(MatchId matchId, Func<T, T> funcUpdate) where T : MatchData
    {
        var matchData = await _fs.ReadJsonAsync<T>(path => GetMatchFilePath(path, matchId));
        var updated = funcUpdate(matchData);
        return updated;
    }

    public async Task DeleteMatchDataAsync(MatchId matchId)
    {
        await _fs.DeleteFileAsync(path => GetMatchFilePath(path, matchId));
    }
}
