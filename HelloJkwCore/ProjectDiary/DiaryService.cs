using Common;
using Common.Extensions;
using Common.FileSystem;
using Common.User;
using JkwExtensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectDiary
{
    public class DiaryService : IDiaryService
    {
        private readonly DiaryOption _option;
        private readonly FileSystemService _fsService;
        private readonly IFileSystem _fs;
        private readonly IFileSystem _backup;
        private readonly ConcurrentDictionary<string /* DiaryName */, List<DiaryFileName>> _filesCache = new();

        public DiaryService(DiaryOption diaryOption, FileSystemService fsService)
        {
            _option = diaryOption;
            _fsService = fsService;

            _fs = _fsService.GetFileSystem(_option.MainFileSystem);
            if (_option.UseBackup)
            {
                _backup = _fsService.GetFileSystem(_option.BackupFileSystem);
            }
        }

        private async Task<List<DiaryInfo>> GetDiaryInfosAsync()
        {
            var path = DiaryPath.DiaryList();
            if (await _fs.FileExistsAsync(path))
            {
                return await _fs.ReadJsonAsync<List<DiaryInfo>>(path);
            }
            else
            {
                return new List<DiaryInfo>();
            }
        }

        private async Task<bool> SaveDiaryInfosAsync(List<DiaryInfo> diaryInfos)
        {
            await _fs.WriteJsonAsync(DiaryPath.DiaryList(), diaryInfos);
            return true;
        }

        private async Task<List<DiaryFileName>> GetDiaryListAsync(string diaryName)
        {
            if (_filesCache.TryGetValue(diaryName, out var list))
            {
                return list;
            }

            var diaryPath = DiaryPath.Diary(diaryName);

            if (!(await _fs.DirExistsAsync(diaryPath)))
            {
                await _fs.CreateDirectoryAsync(diaryPath);
            }

            var files = await _fs.GetFilesAsync(diaryPath, ".diary");

            var diaryFileNameList = files
                .OrderBy(x => x)
                .Where(x => x.Left(8).TryToDate(out var _))
                .Select(x => new DiaryFileName(x))
                .ToList();

            _filesCache.AddOrUpdate(diaryName, diaryFileNameList, (key, old) => diaryFileNameList);

            return diaryFileNameList;
        }

        private void SaveDiaryCache(string diaryName, List<DiaryFileName> list)
        {
            _filesCache.AddOrUpdate(diaryName, list, (key, old) => list);
        }

        private async Task<DiaryContent> GetDiaryContentAsync(string diaryContentPath)
        {
            return await _fs.ReadJsonAsync<DiaryContent>(diaryContentPath);
        }

        public async Task<DiaryInfo> CreateDiaryInfoAsync(AppUser user, string diaryName, bool isSecret)
        {
            var diaryList = await GetDiaryInfosAsync();

            if (diaryList.Any(x => x.DiaryName == diaryName))
                throw new DuplicatedDiaryNameException();

            var newDiary = new DiaryInfo(user.Id, user.Email, diaryName, isSecret);
            diaryList.Add(newDiary);
            await SaveDiaryInfosAsync(diaryList);

            return newDiary;
        }

        public async Task<DiaryInfo> GetUserDiaryInfoAsync(AppUser user)
        {
            var diaryList = await GetDiaryInfosAsync();

            return diaryList.FirstOrDefault(x => x.Owner == user?.Email);
        }

        public async Task<DiaryInfo> GetDiaryInfoAsync(AppUser user, string diaryName)
        {
            var diaryList = await GetViewableDiaryInfoAsync(user);

            return diaryList.FirstOrDefault(x => x.DiaryName == diaryName);
        }

        public async Task<List<DiaryInfo>> GetWritableDiaryInfoAsync(AppUser user)
        {
            var diaryList = await GetDiaryInfosAsync();

            return diaryList.Where(x =>
                {
                    if (x.Owner == user.Email)
                        return true;
                    if (x.Writers.Contains(user.Email))
                        return true;
                    return false;
                })
                .ToList();
        }

        public async Task<List<DiaryInfo>> GetViewableDiaryInfoAsync(AppUser user)
        {
            var diaryList = await GetDiaryInfosAsync();

            return diaryList.Where(x =>
                {
                    if (x.Owner == user.Email)
                        return true;
                    if (x.Writers.Contains(user.Email))
                        return true;
                    if (x.Viewers.Contains(user.Email))
                        return true;
                    return false;
                })
                .ToList();
        }

        public async Task<DiaryView> GetLastDiaryViewAsync(AppUser user, DiaryInfo diary)
        {
            var list = await GetDiaryListAsync(diary.DiaryName);

            if (list.Empty())
            {
                return default;
            }

            DateTime lastDate = list.Last().Date;

            return await GetDiaryViewAsync(user, diary, lastDate);
        }

        public async Task<DiaryView> GetDiaryViewAsync(AppUser user, DiaryInfo diary, DateTime date)
        {
            var list = await GetDiaryListAsync(diary.DiaryName);

            var dates = list
                .Select(x => x.Date)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            var before = dates.Where(x => x.Date < date.Date).ToList();
            var after = dates.Where(x => x.Date > date.Date).ToList();
            DateTime? prevDate = before.Any() ? before.Last() : null;
            DateTime? nextDate = after.Any() ? after.First() : null;

            var todayContents = await Task.WhenAll(list
                .Where(x => x.Date == date.Date)
                .Select(x => DiaryPath.Content(diary.DiaryName, x.FileName))
                .Select(async path => await GetDiaryContentAsync(path)));

            return new DiaryView
            {
                DiaryInfo = diary,
                DiaryContents = todayContents.ToList(),
                DiaryNavigationData = new DiaryNavigationData
                {
                    Today = date.Date,
                    PrevDate = prevDate,
                    NextDate = nextDate,
                },
            };
        }

        public async Task<DiaryContent> WriteDiaryAsync(AppUser user, DiaryInfo diary, DateTime date, string text)
        {
            return await WriteDiaryAsync(user, diary, date, text, false);
        }

        public async Task<DiaryContent> WriteDiaryAsync(AppUser user, DiaryInfo diary, DateTime date, string text, string password)
        {
            var cipherText = text.Encrypt(password);
            return await WriteDiaryAsync(user, diary, date, cipherText, true);
        }

        private async Task<DiaryContent> WriteDiaryAsync(AppUser user, DiaryInfo diary, DateTime date, string text, bool isSecret)
        {
            var list = await GetDiaryListAsync(diary.DiaryName);

            var content = new DiaryContent
            {
                Date = date,
                Text = text,
                IsSecret = isSecret,
                RegDate = DateTime.Now,
                LastModifyDate = DateTime.Now,
                Index = MakeNewIndex(list, date),
            };

            var diaryPath = DiaryPath.Content(diary.DiaryName, content.GetFileName());
            await _fs.WriteJsonAsync(diaryPath, content);

            list.Add(new DiaryFileName(content.GetFileName()));

            list = list.OrderBy(x => x).ToList();

            SaveDiaryCache(diary.DiaryName, list);

            return content;

        }

        private int MakeNewIndex(List<DiaryFileName> fileList, DateTime date)
        {
            var today = fileList.Where(x => x.Date == date).ToList();
            if (today.Any())
            {
                return today.Max(x => x.Index) + 1;
            }
            else
            {
                return 1;
            }
        }

        public async Task<List<DiaryContent>> UpdateDiaryAsync(AppUser user, DiaryInfo diary, List<DiaryContent> contents)
        {
            var list = await GetDiaryListAsync(diary.DiaryName);

            var deleteFiles = contents.Where(x => string.IsNullOrWhiteSpace(x.Text)).ToList();
            var updateFiles = contents.Where(x => !string.IsNullOrWhiteSpace(x.Text)).ToList();

            foreach (var deleteFile in deleteFiles)
            {
                var path = DiaryPath.Content(diary.DiaryName, deleteFile.GetFileName());
                await _fs.DeleteFileAsync(path);
                list.Remove(new DiaryFileName(deleteFile.GetFileName()));
            }

            foreach (var updateFile in updateFiles)
            {
                var path = DiaryPath.Content(diary.DiaryName, updateFile.GetFileName());
                await _fs.WriteJsonAsync(path, updateFile);
            }

            SaveDiaryCache(diary.DiaryName, list);

            return updateFiles;
        }

        public async Task<List<DiaryContent>> UpdateDiaryAsync(AppUser user, DiaryInfo diary, List<DiaryContent> contents, string password)
        {
            foreach (var content in contents.Where(x => x.IsSecret))
            {
                content.Text = content.Text.Encrypt(password);
            }

            return await UpdateDiaryAsync(user, diary, contents);
        }
    }
}
