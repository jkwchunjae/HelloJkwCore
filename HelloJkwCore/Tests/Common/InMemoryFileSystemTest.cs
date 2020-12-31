using Common.FileSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Common
{
    //public class InMemoryFileSystemTest
    //{
    //    InMemoryFileSystem _fs;

    //    public InMemoryFileSystemTest()
    //    {
    //        _fs = new InMemoryFileSystem();
    //    }

    //    [Fact]
    //    public async Task FileReadWriteTest()
    //    {
    //        var path = "/test/test.json";
    //        var data = "test";

    //        await _fs.WriteJsonAsync(path, data);

    //        var result = await _fs.ReadJsonAsync<string>(path);

    //        Assert.Equal(data, result);
    //    }

    //    [Fact]
    //    public async Task FileExistsTest()
    //    {
    //        var path = "/test/test.json";
    //        var data = "test";

    //        await _fs.WriteJsonAsync(path, data);

    //        var exists = await _fs.FileExistsAsync(path);

    //        Assert.True(exists);
    //    }

    //    [Fact]
    //    public async Task FileNotExistsTest()
    //    {
    //        var path = "/test/test_unknown.json";

    //        var exists = await _fs.FileExistsAsync(path);

    //        Assert.False(exists);
    //    }

    //    [Fact]
    //    public async Task FileDeleteTest()
    //    {
    //        var path = "/test/test.json";
    //        var data = "test";

    //        await _fs.WriteJsonAsync(path, data);

    //        await _fs.DeleteFileAsync(path);

    //        var exists = await _fs.FileExistsAsync(path);

    //        Assert.False(exists);
    //    }

    //    [Fact]
    //    public async Task GetFileListTest()
    //    {
    //        var dirPath = "/test";
    //        var path1 = "/test/test1.json";
    //        var path2 = "/test/test2.json";
    //        var data = "test";

    //        await _fs.WriteJsonAsync(path1, data);
    //        await _fs.WriteJsonAsync(path2, data);

    //        var list = await _fs.GetFilesAsync(dirPath);

    //        Assert.Contains(Path.GetFileName(path1), list);
    //        Assert.Contains(Path.GetFileName(path2), list);
    //    }
    //}
}
