namespace ProjectPingpong;

public interface IPpMatchService
{
    Task<T?> CreateMatchAsync<T>(MatchId matchId) where T : MatchData, new();
    Task<T?> CreateMatchAsync<T>(MatchId matchId, T matchData) where T : MatchData, new();
    Task<T> GetMatchDataAsync<T>(MatchId matchId) where T : MatchData;
    Task<T> UpdateMatchDataAsync<T>(MatchId matchId, Func<T, T> funcUpdate) where T : MatchData;
    Task DeleteMatchDataAsync(MatchId matchId);
    PpNotifier<MatchId, MatchData> Watch(MatchId matchId);
    void Unwatch(MatchId matchId);
}

public class PpMatchService : IPpMatchService
{
    private IFileSystem _fs;
    private Dictionary<MatchId, PpNotifier<MatchId, MatchData>> _updators = new();

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

        if (await _fs.FileExistsAsync(path => GetMatchFilePath(path, matchId)))
        {
            return await GetMatchDataAsync<T>(matchId);
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

    public async Task<T?> CreateMatchAsync<T>(MatchId matchId, T matchData) where T : MatchData, new()
    {
        if (matchId.HasInvalidFileNameChar() || string.IsNullOrEmpty(matchId.Id))
        {
            return null;
        }

        if (await _fs.FileExistsAsync(path => GetMatchFilePath(path, matchId)))
        {
            return await GetMatchDataAsync<T>(matchId);
        }

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
        await _fs.WriteJsonAsync(path => GetMatchFilePath(path, matchId), updated);
        MatchUpdated(updated);
        return updated;
    }

    public async Task DeleteMatchDataAsync(MatchId matchId)
    {
        await _fs.DeleteFileAsync(path => GetMatchFilePath(path, matchId));
    }

    private void MatchUpdated(MatchData matchData)
    {
        if (_updators.TryGetValue(matchData.Id, out var matchUpdator))
        {
            matchUpdator.MatchUpdated(matchData);
        }
    }

    public PpNotifier<MatchId, MatchData> Watch(MatchId matchId)
    {
        lock (_updators)
        {
            if (_updators.TryGetValue(matchId, out var matchUpdator))
            {
                matchUpdator.WatchCount++;
                return matchUpdator;
            }
            else
            {
                var newUpdator = new PpNotifier<MatchId, MatchData>(matchId);
                _updators.Add(matchId, newUpdator);
                return newUpdator;
            }
        }
    }

    public void Unwatch(MatchId matchId)
    {
        lock (_updators)
        {
            if (_updators.TryGetValue(matchId, out var matchUpdator))
            {
                matchUpdator.WatchCount--;
                if (matchUpdator.WatchCount <= 0)
                {
                    _updators.Remove(matchId);
                }
            }
        }
    }
}
