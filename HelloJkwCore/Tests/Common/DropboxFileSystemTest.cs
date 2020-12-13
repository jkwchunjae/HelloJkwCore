using Common;
using Common.FileSystem;
using Dropbox.Api;
using HelloJkwCore.Authentication;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Common
{
#if DEBUG
    public class DropboxFileSystemTest
    {
        DropboxFileSystem _fs;

        private bool IsLocal => Environment.OSVersion.Platform == PlatformID.Win32NT;

        public DropboxFileSystemTest()
        {
            if (!IsLocal)
                return;

            var authOptionPath = @"C:\Users\jkwch\Dropbox\helloJkw\config\oauth-option.json";
            var authOptions = Json.Deserialize<List<OAuthOption>>(File.ReadAllText(authOptionPath));
            var dropboxOption = authOptions.First(x => x.Provider == AuthProvider.Dropbox);
            var client = new DropboxClient(dropboxOption.RefreshToken, dropboxOption.ClientId, dropboxOption.ClientSecret);
            _fs = new DropboxFileSystem(client);
        }

        [Fact]
        public async Task FileReadWriteTest()
        {
            if (!IsLocal)
                return;

            var path = "/test/test.json";
            var data = "test";

            await _fs.WriteJsonAsync(path, data);

            var result = await _fs.ReadJsonAsync<string>(path);

            Assert.Equal(data, result);
        }

        [Fact]
        public async Task FileExistsTest()
        {
            if (!IsLocal)
                return;

            var path = "/test/test.json";
            var data = "test";

            await _fs.WriteJsonAsync(path, data);

            var exists = await _fs.FileExistsAsync(path);

            Assert.True(exists);
        }

        [Fact]
        public async Task FileNotExistsTest()
        {
            if (!IsLocal)
                return;

            var path = "/test/test_unknown.json";

            var exists = await _fs.FileExistsAsync(path);

            Assert.False(exists);
        }

        [Fact]
        public async Task FileDeleteTest()
        {
            if (!IsLocal)
                return;

            var path = "/test/test.json";
            var data = "test";

            await _fs.WriteJsonAsync(path, data);

            await _fs.DeleteFileAsync(path);

            var exists = await _fs.FileExistsAsync(path);

            Assert.False(exists);
        }

        [Fact]
        public async Task GetFileListTest()
        {
            if (!IsLocal)
                return;

            var dirPath = "/test";
            var path1 = "/test/test1.json";
            var path2 = "/test/test2.json";
            var data = "test";

            await _fs.WriteJsonAsync(path1, data);
            await _fs.WriteJsonAsync(path2, data);

            var list = await _fs.GetFilesAsync(dirPath);

            Assert.Contains(Path.GetFileName(path1), list);
            Assert.Contains(Path.GetFileName(path2), list);
        }
    }
#endif
}
