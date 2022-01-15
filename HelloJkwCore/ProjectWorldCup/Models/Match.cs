using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

public class Match
{
    /// <summary> 매치 시작 시간 </summary>
    public DateTime Time { get; set; }
    public MatchStatus Status { get; set; } = MatchStatus.Before;
    public Team HomeTeam { get; set; } = new();
    public Team AwayTeam { get; set; } = new();
    public int HomeScore { get; set; }
    public int AwayScore { get; set; }
    /// <summary> 홈팀 승부차기 득점 </summary>
    public int HomePenaltyScore { get; set; }
    /// <summary> 원정팀 승부차기 득점 </summary>
    public int AwayPenaltyScore { get; set; }
    public Dictionary<MatchInfoType, string> Info { get; set; } = new();

    public bool IsDraw => HomeScore == AwayScore && HomePenaltyScore == AwayPenaltyScore;
    public (Team Team, int Score, int PenaltyScore) Winner => HomeScore != AwayScore
        ? (HomeScore > AwayScore ? (HomeTeam, HomeScore, HomePenaltyScore) : (AwayTeam, AwayScore, AwayPenaltyScore))
        : (HomePenaltyScore > AwayPenaltyScore ? (HomeTeam, HomeScore, HomePenaltyScore) : (AwayTeam, AwayScore, AwayPenaltyScore));
    public (Team Team, int Score, int PenaltyScore) Looser => HomeScore != AwayScore
        ? (HomeScore < AwayScore ? (HomeTeam, HomeScore, HomePenaltyScore) : (AwayTeam, AwayScore, AwayPenaltyScore))
        : (HomePenaltyScore < AwayPenaltyScore ? (HomeTeam, HomeScore, HomePenaltyScore) : (AwayTeam, AwayScore, AwayPenaltyScore));


    public static Match CreateFromFifaMatchData(FifaMatchData fifaMatchData)
    {
        return new Match
        {
            Time = fifaMatchData.Date,
            Status = MatchStatus.Before,
            HomeTeam = new Team { Id = fifaMatchData.PlaceholderA, Name = fifaMatchData.PlaceholderA },
            AwayTeam = new Team { Id = fifaMatchData.PlaceholderB, Name = fifaMatchData.PlaceholderB },
            Info = new()
            {
                [MatchInfoType.MatchNumber] = fifaMatchData.MatchNumber.ToString(),
            },
        };
    }
}