using System.Runtime.Caching;

namespace ProjectWorldCup.FifaLibrary;

public partial class Fifa : IFifa
{
    private readonly IFileSystem _fs;
    private static HttpClient _httpClient = new();
    private MemoryCache _cache = new MemoryCache("FIFA");
    private readonly ISerializer _serializer;

    public Fifa(
        IFileSystemService fsService,
        ISerializer serializer,
        WorldCupOption option)
    {
        _serializer = serializer;
        _fs = fsService.GetFileSystem(option.FileSystemSelect, option.Path);
    }

    protected async Task<T> GetFromCacheOrAsync<T>(string cacheKey, Func<Task<T>> func)
    {
        Func<Paths, string> cacheFilePath = paths => $"{paths["Cache"]}/{cacheKey}.json";
        try
        {
            if (_cache.Contains(cacheKey))
            {
                return (T)_cache.Get(cacheKey);
            }

            var result = await func();

            //await _fs.WriteJsonAsync(cacheFilePath, result); // 계속 남아서 주석 걸어둠
            _cache.Set(new CacheItem(cacheKey, result), new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(5),
            });

            return result;
        }
        catch
        {
            if (await _fs.FileExistsAsync(cacheFilePath))
            {
                return await _fs.ReadJsonAsync<T>(cacheFilePath);
            }
            throw;
        }
    }
}