using Common;
using JkwExtensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HelloJkwService.Diary
{
    public class DiaryService
    {
        private readonly DiaryOption _option;
        private readonly Encoding _encoding = new UTF8Encoding(false);

        private readonly ICache<List<DiaryData>> _diaryCache;

        public DiaryService(DiaryOption option)
        {
            _option = option;

            _diaryCache = new MemoryCache<List<DiaryData>>();
        }

        private async Task<DiaryInfo> GetDiaryInfoByAsync(Func<DiaryInfo, bool> func, CancellationToken ct)
        {
            var diaryInfoList = JsonConvert.DeserializeObject<List<DiaryInfo>>
                (await File.ReadAllTextAsync(_option.DiaryListPath, _encoding, ct));

            return diaryInfoList.FirstOrDefault(func);
        }

        public async Task<DiaryInfo> GetDiaryInfoByUserIdAsync(string userId, CancellationToken ct)
            => await GetDiaryInfoByAsync(x => x.Id == userId, ct);

        public async Task<DiaryInfo> GetDiaryInfoByDiaryNameAsync(string diaryName, CancellationToken ct)
            => await GetDiaryInfoByAsync(x => x.DiaryName == diaryName, ct);

        public async Task<List<DiaryData>> GetDiaryDataListAsync(DiaryInfo diaryInfo, CancellationToken ct)
            => await GetDiaryDataListAsync(diaryInfo.DiaryName, ct);

        public async Task<List<DiaryData>> GetDiaryDataListAsync(string diaryName, CancellationToken ct)
        {
            if (_diaryCache.TryGet(diaryName, out var diaries))
            {
                return diaries;
            }

            var diaryDirectoryPath = Path.Combine(_option.RootPath, diaryName);
            var fileList = Directory.GetFiles(diaryDirectoryPath, "*.diary");

            var diaryTextList = await Task.WhenAll(fileList
#if DEBUG
                .TakeLast(30)
#endif
                .Select(path => File.ReadAllTextAsync(path, _encoding, ct)));

            var diaryList = diaryTextList
                .Select(x => JsonConvert.DeserializeObject<DiaryData>(x))
                .OrderBy(x => x.Date)
                .ThenBy(x => x.Index)
                .ToList();

            var diaryInfo = await GetDiaryInfoByDiaryNameAsync(diaryName, ct);

            diaryList.ForEach(x => x.SetDiaryInfo(this, diaryInfo));

            _diaryCache.Set(diaryName, diaryList);

            return diaryList;
        }

        public List<DiaryData> GetDiaryDataListFromCache(string diaryName)
        {
            if (_diaryCache.TryGet(diaryName, out var diaries))
            {
                return diaries;
            }
            return new List<DiaryData>();
        }
    }
}
