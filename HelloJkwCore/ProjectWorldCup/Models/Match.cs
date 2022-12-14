namespace ProjectWorldCup;

[JsonConverter(typeof(StringEnumConverter))]
public enum MatchStatus
{
    Before,
    Playing,
    Done,
}

[JsonConverter(typeof(StringEnumConverter))]
public enum MatchInfoType
{
    GroupName,
    MatchNumber,
}

public interface IMatch<TTeam> where TTeam : Team
{
    /// <summary> 매치 시작 시간 </summary>
    DateTime Time { get; set; }
    MatchStatus Status { get; set; }
    TTeam HomeTeam { get; set; }
    TTeam AwayTeam { get; set; }
    int HomeScore { get; set; }
    int AwayScore { get; set; }
    /// <summary> 홈팀 승부차기 득점 </summary>
    int HomePenaltyScore { get; set; }
    /// <summary> 원정팀 승부차기 득점 </summary>
    int AwayPenaltyScore { get; set; }
    Dictionary<MatchInfoType, string> Info { get; set; }

    IEnumerable<TTeam> Teams { get; }
    bool IsDraw { get; }
    (TTeam Team, int Score, int PenaltyScore) Winner { get; }
    (TTeam Team, int Score, int PenaltyScore) Looser { get; }
}

public class Match<TTeam> : IMatch<TTeam> where TTeam : Team
{
    /// <summary> 매치 시작 시간 </summary>
    public DateTime Time { get; set; }
    public MatchStatus Status { get; set; } = MatchStatus.Before;
    public TTeam HomeTeam { get; set; }
    public TTeam AwayTeam { get; set; }
    public int HomeScore { get; set; }
    public int AwayScore { get; set; }
    /// <summary> 홈팀 승부차기 득점 </summary>
    public int HomePenaltyScore { get; set; }
    /// <summary> 원정팀 승부차기 득점 </summary>
    public int AwayPenaltyScore { get; set; }
    public Dictionary<MatchInfoType, string> Info { get; set; } = new();
    public IEnumerable<TTeam> Teams => new[] { HomeTeam, AwayTeam };
    public bool IsDraw => HomeScore == AwayScore && HomePenaltyScore == AwayPenaltyScore;

    public string WinnerId { get; set; }
    public (TTeam Team, int Score, int PenaltyScore) Winner => string.IsNullOrEmpty(WinnerId) ? default
        : WinnerId == HomeTeam.FifaTeamId ? (HomeTeam, HomeScore, HomePenaltyScore) : (AwayTeam, AwayScore, AwayPenaltyScore);
    public (TTeam Team, int Score, int PenaltyScore) Looser => string.IsNullOrEmpty(WinnerId) ? default
        : WinnerId == HomeTeam.FifaTeamId ? (AwayTeam, AwayScore, AwayPenaltyScore) : (HomeTeam, HomeScore, HomePenaltyScore);

    public Match()
    {
    }

    public Match(Match<TTeam> match)
    {
        Time = match.Time;
        Status = match.Status;
        HomeTeam = match.HomeTeam;
        AwayTeam = match.AwayTeam;
        HomeScore = match.HomeScore;
        AwayTeam = match.AwayTeam;
        WinnerId = match.WinnerId;
        Info = match.Info;
    }
}
