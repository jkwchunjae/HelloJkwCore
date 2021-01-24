using Common;
using JkwExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProjectDiary
{
    public class DiaryService : IDiaryService
    {
        private readonly IFileSystem _fs;
        private readonly IDiarySearchService _diarySearchService;
        private readonly Dictionary<string /* DiaryName */, List<DiaryFileName>> _filesCache = new();

        public DiaryService(
            DiaryOption option,
            IDiarySearchService diarySearchService,
            IFileSystemService fsService)
        {
            _diarySearchService = diarySearchService;
            _fs = fsService.GetFileSystem(option.FileSystemSelect);
        }

        private async Task<List<DiaryFileName>> GetDiaryListAsync(string diaryName)
        {
            lock (_filesCache)
            {
                if (_filesCache.TryGetValue(diaryName, out var list))
                {
                    return list;
                }
            }

            Func<PathOf, string> diaryPath = path => path.Diary(diaryName);

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

            lock (_filesCache)
            {
                _filesCache[diaryName] = diaryFileNameList;
            }

            return diaryFileNameList;
        }

        private void SaveDiaryCache(string diaryName, List<DiaryFileName> list)
        {
            lock (_filesCache)
            {
                _filesCache[diaryName] = list;
            }
        }

        /// <summary>
        /// Atomic operation
        /// </summary>
        private async Task<bool> CheckAndAddDiaryNameAsync(string diaryName)
        {
            if (diaryName.Length < 3)
                return false;
            if (diaryName.Length > 30)
                return false;
            if (!Regex.IsMatch(diaryName, @"^[a-z]+$"))
                return false;

            // TODO lock
            var diaryNameList = await _fs.ReadJsonAsync<List<string>>(path => path.DiaryNameListFile());
            if (diaryNameList == default(List<string>))
            {
                diaryNameList = new List<string>();
            }

            if (diaryNameList.Contains(diaryName))
            {
                return false;
            }

            diaryNameList.Add(diaryName);
            await _fs.WriteJsonAsync(path => path.DiaryNameListFile(), diaryNameList);

            return true;
        }

        private async Task<DiaryContent> GetDiaryContentAsync(string diaryName, DiaryFileName fileName)
        {
            return await _fs.ReadJsonAsync<DiaryContent>(path => path.Content(diaryName, fileName.ToString()));
        }

        public async Task<DiaryContent> GetDiaryContentAsync(AppUser user, DiaryInfo diary, DiaryFileName diaryFileName)
        {
            var force = false;
            if (user.HasRole(UserRole.Admin))
                force = true;

            if (!force && !diary.CanRead(user?.Email))
                return null;

            return await GetDiaryContentAsync(diary.DiaryName, diaryFileName);
        }

        private async Task<UserDiaryInfo> CreateUserDiaryInfoAsync(AppUser user)
        {
            Func<PathOf, string> path = pathof => pathof.UserDiaryInfo(user);
            if (await _fs.FileExistsAsync(path))
            {
                return await _fs.ReadJsonAsync<UserDiaryInfo>(path);
            }

            var userDiaryInfo = new UserDiaryInfo
            {
                UserId = user.Id,
            };

            var success = await _fs.WriteJsonAsync(path, userDiaryInfo);

            if (success)
            {
                return userDiaryInfo;
            }
            return null;
        }

        public async Task<DiaryInfo> CreateDiaryInfoAsync(AppUser user, string diaryName, bool isSecret)
        {
            // 1. 일기장 이름 등록 
            if (!await CheckAndAddDiaryNameAsync(diaryName))
            {
                return null;
            }

            // 일단 만들자.
            var userDiaryInfo = await CreateUserDiaryInfoAsync(user);
            if (userDiaryInfo == null)
            {
                // 만들지 못했거나 읽기 실패
                return null;
            }

            userDiaryInfo.AddMyDiary(diaryName);
            await _fs.WriteJsonAsync(path => path.UserDiaryInfo(user), userDiaryInfo);

            var newDiary = new DiaryInfo(user.Id, user.Email, diaryName, isSecret);
            await _fs.WriteJsonAsync(path => path.DiaryInfo(diaryName), newDiary);

            return newDiary;
        }

        public async Task<UserDiaryInfo> GetUserDiaryInfoAsync(AppUser user)
        {
            if (await _fs.FileExistsAsync(path => path.UserDiaryInfo(user)))
            {
                return await _fs.ReadJsonAsync<UserDiaryInfo>(path => path.UserDiaryInfo(user));
            }

            return null;
        }

        public async Task<DiaryInfo> GetDiaryInfoAsync(AppUser user, string diaryName)
        {
            var userDiaryInfo = await GetUserDiaryInfoAsync(user);

            if (userDiaryInfo?.IsViewable(diaryName) ?? false)
            {
                if (await _fs.FileExistsAsync(path => path.DiaryInfo(diaryName)))
                {
                    return await _fs.ReadJsonAsync<DiaryInfo>(path => path.DiaryInfo(diaryName));
                }
            }

            return null;
        }

        public async Task<List<DiaryInfo>> GetWritableDiaryInfoAsync(AppUser user)
        {
            var userDiaryInfo = await GetUserDiaryInfoAsync(user);

            var writableList = userDiaryInfo.MyDiaryList
                .Concat(userDiaryInfo.WriterList);

            var writableDiaryList = await writableList
                .Select(async diaryName => await GetDiaryInfoAsync(user, diaryName))
                .WhenAll();

            return writableDiaryList.ToList();
        }

        public async Task<List<DiaryInfo>> GetViewableDiaryInfoAsync(AppUser user)
        {
            var userDiaryInfo = await GetUserDiaryInfoAsync(user);

            var viewableList = userDiaryInfo.MyDiaryList
                .Concat(userDiaryInfo.WriterList)
                .Concat(userDiaryInfo.ViewList);

            var viewableDiaryList = await viewableList
                .Select(async diaryName => await GetDiaryInfoAsync(user, diaryName))
                .WhenAll();

            return viewableDiaryList.ToList();
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

            var todayContents = await list
                .Where(x => x.Date == date.Date)
                .Select(async fileName => await GetDiaryContentAsync(user, diary, fileName))
                .WhenAll();

            return new DiaryView
            {
                DiaryInfo = diary,
                DiaryContents = todayContents.Where(x => x != null).ToList(),
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

            Func<PathOf, string> diaryPath = path => path.Content(diary.DiaryName, content.GetFileName());
            await _fs.WriteJsonAsync(diaryPath, content);

            var diaryFileName = new DiaryFileName(content.GetFileName());
            list.Add(diaryFileName);

            list = list.OrderBy(x => x).ToList();

            SaveDiaryCache(diary.DiaryName, list);

            if (!diary.IsSecret)
            {
                if (_diarySearchService != null)
                    await _diarySearchService.AppendDiaryTextAsync(diary.DiaryName, diaryFileName, text);
            }

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
                Func<PathOf, string> deleteFilePath = path => path.Content(diary.DiaryName, deleteFile.GetFileName());
                await _fs.DeleteFileAsync(deleteFilePath);
                list.Remove(new DiaryFileName(deleteFile.GetFileName()));
            }

            foreach (var updateFile in updateFiles)
            {
                Func<PathOf, string> updateFilePath = path => path.Content(diary.DiaryName, updateFile.GetFileName());
                await _fs.WriteJsonAsync(updateFilePath, updateFile);
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

        public async Task<List<DiaryFileName>> GetDiaryFileAllAsync(AppUser user, DiaryInfo diary)
        {
            var list = await GetDiaryListAsync(diary.DiaryName);

            return list;
        }

        public async Task<List<DiaryInfo>> GetAllDiaryListAsync(AppUser user)
        {
            if (!(user?.HasRole(UserRole.Admin) ?? false))
                return null;

            var diaryNameList = await _fs.ReadJsonAsync<List<string>>(path => path.DiaryNameListFile());
            var diaryInfoList = await (diaryNameList ?? new List<string>())
                .Select(async diaryName => await _fs.ReadJsonAsync<DiaryInfo>(path => path.DiaryInfo(diaryName)))
                .WhenAll();

            return diaryInfoList.ToList();
        }
    }
}
