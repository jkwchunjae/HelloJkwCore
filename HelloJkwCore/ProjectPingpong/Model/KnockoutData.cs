namespace ProjectPingpong;

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
    public KnockoutId(string id)
        : base(id)
    {
        CompetitionName = Parse(id);
    }
    public KnockoutId(CompetitionName competitionName, string knockoutId)
        : this($"{competitionName}-{knockoutId}")
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
public class KnockoutData
{
    public KnockoutId Id { get; set; } = KnockoutId.Default;
    public CompetitionName CompetitionName { get; set; } = CompetitionName.Default;
    public List<Player>? PlayerList { get; set; }
    public List<MatchId>? MatchIdList { get; set; }
    [JsonIgnore] public List<KnockoutMatchData>? MatchList { get; set; }
}
