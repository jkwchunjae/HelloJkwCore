using Microsoft.Extensions.DependencyInjection;

namespace Common;

public class LocalFileSystemBuilder : IFileSystemBuilder
{
    public FileSystemType FileSystemType => FileSystemType.Local;
    private readonly ISerializer _serializer;
    private readonly Encoding _encoding;

    public LocalFileSystemBuilder(ISerializer serializer, Encoding? encoding = null)
    {
        _serializer = serializer;
        _encoding = encoding ?? new UTF8Encoding(false);
    }

    public IFileSystem Build(PathMap pathMap)
    {
        var paths = new Paths(pathMap, FileSystemType.Local);
        return new LocalFileSystem(paths, _serializer, _encoding);
    }
}

public static class LocalFileSystemExtensions
{
    public static IServiceCollection AddLocalFileSystem(this IServiceCollection services)
    {
        services.AddSingleton<IFileSystemBuilder, LocalFileSystemBuilder>(serviceProvider =>
        {
            var serializer = serviceProvider.GetRequiredService<ISerializer>();
            return new LocalFileSystemBuilder(serializer);
        });
        return services;
    }
}

public class LocalFileSystem : IFileSystem
{
    protected readonly Paths _paths;
    private readonly ISerializer _serializer;
    private readonly Encoding _encoding;

    public FileSystemType FileSystemType => FileSystemType.Local;

    public LocalFileSystem(Paths paths,
        ISerializer serializer,
        Encoding? encoding = null)
    {
        _paths = paths;
        _serializer = serializer;
        _encoding = encoding ?? new UTF8Encoding(false);
    }

    public Task<bool> CreateDirectoryAsync(Func<Paths, string> pathFunc, CancellationToken ct = default)
    {
        var path = pathFunc(_paths);
        Directory.CreateDirectory(path);
        return Task.FromResult(true);
    }

    public Task<bool> DeleteFileAsync(Func<Paths, string> pathFunc, CancellationToken ct = default)
    {
        var path = pathFunc(_paths);
        File.Delete(path);
        return Task.FromResult(true);
    }

    public Task<bool> DirExistsAsync(Func<Paths, string> pathFunc, CancellationToken ct = default)
    {
        var path = pathFunc(_paths);
        var exists = Directory.Exists(path);
        return Task.FromResult(exists);
    }

    public Task<bool> FileExistsAsync(Func<Paths, string> pathFunc, CancellationToken ct = default)
    {
        var path = pathFunc(_paths);
        return Task.FromResult(File.Exists(path));
    }

    public Task<List<string>> GetFilesAsync(Func<Paths, string> pathFunc, string? extension = null, CancellationToken ct = default)
    {
        var path = pathFunc(_paths);

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

    public async Task<T> ReadJsonAsync<T>(Func<Paths, string> pathFunc, CancellationToken ct = default)
    {
        var path = pathFunc(_paths);
        if (File.Exists(path))
        {
            var text = await File.ReadAllTextAsync(path, _encoding);
            return _serializer.Deserialize<T>(text);
        }
        else
        {
            throw new FileNotFoundException(path);
        }
    }

    public async Task<string> ReadTextAsync(Func<Paths, string> pathFunc, CancellationToken ct = default)
    {
        var path = pathFunc(_paths);
        if (!File.Exists(path))
            return string.Empty;

        var text = await File.ReadAllTextAsync(path, _encoding);
        return text;
    }

    public async Task<bool> WriteJsonAsync<T>(Func<Paths, string> pathFunc, T obj, CancellationToken ct = default)
    {
        var path = pathFunc(_paths);
        var dir = Directory.GetParent(path);
        if (!Directory.Exists(dir!.FullName))
        {
            Directory.CreateDirectory(dir.FullName);
        }
        await File.WriteAllTextAsync(path, _serializer.Serialize(obj), _encoding, ct);
        return true;
    }

    public async Task<bool> WriteTextAsync(Func<Paths, string> pathFunc, string text, CancellationToken ct = default)
    {
        var path = pathFunc(_paths);
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
        var path = pathFunc(_paths);
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
        var path = pathFunc(_paths);
        if (!File.Exists(path))
            return [];

        var bytes = await File.ReadAllBytesAsync(path);
        return bytes;
    }
}