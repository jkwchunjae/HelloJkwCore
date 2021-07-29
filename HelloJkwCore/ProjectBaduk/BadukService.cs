using Common;
using JkwExtensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBaduk
{
    public class BadukService : IBadukService
    {
        IFileSystem _fs;
        public BadukService(
            BadukOption option,
            IFileSystemService fsService)
        {
            _fs = fsService.GetFileSystem(option.FileSystemSelect);
        }

        public Task DeleteBadukGameData(DiaryName diaryName, string subject)
        {
            return _fs.DeleteFileAsync(path => GameDataFilePath(path, diaryName, subject));
        }

        public async Task<BadukGameData> GetBadukGameData(DiaryName diaryName, string subject)
        {
            var gameData = await _fs.ReadJsonAsync<BadukGameData>(path => GameDataFilePath(path, diaryName, subject));

            return gameData;
        }

        public async Task<List<BadukGameData>> GetBadukSummaryList(DiaryName diaryName)
        {
            var list = await _fs.GetFilesAsync(path => GameDataSavePath(path, diaryName));

            var gameDataList = await list
                .Select(fileName => Path.GetFileNameWithoutExtension(fileName))
                .Select(async file => await _fs.ReadJsonAsync<BadukGameData>(path => GameDataFilePath(path, diaryName, file)))
                .WhenAll();

            return gameDataList.OrderByDescending(x => x.CreateTime).ToList();
        }

        public async Task SaveBadukGameData(DiaryName diaryName, BadukGameData badukGameData)
        {
            await _fs.WriteJsonAsync(path => GameDataFilePath(path, diaryName, badukGameData.Subject), badukGameData);
        }

        public async Task<List<BadukDiary>> GetBadukDiaryList(AppUser user)
        {
            var list = await _fs.GetFilesAsync(path => path.GetPath(PathType.BadukDiaryPath));

            var diaryList = await list
                .Select(fileName => Path.GetFileNameWithoutExtension(fileName))
                .Select(async file => await _fs.ReadJsonAsync<BadukDiary>(path => DiaryFilePath(path, file)))
                .WhenAll();

            return diaryList.OrderBy(x => x.Name).ToList();
        }

        public async Task CreateBadukDiary(AppUser user, DiaryName diaryName)
        {
            var diaryData = new BadukDiary
            {
                Name = diaryName,
                OwnerUserId = user.Id,
                ConnectUserIdList = new() { user.Id },
            };
            await _fs.WriteJsonAsync(path => DiaryFilePath(path, diaryName.Name), diaryData);
        }

        private string GameDataSavePath(PathOf path, DiaryName diaryName)
        {
            return path.GetPath(PathType.BadukSavePath) + "/" + diaryName;
        }

        private string GameDataFilePath(PathOf path, DiaryName diaryName, string fileName)
        {
            var badukSavePath = path.GetPath(PathType.BadukSavePath);
            return $"{badukSavePath}/{diaryName}/{fileName}.json";
        }

        private string DiaryFilePath(PathOf path, string diaryFileName)
        {
            return $"${path.GetPath(PathType.BadukDiaryPath)}/{diaryFileName}";
        }
    }
}
