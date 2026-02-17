namespace ProjectWorldCup.FifaLibrary;

public partial class Fifa : IFifa
{
    private readonly string MatchesCacheKey = nameof(MatchesCacheKey);
    public static readonly string SeasonId = "285023"; // 2026 월드컵
    public static readonly string GroupStageId = "289273"; // 조별리그
    public static readonly string Round32StageId = "289287"; // 32강
    public static readonly string Round16StageId = "289288"; // 16강
    public static readonly string Round8StageId = "289289"; // 8강
    public static readonly string Round4StageId = "289290"; // 4강
    public static readonly string ThirdStageId = "289291"; // 3-4위전
    public static readonly string FinalStageId = "289292"; // 결승전

    public async Task<List<FifaMatchData>> GetGroupStageMatchesAsync()
    {
        var matches = await GetMatches(GroupStageId);

        if (matches.Count == 72)
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
    public async Task<List<FifaMatchData>> GetMatches(string stageId)
    {
        var cacheTime = DateTime.Now.ToString("yyyyMMdd.HHmm").Left(12); // 분의 앞자리만 쓴다. 10분에 한 번 캐시
        var cacheKey = $"{MatchesCacheKey}_{stageId}_{cacheTime}0";

        return await GetFromCacheOrAsync<List<FifaMatchData>>(cacheKey, async () =>
        {
            var url = $"https://api.fifa.com/api/v3/calendar/matches?idcompetition=17&idSeason={SeasonId}&idStage={stageId}&language=ko";
            var res = await _httpClient.GetAsync(url);
            var text = await res.Content.ReadAsStringAsync();
            text = text.Replace("{format}", "sq").Replace("{size}", "2");
            var root = _serializer.Deserialize<FifaDataRoot<FifaMatchData>>(text);
            if (root?.Results?.Any() ?? false)
            {
                return root.Results
                    .OrderBy(x => x.MatchNumber)
                    .ToList();
            }
            else
            {
                return new();
            }
        });
    }

    public async Task<List<FifaMatchData>> GetKnockoutStageMatchesAsync()
    {
        var matchesss = await Task.WhenAll(
            GetMatches(Round32StageId),
            GetMatches(Round16StageId),
            GetMatches(Round8StageId),
            GetMatches(Round4StageId),
            GetMatches(ThirdStageId),
            GetMatches(FinalStageId)
        );
        var matches = matchesss
            .SelectMany(x => x)
            .OrderBy(match => match.MatchNumber)
            .ToList();

        return matches;
    }

    public async Task<List<FifaMatchData>> GetRound32MatchesAsync()
    {
        var round32Matches = await GetMatches(Round32StageId);

        if (round32Matches.Count == 16)
        {
            if ((await GetFailoverData("Round32Matches.json")) == string.Empty)
            {
                await SaveFailoverData("Round32Matches.json", round32Matches);
            }
            return round32Matches;
        }
        else
        {
            return await GetFailoverData<List<FifaMatchData>>("Round32Matches.json");
        }
    }

    public async Task<List<FifaMatchData>> GetRound16MatchesAsync()
    {
        var round16Matches = await GetMatches(Round16StageId);

        if (round16Matches.Count == 8)
        {
            if ((await GetFailoverData("Round16Matches.json")) == string.Empty)
            {
                await SaveFailoverData("Round16Matches.json", round16Matches);
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
        var matchesss = await Task.WhenAll(
            GetMatches(Round8StageId),
            GetMatches(Round4StageId),
            GetMatches(ThirdStageId),
            GetMatches(FinalStageId)
        );
        var afterMatches = matchesss.SelectMany(x => x).ToList();

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