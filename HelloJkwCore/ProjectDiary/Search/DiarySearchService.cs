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
            using (_lock.AcquireWriterLock())
            {
                if (!_engineDic.ContainsKey(diaryName))
                {
                    var engine = new DiarySearchEngine();

                    Func<PathOf, string> triePath = path => path.DiaryTrie(diaryName);
                    if (await _fs.FileExistsAsync(triePath))
                    {
                        var trie = await _fs.ReadJsonAsync<DiaryTrie>(triePath);
                        engine.SetTrie(trie);
                    }

                    _engineDic[diaryName] = engine;
                }

                return _engineDic[diaryName];
            }
        }

        public void RefreshCache(string diaryName)
        {
            using (_lock.AcquireWriterLock())
            {
                if (_engineDic.ContainsKey(diaryName))
                {
                    _engineDic.Remove(diaryName);
                }
            }
        }

        public void RefreshCacheAll()
        {
            using (_lock.AcquireWriterLock())
            {
                _engineDic.Clear();
            }
        }

        public async Task AppendDiaryTextAsync(string diaryName, DiaryFileName fileName, string diaryText)
        {
            var engine = await GetSearchEngineAsync(diaryName);
            engine.AddText(diaryText, fileName.FileName);

            var trieJsonText = engine.GetTrieJson();

            await _fs.WriteTextAsync(path => path.DiaryTrie(diaryName), trieJsonText);
        }

        public async Task<IEnumerable<DiaryFileName>> SearchAsync(string diaryName, DiarySearchData searchData)
        {
            var engine = await GetSearchEngineAsync(diaryName);
            var result = engine.Search(searchData.Keyword);
            var list = result?.Select(x => new DiaryFileName(x)).ToList();

            return list;
        }
    }
}
