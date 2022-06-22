namespace ProjectPingpong;

[JsonConverter(typeof(StringIdJsonConverter<KnockoutId>))]
public class KnockoutId : StringId
{
    public static readonly KnockoutId Default = new KnockoutId(string.Empty);
    private CompetitionName _competitionName = CompetitionName.Default;
    public CompetitionName CompetitionName
    {
        get
        {
            if (string.IsNullOrEmpty(_competitionName?.Id))
            {
                _competitionName = Parse(Id);
            }
            return _competitionName;
        }
        set
        {
            _competitionName = value;
        }
    }
    public KnockoutId() { }
    public KnockoutId(string id)
        : base(id)
    {
        CompetitionName = Parse(id);
    }
    public KnockoutId(CompetitionName competitionName, string knockoutId)
        : this($"{competitionName}.{knockoutId}")
    {
    }

    private CompetitionName Parse(string id)
    {
        if (id == string.Empty)
        {
            return CompetitionName.Default;
        }
        else
        {
            var arr = id.Split('.');
            var cName = new CompetitionName(arr[0]);
            return cName;
        }
    }

}
public class KnockoutData
{
    public KnockoutId Id { get; set; } = KnockoutId.Default;
    public CompetitionName CompetitionName { get; set; } = CompetitionName.Default;
    public List<PlayerName>? PlayerList { get; set; }
    public List<MatchId>? MatchIdList { get; set; }
    [JsonIgnore] public List<KnockoutMatchData>? MatchList { get; set; }
}
public class KnockoutUpdator
{
    KnockoutData _knockoutData;
    IPpService _service;
    IPpMatchService _matchService;
    public KnockoutUpdator(KnockoutData knockoutData, IPpService service, IPpMatchService matchService)
    {
        _knockoutData = knockoutData;
        _service = service;
        _matchService = matchService;
    }

    public async Task<KnockoutData> AddPlayer(IEnumerable<PlayerName> players)
    {
        var knockoutData = await _service.UpdateKnockoutAsync(_knockoutData.Id, knockoutData =>
        {
            knockoutData.PlayerList ??= new();

            // 진짜 추가된 유저 추가
            var newPlayers = players.Where(p => knockoutData.PlayerList.Empty(pp => pp.Name == p.Name));
            knockoutData.PlayerList = knockoutData.PlayerList
                .Concat(newPlayers)
                .ToList();

            return knockoutData;
        });

        if (knockoutData != null)
        {
            _knockoutData = knockoutData;
        }
        return _knockoutData;
    }
    public async Task<KnockoutData> RemovePlayer(PlayerName playerName)
    {
        var knockoutData = await _service.UpdateKnockoutAsync(_knockoutData.Id, knockoutData =>
        {
            knockoutData.PlayerList ??= new();
            knockoutData.PlayerList.RemoveAll(pName => pName == playerName);

            return knockoutData;
        });

        if (knockoutData != null)
        {
            _knockoutData = knockoutData;
        }
        return _knockoutData;

    }

    public async Task<KnockoutData> AddMatches(IEnumerable<KnockoutMatchData> matches)
    {
        var knockoutData = await _service.UpdateKnockoutAsync(_knockoutData.Id, knockoutData =>
        {
            knockoutData.MatchIdList ??= new();
            knockoutData.MatchIdList = knockoutData.MatchIdList
                .Concat(matches.Select(m => m.Id))
                .ToList();
            knockoutData.MatchList ??= new();
            knockoutData.MatchList = knockoutData.MatchList
                .Concat(matches)
                .ToList();

            return knockoutData;
        });

        if (knockoutData != null)
        {
            _knockoutData = knockoutData;
        }
        foreach (var match in matches)
        {
            await _matchService.CreateMatchAsync<KnockoutMatchData>(match.Id);
        }
        return _knockoutData;
    }
    public async Task<KnockoutData> ClearMatches()
    {
        var prevMatches = _knockoutData.MatchIdList;
        var knockoutData = await _service.UpdateKnockoutAsync(_knockoutData.Id, knockoutData =>
        {
            knockoutData.MatchIdList ??= new();
            knockoutData.MatchIdList.Clear();
            knockoutData.MatchList ??= new();
            knockoutData.MatchList.Clear();

            return knockoutData;
        });

        if (knockoutData != null)
        {
            _knockoutData = knockoutData;
        }
        foreach (var match in prevMatches ?? new())
        {
            await _matchService.DeleteMatchDataAsync(match);
        }
        return _knockoutData;
    }
}
