namespace ProjectDiary;

public class DiarySearchService : IDiarySearchService
{
    private readonly IFileSystem _fs;

    private Dictionary<string, DiarySearchEngine> _engineDic = new();

    private ReaderWriterLockSlim _lock = new();

    public DiarySearchService(
        IFileSystemService fsService,
        DiaryOption diaryOption)
    {
        _fs = fsService.GetFileSystem(diaryOption.SearchEngineFileSystem, diaryOption.Path);
    }

    private async Task<DiarySearchEngine> GetSearchEngineAsync(DiaryName diaryName, int year)
    {
        var engineKey = $"{diaryName}.{year}";
        var hasEngine = false;
        var engineCount = 0;
        lock (_engineDic)
        {
            hasEngine = _engineDic.ContainsKey(engineKey);
            engineCount = _engineDic.Count(x => x.Key.StartsWith($"{diaryName}."));
        }

        DiarySearchEngine engine = null;
        if (!hasEngine)
        {
            if (engineCount > 100)
            {
                throw new TooManyDiaryEngineException();
            }
            engine = new DiarySearchEngine();

            Func<Paths, string> triePath = path => path.DiaryTrie(diaryName, year);
            if (await _fs.FileExistsAsync(triePath))
            {
                var trie = await _fs.ReadJsonAsync<DiaryTrie>(triePath);
                engine.SetTrie(trie);
            }
        }

        lock (_engineDic)
        {
            if (!_engineDic.ContainsKey(engineKey))
            {
                _engineDic[engineKey] = engine;
            }

            return _engineDic[engineKey];
        }
    }

    public void RefreshCache(DiaryName diaryName)
    {
        lock (_engineDic)
        {
            var removeKey = _engineDic.Keys
                .Where(key => key.StartsWith($"{diaryName}."))
                .ToArray();

            foreach (var key in removeKey)
            {
                _engineDic.Remove(key);
            }
        }
    }

    public void RefreshCacheAll()
    {
        lock (_engineDic)
        {
            _engineDic.Clear();
        }
    }

    public async Task AppendDiaryTextAsync(DiaryName diaryName, DiaryFileName fileName, string diaryText)
    {
        var year = fileName.Date.Year;
        var engine = await GetSearchEngineAsync(diaryName, year);
        engine.AddText(diaryText, fileName.FileName);
    }

    public async Task<IEnumerable<DiaryFileName>> SearchAsync(DiaryName diaryName, DiarySearchData searchData)
    {
        var minYear = searchData.BeginDate.Value.Year;
        var maxYear = searchData.EndDate.Value.Year;
        var lists = await Enumerable.Range(minYear, maxYear - minYear + 1)
            .Select(async year =>
            {
                var engine = await GetSearchEngineAsync(diaryName, year);
                var result = engine.Search(searchData.Keyword);
                var list = result?.Select(x => new DiaryFileName(x))
                    .Where(x => x.Date >= searchData.BeginDate)
                    .Where(x => x.Date <= searchData.EndDate)
                    .ToList();
                return list;
            })
            .WhenAll();

        var list = lists
            .Where(x => x != null)
            .SelectMany(x => x).ToList();

        return list;
    }

    public Task ClearTrie(DiaryName diaryName)
    {
        lock (_engineDic)
        {
            var clearKey = _engineDic.Keys
                .Where(key => key.StartsWith($"{diaryName}."))
                .ToArray();

            foreach (var key in clearKey)
            {
                var engine = _engineDic[key];
                engine.SetTrie(new DiaryTrie());
            }
        }

        return Task.CompletedTask;
    }

    public async Task<bool> SaveDiaryTrie(DiaryName diaryName, int year)
    {
        var engine = await GetSearchEngineAsync(diaryName, year);
        var jsonText = engine.GetTrieJson();
        return await _fs.WriteTextAsync(path => path.DiaryTrie(diaryName, year), jsonText);
    }
}