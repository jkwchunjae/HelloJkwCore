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

        /// <summary> FIFA 는 __NEXT_DATA__ id 스크립트 안에 페이지에 포함될 json을 넣어놓는다. 그걸 가져와서 쓰자.  </summary>
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
