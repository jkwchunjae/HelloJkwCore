using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Runtime.Caching;

namespace ProjectWorldCup;

public partial class Fifa : IFifa
{
    private static HttpClient _httpClient = new();
    private MemoryCache _cache = new MemoryCache("FIFA");

    /// <summary> FIFA 는 __NEXT_DATA__ id 스크립트 안에 페이지에 포함될 json을 넣어놓는다. 그걸 가져와서 쓰자.  </summary>
    private async Task<JObject> GetPageData(Uri uri)
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

    private async Task<T> GetFromCacheOrAsync<T>(string cacheKey, Func<Task<T>> func)
    {
        try
        {
            if (_cache.Contains(cacheKey))
            {
                return (T)_cache.Get(cacheKey);
            }

            var result = await func();

            _cache.Set(new CacheItem(cacheKey, result), new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(5),
            });

            return result;
        }
        catch
        {
            throw;
        }
    }
}