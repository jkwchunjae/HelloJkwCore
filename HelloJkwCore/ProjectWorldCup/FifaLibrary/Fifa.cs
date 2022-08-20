using System.Runtime.Caching;

namespace ProjectWorldCup.FifaLibrary;

public partial class Fifa : IFifa
{
    private readonly IFileSystem _fs;
    private static HttpClient _httpClient = new();
    private MemoryCache _cache = new MemoryCache("FIFA");

    public Fifa(
        IFileSystemService fsService,
        WorldCupOption option)
    {
        _fs = fsService.GetFileSystem(option.FileSystemSelect, option.Path);
    }

    /// <summary> FIFA 는 __NEXT_DATA__ id 스크립트 안에 페이지에 포함될 json을 넣어놓는다. 그걸 가져와서 쓰자.  </summary>
    protected async Task<JObject> GetPageData(Uri uri)
    {
        var pageDataText = await GetFromCacheOrAsync<string>(uri.ToString(), async () =>
        {
            var res = await _httpClient.GetAsync(uri);
            var text = await res.Content.ReadAsStringAsync();
            var pattern = @"<script id=""__NEXT_DATA__"".*?>(.*?)<\/script>";
            var match = Regex.Match(text, pattern);
            var data = match.Groups[1].Captures[0].Value;

            return data;
        });

        var obj = JObject.Parse(pageDataText);
        var o = obj["props"]?["pageProps"]?["pageData"];

        return o?.ToObject<JObject>() ?? null;
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

            await _fs.WriteJsonAsync(cacheFilePath, result);
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

    protected async Task<T> UseCacheIfError<T>(string cacheKey, int retryCount, Func<Task<T>> func)
    {
        Func<Paths, string> cacheFilePath = paths => $"{paths["Cache"]}/{cacheKey}.json";

        while (retryCount-- > 0)
        {
            try
            {
                T result = await func();

                await _fs.WriteJsonAsync(cacheFilePath, result);
                _cache.Set(new CacheItem(cacheKey, result), new CacheItemPolicy
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddHours(1),
                });

                return result;
            }
            catch
            {
            }
        }

        if (_cache.Contains(cacheKey))
        {
            return (T)_cache.Get(cacheKey);
        }
        if (await _fs.FileExistsAsync(cacheFilePath))
        {
            return await _fs.ReadJsonAsync<T>(cacheFilePath);
        }

        return await func();
    }
}