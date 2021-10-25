using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JkwExtensions;

namespace ProjectDiary
{
    public class DiarySearchService : IDiarySearchService
    {
        private readonly IFileSystem _fs;

        private Dictionary<string, DiarySearchEngine> _engineDic = new();

        private ReaderWriterLockSlim _lock = new();

        public DiarySearchService(
            IFileSystemService fsService,
            DiaryOption diaryOption)
        {
            _fs = fsService.GetFileSystem(diaryOption.SearchEngineFileSystem);
        }

        private async Task<DiarySearchEngine> GetSearchEngineAsync(string diaryName)
        {
            var hasEngine = false;
            lock (_engineDic)
            {
                hasEngine = _engineDic.ContainsKey(diaryName);
            }

            DiarySearchEngine engine = null;
            if (!hasEngine)
            {
                engine = new DiarySearchEngine();

                Func<PathOf, string> triePath = path => path.DiaryTrie(diaryName);
                if (await _fs.FileExistsAsync(triePath))
                {
                    var trie = await _fs.ReadJsonAsync<DiaryTrie>(triePath);
                    engine.SetTrie(trie);
                }
            }

            lock (_engineDic)
            {
                if (!_engineDic.ContainsKey(diaryName))
                {
                    _engineDic[diaryName] = engine;
                }

                return _engineDic[diaryName];
            }
        }

        public void RefreshCache(string diaryName)
        {
            lock (_engineDic)
            {
                if (_engineDic.ContainsKey(diaryName))
                {
                    _engineDic.Remove(diaryName);
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

        public async Task AppendDiaryTextAsync(string diaryName, DiaryFileName fileName, string diaryText)
        {
            var engine = await GetSearchEngineAsync(diaryName);
            engine.AddText(diaryText, fileName.FileName);
        }

        public async Task<IEnumerable<DiaryFileName>> SearchAsync(string diaryName, DiarySearchData searchData)
        {
            var engine = await GetSearchEngineAsync(diaryName);
            var result = engine.Search(searchData.Keyword);
            var list = result?.Select(x => new DiaryFileName(x))
                .Where(x => x.Date >= searchData.BeginDate)
                .Where(x => x.Date <= searchData.EndDate)
                .ToList();

            return list;
        }

        public async Task ClearTrie(string diaryName)
        {
            var engine = await GetSearchEngineAsync(diaryName);
            engine.SetTrie(new DiaryTrie());
        }

        public async Task<bool> SaveDiaryTrie(string diaryName)
        {
            var engine = await GetSearchEngineAsync(diaryName);
            var jsonText = engine.GetTrieJson();
            return await _fs.WriteTextAsync(path => path.DiaryTrie(diaryName), jsonText);
        }
    }
}
