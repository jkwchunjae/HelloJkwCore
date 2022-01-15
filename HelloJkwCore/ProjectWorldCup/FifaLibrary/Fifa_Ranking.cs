using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectWorldCup;

public class RankingTeamData
{
    public RankingItem RankingItem { get; set; }
    public double PreviousPoints { get; set; }
    [JsonProperty("tag")]
    public TeamTag Region { get; set; }
}
public class RankingItem
{
    public int Rank { get; set; }
    public TeamFlag Flag { get; set; }
    public string Name { get; set; }
    public double TotalPoints { get; set; }
    public bool Active { get; set; }
    public int PreviousRank { get; set; }
    public string CountryUrl { get; set; }
    public string CountryCode { get; set; }
}
public partial class Fifa : IFifa
{
    private readonly string RankingCacheKey = nameof(RankingCacheKey);

    public async Task<List<RankingTeamData>> GetLastRankingAsync(Gender gender)
    {
        return await GetFromCacheOrAsync<List<RankingTeamData>>($"{RankingCacheKey}{gender}", async () =>
        {
            var rankInfoUrl = $"https://www.fifa.com/fifa-world-ranking/{gender}".ToLower();
            var pageData = await GetPageData(new Uri(rankInfoUrl));
            var dates = pageData?["ranking"]?["dates"]?
                .Select(x => new { Id = x.Value<string>("id"), Text = x.Value<string>("text") })
                .ToList();

            var lastDateId = dates?.FirstOrDefault()?.Id ?? string.Empty;
            var url = $@"https://www.fifa.com/api/ranking-overview?locale=en&dateId={lastDateId}";
            var res = await _httpClient.GetAsync(url);
            var text = await res.Content.ReadAsStringAsync();
            var obj = JObject.Parse(text);

            var ranking = obj["rankings"]?.ToObject<List<RankingTeamData>>();
            return ranking ?? new();
        });
    }
}