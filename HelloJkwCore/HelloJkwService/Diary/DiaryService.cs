using Common;
using Common.Core;
using JkwExtensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        private async Task<DiaryInfo> GetDiaryInfoByAsync(Func<DiaryInfo, bool> func, CancellationToken ct = default)
        {
            var diaryInfoList = JsonConvert.DeserializeObject<List<DiaryInfo>>
                (await File.ReadAllTextAsync(_option.DiaryListPath, _encoding, ct));

            var diaryInfo = diaryInfoList.FirstOrDefault(func);
            if (diaryInfo != null)
            {
                diaryInfo.Writers.Add(diaryInfo.Owner);
                diaryInfo.Viewers.Add(diaryInfo.Owner);
            }
            return diaryInfo;
        }

        public async Task<DiaryInfo> GetDiaryInfoByUserIdAsync(string userId, CancellationToken ct = default)
            => await GetDiaryInfoByAsync(x => x.Id == userId, ct);

        public async Task<DiaryInfo> GetDiaryInfoByDiaryNameAsync(string diaryName, CancellationToken ct = default)
            => await GetDiaryInfoByAsync(x => x.DiaryName == diaryName, ct);

        public async Task<List<DiaryData>> GetDiaryDataListAsync(DiaryInfo diaryInfo, CancellationToken ct = default)
            => await GetDiaryDataListAsync(diaryInfo.DiaryName, ct);

        public async Task<List<DiaryData>> GetDiaryDataListAsync(string diaryName, CancellationToken ct = default)
        {
            if (_diaryCache.TryGet(diaryName, out var diaries))
            {
                return diaries;
            }

            var diaryDirectoryPath = Path.Combine(_option.RootPath, diaryName);
            if (!Directory.Exists(diaryDirectoryPath))
                Directory.CreateDirectory(diaryDirectoryPath);

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

        public async Task<Result> WriteDiaryAsync(string diaryName, DateTime date, string content)
        {
            var diaryDirectoryPath = Path.Combine(_option.RootPath, diaryName);
            var searchPattern = $"{date:yyyyMMdd}_*.diary";
            var exists = Directory.GetFiles(diaryDirectoryPath, searchPattern);

            var index = 1;
            if (exists.Any())
            {
                var regex = new Regex(@"\d+_(\d+).diary");
                var lastIndex = exists.Select(x => regex.Match(x))
                    .Where(x => x.Success)
                    .Select(x => x.Groups[1].Value.ToInt())
                    .Max();
                index = lastIndex + 1;
            }

            var diaryData = new DiaryData
            {
                Date = date,
                CreateDate = DateTime.Now,
                LastModifyDate = DateTime.Now,
                Index = index,
                IsSecure = false,
                Text = content,
            };

            var diaryInfo = await GetDiaryInfoByDiaryNameAsync(diaryName);
            diaryData.SetDiaryInfo(this, diaryInfo);

            var diaryPath = Path.Combine(diaryDirectoryPath, $"{date:yyyyMMdd}_{index}.diary");
            await File.WriteAllTextAsync(diaryPath, JsonConvert.SerializeObject(diaryData), _encoding);

            if (_diaryCache.TryGet(diaryName, out var diaries))
            {
                diaries.Add(diaryData);
            }
            else
            {
                _diaryCache.Set(diaryName, new List<DiaryData> { diaryData });
            }


            return Result.Success();
        }

        public async Task<Result> UpdateDiaryAsync(string diaryName, List<DiaryData> diaryList)
        {
            var diaryDirectoryPath = Path.Combine(_option.RootPath, diaryName);

            var cachedList = GetDiaryDataListFromCache(diaryName);

            foreach (var diaryData in diaryList)
            {
                var date = diaryData.Date;
                var index = diaryData.Index;
                var diaryPath = Path.Combine(diaryDirectoryPath, $"{date:yyyyMMdd}_{index}.diary");
                var findDiary = cachedList.Find(x => x.Date == date && x.Index == index);
                if (findDiary != null)
                {
                    cachedList.Remove(findDiary);
                }

                if (string.IsNullOrWhiteSpace(diaryData.Text))
                {
                    File.Delete(diaryPath);
                }
                else
                {
                    diaryData.LastModifyDate = DateTime.Now;
                    cachedList.Add(diaryData);
                    await File.WriteAllTextAsync(diaryPath, JsonConvert.SerializeObject(diaryData), _encoding);
                }
            }

            _diaryCache.Set(diaryName, cachedList);

            return Result.Success();
        }
    }
}
