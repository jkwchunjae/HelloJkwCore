using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProjectWorldCup
{
    public partial class Fifa : IFifa
    {
        private static HttpClient _httpClient = new();

        private async Task<JObject?> GetPageData(Uri uri)
        {
            var res = await _httpClient.GetAsync(uri);

            var text = await res.Content.ReadAsStringAsync();

            var pattern = @"<script id=""__NEXT_DATA__"".*?>(.*?)<\/script>";

            var match = Regex.Match(text, pattern);

            var data = match.Groups[1].Captures[0].Value;

            var obj = JObject.Parse(data);

            var o = obj["props"]?["pageProps"]?["pageData"];

            return o?.ToObject<JObject>() ?? null;
        }
    }
}
