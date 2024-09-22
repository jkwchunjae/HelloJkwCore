using Microsoft.Extensions.DependencyInjection;

namespace Common;

public class InMemoryFileSystemBuilder : IFileSystemBuilder
{
    public FileSystemType FileSystemType => FileSystemType.InMemory;

    private readonly ISerializer _serializer;

    public InMemoryFileSystemBuilder(ISerializer serializer)
    {
        _serializer = serializer;
    }

    public IFileSystem Build(PathMap pathMap)
    {
        var paths = new Paths(pathMap, FileSystemType.InMemory);
        return new InMemoryFileSystem(paths, _serializer);
    }
}

public static class InMemoryFileSystemExtensions
{
    public static IServiceCollection AddInMemoryFileSystem(this IServiceCollection services)
    {
        services.AddSingleton<IFileSystemBuilder, InMemoryFileSystemBuilder>(serviceProvider =>
        {
            var serializer = serviceProvider.GetRequiredService<ISerializer>();
            return new InMemoryFileSystemBuilder(serializer);
        });
        return services;
    }
}

public class InMemoryFileSystem : IFileSystem
{
    public FileSystemType FileSystemType => FileSystemType.InMemory;
    protected readonly Paths _paths;
    private readonly ISerializer _serializer;
    private readonly Dictionary<string, string> _files = new();


    public InMemoryFileSystem(Paths paths, ISerializer serializer)
    {
        _paths = paths;
        _serializer = serializer;
    }

    public Task<bool> CreateDirectoryAsync(Func<Paths, string> pathFunc, CancellationToken ct = default)
    {
        return Task.FromResult(true);
    }

    public Task<bool> DeleteFileAsync(Func<Paths, string> pathFunc, CancellationToken ct = default)
    {
        var path = pathFunc(_paths);
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
        var path = pathFunc(_paths);
        return Task.FromResult(_files.ContainsKey(path));
    }

    public Task<List<string>> GetFilesAsync(Func<Paths, string> pathFunc, string? extension = null, CancellationToken ct = default)
    {
        var path = pathFunc(_paths);
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

    public Task<T> ReadJsonAsync<T>(Func<Paths, string> pathFunc, CancellationToken ct = default)
    {
        var path = pathFunc(_paths);

        if (_files.TryGetValue(path, out var text))
        {
            return Task.FromResult(_serializer.Deserialize<T>(text));
        }
        else
        {
            throw new FileNotFoundException();
        }
    }

    public Task<string> ReadTextAsync(Func<Paths, string> pathFunc, CancellationToken ct = default)
    {
        var path = pathFunc(_paths);

        if (!_files.ContainsKey(path))
            return Task.FromResult(string.Empty);

        var text = _files[path];
        return Task.FromResult(text);
    }

    public Task<bool> WriteJsonAsync<T>(Func<Paths, string> pathFunc, T obj, CancellationToken ct = default)
    {
        var path = pathFunc(_paths);
        _files[path] = _serializer.Serialize(obj);
        return Task.FromResult(true);
    }

    public Task<bool> WriteTextAsync(Func<Paths, string> pathFunc, string text, CancellationToken ct = default)
    {
        var path = pathFunc(_paths);
        _files[path] = text;
        return Task.FromResult(true);
    }

    public async Task<bool> WriteBlobAsync(Func<Paths, string> pathFunc, Stream stream, CancellationToken ct = default)
    {
        var path = pathFunc(_paths);
        using (var reader = new StreamReader(stream, Encoding.ASCII))
        {
            var text = await reader.ReadToEndAsync();
            _files[path] = text;
        }
        return true;
    }

    public Task<byte[]> ReadBlobAsync(Func<Paths, string> pathFunc, CancellationToken ct = default)
    {
        var path = pathFunc(_paths);

        if (!_files.ContainsKey(path))
            return Task.FromResult<byte[]>([]);

        var text = _files[path];
        var bytes = Encoding.ASCII.GetBytes(text);
        return Task.FromResult(bytes);
    }
}