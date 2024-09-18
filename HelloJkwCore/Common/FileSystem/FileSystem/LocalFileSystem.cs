namespace Common;

public class LocalFileSystem : IFileSystem
{
    protected readonly Paths _pathOf;
    private readonly Encoding _encoding;

    public FileSystemType FileSystemType => FileSystemType.Local;

    public LocalFileSystem(PathMap pathOption, Encoding? encoding = null)
    {
        if (pathOption.Local == null)
        {
            throw new ArgumentNullException(nameof(pathOption.Local));
        }
        _pathOf = new Paths(pathOption, FileSystemType.Local!);
        _encoding = encoding ?? new UTF8Encoding(false);
    }

    public Task<bool> CreateDirectoryAsync(Func<Paths, string> pathFunc, CancellationToken ct = default)
    {
        var path = pathFunc(GetPathOf());
        Directory.CreateDirectory(path);
        return Task.FromResult(true);
    }

    public Task<bool> DeleteFileAsync(Func<Paths, string> pathFunc, CancellationToken ct = default)
    {
        var path = pathFunc(GetPathOf());
        File.Delete(path);
        return Task.FromResult(true);
    }

    public Task<bool> DirExistsAsync(Func<Paths, string> pathFunc, CancellationToken ct = default)
    {
        var path = pathFunc(GetPathOf());
        var exists = Directory.Exists(path);
        return Task.FromResult(exists);
    }

    public Task<bool> FileExistsAsync(Func<Paths, string> pathFunc, CancellationToken ct = default)
    {
        var path = pathFunc(GetPathOf());
        return Task.FromResult(File.Exists(path));
    }

    public Task<List<string>> GetFilesAsync(Func<Paths, string> pathFunc, string? extension = null, CancellationToken ct = default)
    {
        var path = pathFunc(GetPathOf());

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        var list = Directory.GetFiles(path)
            .Select(x => Path.GetFileName(x))
            .Where(x => extension == null || x.EndsWith(extension))
            .ToList();
        return Task.FromResult(list);
    }

    public Paths GetPathOf()
    {
        if (_pathOf == null)
        {
            throw new NotDefinedFileSystemType(FileSystemType.Local);
        }
        return _pathOf;
    }

    public async Task<T> ReadJsonAsync<T>(Func<Paths, string> pathFunc, CancellationToken ct = default)
    {
        var path = pathFunc(GetPathOf());
        if (File.Exists(path))
        {
            var text = await File.ReadAllTextAsync(path, _encoding);
            return Json.Deserialize<T>(text);
        }
        else
        {
            throw new FileNotFoundException(path);
        }
    }

    public async Task<string> ReadTextAsync(Func<Paths, string> pathFunc, CancellationToken ct = default)
    {
        var path = pathFunc(GetPathOf());
        if (!File.Exists(path))
            return string.Empty;

        var text = await File.ReadAllTextAsync(path, _encoding);
        return text;
    }

    public async Task<bool> WriteJsonAsync<T>(Func<Paths, string> pathFunc, T obj, CancellationToken ct = default)
    {
        var path = pathFunc(GetPathOf());
        var dir = Directory.GetParent(path);
        if (!Directory.Exists(dir!.FullName))
        {
            Directory.CreateDirectory(dir.FullName);
        }
        await File.WriteAllTextAsync(path, Json.Serialize(obj), _encoding, ct);
        return true;
    }

    public async Task<bool> WriteTextAsync(Func<Paths, string> pathFunc, string text, CancellationToken ct = default)
    {
        var path = pathFunc(GetPathOf());
        var dir = Directory.GetParent(path);
        if (!Directory.Exists(dir!.FullName))
        {
            Directory.CreateDirectory(dir.FullName);
        }
        await File.WriteAllTextAsync(path, text, _encoding, ct);
        return true;
    }

    public async Task<bool> WriteBlobAsync(Func<Paths, string> pathFunc, Stream stream, CancellationToken ct = default)
    {
        var path = pathFunc(GetPathOf());
        var dir = Directory.GetParent(path);
        if (!Directory.Exists(dir!.FullName))
        {
            Directory.CreateDirectory(dir.FullName);
        }
        using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
        {
            await stream.CopyToAsync(fileStream, 4096, ct);
        }
        return true;
    }

    public async Task<byte[]> ReadBlobAsync(Func<Paths, string> pathFunc, CancellationToken ct = default)
    {
        var path = pathFunc(GetPathOf());
        if (!File.Exists(path))
            return [];

        var bytes = await File.ReadAllBytesAsync(path);
        return bytes;
    }
}