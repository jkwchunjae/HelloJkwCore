namespace ProjectWorldCup.FifaLibrary;

public partial class Fifa : IFifa
{
    private readonly string GroupStageMatchesCacheKey = nameof(GroupStageMatchesCacheKey);
    private readonly string KnockoutStageMatchesCacheKey = nameof(KnockoutStageMatchesCacheKey);

    public async Task<List<FifaMatchData>> GetGroupStageMatchesAsync()
    {
        var beginDate = new DateTime(2022, 11, 21);
        var endDate = new DateTime(2022, 12, 3);
        var dates = Enumerable.Range(0, 999)
            .Select(x => beginDate.AddDays(x))
            .Where(date => date <= endDate);
        var matches2 = await dates
            .Select(async date => await GetMatches(date))
            .WhenAll();
        var matches = matches2
            .SelectMany(m => m)
            .GroupBy(match => match.IdMatch, (k, arr) => arr.First())
            .ToList();

        return matches;
    }
    public async Task<List<FifaMatchData>> GetMatches(DateTime date)
    {
        var retryCount = 5;
        return await UseCacheIfError<List<FifaMatchData>>(GroupStageMatchesCacheKey + date.ToString("yyyy.MM.dd"), retryCount, async () =>
        {
            var url = $"https://cxm-api.fifa.com/fifaplusweb/api/sections/competitionpage/matches?competitionId=17&locale=en&date={date:yyyy-MM-dd}&timezoneoffset=-540";
            var res = await _httpClient.GetAsync(url);
            var text = await res.Content.ReadAsStringAsync();
            var root = JsonConvert.DeserializeObject<MatchDataRoot>(text);
            return root.Competition.ActiveSeasons[0].Matches;
        });
    }

    public async Task<List<FifaMatchData>> GetKnockoutStageMatchesAsync()
    {
        return await GetFromCacheOrAsync<List<FifaMatchData>>(KnockoutStageMatchesCacheKey, async () =>
        {
            var url = "https://www.fifa.com/tournaments/mens/worldcup/qatar2022/match-center";
            var pageData = await GetPageData(new Uri(url));
            var knockoutStageMatches = pageData["matchesOverviewProps"]["knockoutPhaseMatches"].ToArray();

            return knockoutStageMatches
                ?.SelectMany(x => x["matches"].Select(e => e.ToObject<FifaMatchData>()).ToList())
                .ToList() ?? new();
        });
    }
}