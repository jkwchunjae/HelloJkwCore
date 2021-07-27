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

        public Task DeleteBadukGameData(AppUser user, string subject)
        {
            return _fs.DeleteFileAsync(path => SaveFilePath(path, user, subject));
        }

        public async Task<BadukGameData> GetBadukGameData(AppUser user, string subject)
        {
            var gameData = await _fs.ReadJsonAsync<BadukGameData>(path => SaveFilePath(path, user, subject));

            return gameData;
        }

        public async Task<List<BadukGameData>> GetBadukSummaryList(AppUser user)
        {
            var list = await _fs.GetFilesAsync(path => UserSavePath(path, user));

            var gameDataList = await list
                .Select(fileName => Path.GetFileNameWithoutExtension(fileName))
                .Select(async file => await _fs.ReadJsonAsync<BadukGameData>(path => SaveFilePath(path, user, file)))
                .WhenAll();

            return gameDataList.OrderByDescending(x => x.CreateTime).ToList();
        }

        public async Task SaveBadukGameData(AppUser user, BadukGameData badukGameData)
        {
            await _fs.WriteJsonAsync(path => SaveFilePath(path, user, badukGameData.Subject), badukGameData);
        }

        private string UserSavePath(PathOf path, AppUser user)
        {
            return path.GetPath(PathType.BadukSavePath) + "/" + user.Id;
        }

        private string SaveFilePath(PathOf path, AppUser user, string fileName)
        {
            var badukSavePath = path.GetPath(PathType.BadukSavePath);
            return $"{badukSavePath}/{user.Id}/{fileName}.json";
        }
    }
}
