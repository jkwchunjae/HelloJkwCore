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
        var beginDate = new DateTime(2022, 11, 20);
        var endDate = new DateTime(2022, 12, 3);
        var matches2 = await GetMatches(beginDate, endDate);
        var matches = matches2
            .Where(match => match.IdStage == GroupStageId)
            .GroupBy(match => match.IdMatch, (k, arr) => arr.First())
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
    public async Task<List<FifaMatchData>> GetMatches(DateTime begin, DateTime end)
    {
        var cacheTime = DateTime.Now.ToString("yyyyMMdd.HHmm").Left(12); // 분의 앞자리만 쓴다. 10분에 한 번 캐시
        var cacheKey = $"{MatchesCacheKey}_{begin:yyyyMMdd}_{end:yyyyMMdd}_{cacheTime}0";

        return await GetFromCacheOrAsync<List<FifaMatchData>>(cacheKey, async () =>
        {
            var url = $"https://api.fifa.com/api/v3/calendar/matches?from={begin:yyyy-MM-dd}&to={end:yyyy-MM-dd}&idcompetition=17&language=en";
            var res = await _httpClient.GetAsync(url);
            var text = await res.Content.ReadAsStringAsync();
            text = text.Replace("{format}", "sq").Replace("{size}", "2");
            var root = JsonConvert.DeserializeObject<FifaDataRoot<FifaMatchData>>(text);
            if (root?.Results?.Any() ?? false)
            {
                return root.Results.ToList();
            }
            else
            {
                return new();
            }
        });
    }

    public async Task<List<FifaMatchData>> GetKnockoutStageMatchesAsync()
    {
        var beginDate = new DateTime(2022, 12, 3);
        var endDate = new DateTime(2022, 12, 20);
        var matches2 = await GetMatches(beginDate, endDate);
        var matches = matches2
            .Where(match => match.IdStage != GroupStageId)
            .GroupBy(match => match.IdMatch, (k, arr) => arr.First())
            .OrderBy(match => match.MatchNumber)
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