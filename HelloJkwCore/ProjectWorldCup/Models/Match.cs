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

public class Match<TTeam> where TTeam : Team
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

    public bool IsDraw => HomeScore == AwayScore && HomePenaltyScore == AwayPenaltyScore;
    public (TTeam Team, int Score, int PenaltyScore) Winner => HomeScore != AwayScore
        ? (HomeScore > AwayScore ? (HomeTeam, HomeScore, HomePenaltyScore) : (AwayTeam, AwayScore, AwayPenaltyScore))
        : (HomePenaltyScore > AwayPenaltyScore ? (HomeTeam, HomeScore, HomePenaltyScore) : (AwayTeam, AwayScore, AwayPenaltyScore));
    public (TTeam Team, int Score, int PenaltyScore) Looser => HomeScore != AwayScore
        ? (HomeScore < AwayScore ? (HomeTeam, HomeScore, HomePenaltyScore) : (AwayTeam, AwayScore, AwayPenaltyScore))
        : (HomePenaltyScore < AwayPenaltyScore ? (HomeTeam, HomeScore, HomePenaltyScore) : (AwayTeam, AwayScore, AwayPenaltyScore));

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
        Info = match.Info;
    }
}
