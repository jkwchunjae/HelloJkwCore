namespace ProjectPingpong;

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
    public LeagueId(string id)
        : base(id)
    {
        CompetitionName = Parse(id);
    }
    public LeagueId(CompetitionName competitionName, string leagueId)
        : this($"{competitionName}-{leagueId}")
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
            var arr = id.Split('-');
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
}
