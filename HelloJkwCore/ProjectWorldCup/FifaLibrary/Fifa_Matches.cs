namespace ProjectWorldCup.FifaLibrary;

public partial class Fifa : IFifa
{
    private readonly string MatchesCacheKey = nameof(MatchesCacheKey);

    public static readonly string GroupStageId = "285063";
    public static readonly string Round16StageId = "285073"; // 16강
    public static readonly string Round8StageId = "285074"; // 8강
    public static readonly string Round4StageId = "285075"; // 4강
    public static readonly string ThirdStageId = "285076"; // 3-4위전
    public static readonly string FinalStageId = "285077"; // 결승전

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
            .Where(match => match.IdStage == GroupStageId)
            .ToList();

        if (matches.Count == 48)
        {
            if ((await GetFailoverData("GroupStageMatches.json")) == string.Empty)
            {
                await SaveFailoverData("GroupStageMatches.json", matches);
            }
            return matches;
        }
        else
        {
            return await GetFailoverData<List<FifaMatchData>>("GroupStageMatches.json");
        }
    }
    public async Task<List<FifaMatchData>> GetMatches(DateTime date)
    {
        string cacheKey;
        if (date < DateTime.Now.AddDays(-1) || date > DateTime.Now.AddDays(1))
        {
            cacheKey = $"{MatchesCacheKey}_{date:yyyyMMdd}";
        }
        else
        {
            var cacheTime = DateTime.Now.ToString("yyyyMMdd.HHmm").Left(12); // 분의 앞자리만 쓴다. 10분에 한 번 캐시
            cacheKey = $"{MatchesCacheKey}_{date:yyyyMMdd}_{cacheTime}";
        }
        return await GetFromCacheOrAsync<List<FifaMatchData>>(cacheKey, async () =>
        {
            var url = $"https://cxm-api.fifa.com/fifaplusweb/api/sections/competitionpage/matches?competitionId=17&locale=en&date={date:yyyy-MM-dd}&timezoneoffset=-540";
            var res = await _httpClient.GetAsync(url);
            var text = await res.Content.ReadAsStringAsync();
            var root = JsonConvert.DeserializeObject<MatchDataRoot>(text);
            if (root?.Competition?.ActiveSeasons?.Any() ?? false)
            {
                return root.Competition.ActiveSeasons[0].Matches;
            }
            else
            {
                return new();
            }
        });
    }

    public async Task<List<FifaMatchData>> GetKnockoutStageMatchesAsync()
    {
        var beginDate = new DateTime(2022, 12, 4);
        var endDate = new DateTime(2022, 12, 20);
        var dates = Enumerable.Range(0, 999)
            .Select(x => beginDate.AddDays(x))
            .Where(date => date <= endDate);
        var matches2 = await dates
            .Select(async date => await GetMatches(date))
            .WhenAll();
        var matches = matches2
            .SelectMany(m => m)
            .GroupBy(match => match.IdMatch, (k, arr) => arr.First())
            .OrderBy(match => match.Date)
            .ToList();

        return matches;
    }

    public async Task<List<FifaMatchData>> GetRound16MatchesAsync()
    {
        var matches = await GetKnockoutStageMatchesAsync();
        var round16Matches = matches
            .Where(match => match.IdStage == Round16StageId)
            .ToList();

        if (round16Matches.Count == 8)
        {
            if ((await GetFailoverData("Round16Matches.json")) == string.Empty)
            {
                await SaveFailoverData("Round16Matches.json", matches);
            }
            return round16Matches;
        }
        else
        {
            return await GetFailoverData<List<FifaMatchData>>("Round16Matches.json");
        }
    }

    public async Task<List<FifaMatchData>> GetFinalMatchesAsync()
    {
        var matches = await GetKnockoutStageMatchesAsync();
        var afterMatches = matches
            .Where(match => match.IdStage != Round16StageId)
            .ToList();

        if (afterMatches.Count == 8)
        {
            if ((await GetFailoverData("FinalMatches.json")) == string.Empty)
            {
                await SaveFailoverData("FinalMatches.json", afterMatches);
            }
            return afterMatches;
        }
        else
        {
            return await GetFailoverData<List<FifaMatchData>>("FinalMatches.json");
        }
    }
}