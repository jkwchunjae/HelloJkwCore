namespace Common;

public class InMemoryFileSystem : IFileSystem
{
    protected readonly Paths _pathOf;
    private readonly Dictionary<string, string> _files = new();

    public InMemoryFileSystem(PathMap pathOption)
    {
        if (pathOption.InMemory != null)
        {
            _pathOf = new Paths(pathOption, FileSystemType.InMemory);
        }
    }

    public Task<bool> CreateDirectoryAsync(Func<Paths, string> pathFunc, CancellationToken ct = default)
    {
        return Task.FromResult(true);
    }

    public Task<bool> DeleteFileAsync(Func<Paths, string> pathFunc, CancellationToken ct = default)
    {
        var path = pathFunc(GetPathOf());
        if (_files.ContainsKey(path))
            _files.Remove(path);

        return Task.FromResult(true);
    }

    public Task<bool> DirExistsAsync(Func<Paths, string> pathFunc, CancellationToken ct = default)
    {
        return Task.FromResult(true);
    }

    public Task<bool> FileExistsAsync(Func<Paths, string> pathFunc, CancellationToken ct = default)
    {
        var path = pathFunc(GetPathOf());
        return Task.FromResult(_files.ContainsKey(path));
    }

    public Task<List<string>> GetFilesAsync(Func<Paths, string> pathFunc, string extension = null, CancellationToken ct = default)
    {
        var path = pathFunc(GetPathOf());
        if (!path.EndsWith("/"))
            path += "/";

        var list = _files.Keys
            .Where(x => x.StartsWith(path))
            .Select(x => x.Replace(path, ""))
            // path를 지웠는데 '/'가 있으면 file이 아니다.
            .Where(x => !x.Contains("/"))
            .Where(x => extension == null || x.EndsWith(extension))
            .ToList();

        return Task.FromResult(list);
    }

    public Paths GetPathOf()
    {
        if (_pathOf == null)
        {
            throw new NotDefinedFileSystemType(FileSystemType.InMemory);
        }
        return _pathOf;
    }

    public Task<T> ReadJsonAsync<T>(Func<Paths, string> pathFunc, CancellationToken ct = default)
    {
        var path = pathFunc(GetPathOf());

        if (!_files.ContainsKey(path))
            return Task.FromResult(default(T));

        var text = _files[path];
        return Task.FromResult(Json.Deserialize<T>(text));
    }

    public Task<string> ReadTextAsync(Func<Paths, string> pathFunc, CancellationToken ct = default)
    {
        var path = pathFunc(GetPathOf());

        if (!_files.ContainsKey(path))
            return Task.FromResult(string.Empty);

        var text = _files[path];
        return Task.FromResult(text);
    }

    public Task<bool> WriteJsonAsync<T>(Func<Paths, string> pathFunc, T obj, CancellationToken ct = default)
    {
        var path = pathFunc(GetPathOf());
        _files[path] = Json.Serialize(obj);
        return Task.FromResult(true);
    }

    public Task<bool> WriteTextAsync(Func<Paths, string> pathFunc, string text, CancellationToken ct = default)
    {
        var path = pathFunc(GetPathOf());
        _files[path] = text;
        return Task.FromResult(true);
    }

    public async Task<bool> WriteBlobAsync(Func<Paths, string> pathFunc, Stream stream, CancellationToken ct = default)
    {
        var path = pathFunc(GetPathOf());
        using (var reader = new StreamReader(stream, Encoding.ASCII))
        {
            var text = await reader.ReadToEndAsync();
            _files[path] = text;
        }
        return true;
    }

    public Task<byte[]> ReadBlobAsync(Func<Paths, string> pathFunc, CancellationToken ct = default)
    {
        var path = pathFunc(GetPathOf());

        if (!_files.ContainsKey(path))
            return Task.FromResult<byte[]>([]);

        var text = _files[path];
        var bytes = Encoding.ASCII.GetBytes(text);
        return Task.FromResult(bytes);
    }
}