using ProjectPingpong.Utils;

namespace ProjectPingpong;

[JsonConverter(typeof(StringIdJsonConverter<LeagueId>))]
public class LeagueId : StringId
{
    public static readonly LeagueId Default = new LeagueId(string.Empty);

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
    public LeagueId() { }
    public LeagueId(string id)
        : base(id)
    {
        CompetitionName = Parse(id);
    }
    public LeagueId(CompetitionName competitionName, string leagueId)
        : this($"{competitionName}.{leagueId}")
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
public class LeagueData
{
    public LeagueId Id { get; set; } = LeagueId.Default;
    public CompetitionName CompetitionName { get; set; } = CompetitionName.Default;
    public List<Player>? PlayerList { get; set; }
    public List<MatchId>? MatchIdList { get; set; }
    [JsonIgnore] public List<MatchData>? MatchList { get; set; }

    public LeagueData()
    {
    }

    public LeagueData(LeagueId id)
    {
        Id = id;
    }
}
public class LeagueUpdator
{
    LeagueData _leagueData;
    IPpService _service;

    public LeagueUpdator(LeagueData leagueData, IPpService service)
    {
        _leagueData = leagueData;
        _service = service;
    }

    public async Task<LeagueData> AddPlayer(Player player)
    {
        var leagueData = await _service.UpdateLeagueAsync(_leagueData.Id, leagueData =>
        {
            leagueData.PlayerList ??= new();
            leagueData.PlayerList.Add(player);

            return leagueData;
        });

        if (leagueData != null)
        {
            _leagueData = leagueData;
        }
        return _leagueData;
    }
    public async Task<LeagueData> RemovePlayer(PlayerName playerName)
    {
        var leagueData = await _service.UpdateLeagueAsync(_leagueData.Id, leagueData =>
        {
            leagueData.PlayerList ??= new();
            leagueData.PlayerList.RemoveAll(p => p.Name == playerName);

            return leagueData;
        });

        if (leagueData != null)
        {
            _leagueData = leagueData;
        }
        return _leagueData;
    }
    public async Task<LeagueData> AddMatch(MatchData matchData)
    {
        var leagueData = await _service.UpdateLeagueAsync(_leagueData.Id, leagueData =>
        {
            leagueData.MatchIdList ??= new();
            leagueData.MatchIdList.Add(matchData.Id);
            leagueData.MatchList ??= new();
            leagueData.MatchList.Add(matchData);

            return leagueData;
        });

        if (leagueData != null)
        {
            _leagueData = leagueData;
        }
        return _leagueData;
    }
    /// <summary>
    /// player를 기반으로 한 매치 생성
    /// </summary>
    /// <returns></returns>
    public async Task<LeagueData> CreateMatches()
    {
        ILeagueMatchGenerator matchGenerator = new LeagueMatchGenerator();
        var matches = matchGenerator.CreateLeagueMatch(_leagueData.PlayerList ?? new())
            .Select(x => new MatchData
            {
                Id = new MatchId(_leagueData.Id, x.Player1.Name, x.Player2.Name),
                LeftPlayer = x.Player1,
                RightPlayer = x.Player2,
            })
            .ToList();

        return await AddMatches(matches);
    }
    public async Task<LeagueData> AddMatches(IEnumerable<MatchData> matches)
    {
        var leagueData = await _service.UpdateLeagueAsync(_leagueData.Id, leagueData =>
        {
            leagueData.MatchIdList ??= new();
            leagueData.MatchIdList.AddRange(matches.Select(m => m.Id));
            leagueData.MatchList ??= new();
            leagueData.MatchList.AddRange(matches);

            return leagueData;
        });

        if (leagueData != null)
        {
            _leagueData = leagueData;
        }
        return _leagueData;
    }
    public async Task<LeagueData> RemoveMatch(MatchId matchId)
    {
        var leagueData = await _service.UpdateLeagueAsync(_leagueData.Id, leagueData =>
        {
            leagueData.MatchIdList?.RemoveAll(id => id == matchId);
            leagueData.MatchList?.RemoveAll(m => m.Id == matchId);

            return leagueData;
        });

        if (leagueData != null)
        {
            _leagueData = leagueData;
        }
        return _leagueData;
    }
}
