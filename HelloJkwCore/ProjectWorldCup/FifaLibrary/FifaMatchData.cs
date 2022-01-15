using Newtonsoft.Json;

namespace ProjectWorldCup;

public class FifaMatchData
{
    [JsonProperty("idMatch")]
    public string IdMatch { get; set; }

    [JsonProperty("idCompetition")]
    public string IdCompetition { get; set; }

    [JsonProperty("idSeason")]
    public string IdSeason { get; set; }

    [JsonProperty("idStage")]
    public string IdStage { get; set; }

    [JsonProperty("idGroup")]
    public string IdGroup { get; set; }

    [JsonProperty("competitionName")]
    public string CompetitionName { get; set; }

    [JsonProperty("stageName")]
    public string StageName { get; set; }

    [JsonProperty("groupName")]
    public string GroupName { get; set; }

    /// <summary> UTC+0 </summary>
    [JsonProperty("date")]
    public DateTime Date { get; set; }

    [JsonProperty("matchDay")]
    public object MatchDay { get; set; }

    [JsonProperty("matchStatus")]
    public int MatchStatus { get; set; }

    /// <summary> UTC+0 </summary>
    [JsonProperty("matchTime")]
    public object MatchTime { get; set; }

    [JsonProperty("stadiumName")]
    public string StadiumName { get; set; }

    [JsonProperty("cityName")]
    public string CityName { get; set; }

    [JsonProperty("period")]
    public int Period { get; set; }

    [JsonProperty("winner")]
    public object Winner { get; set; }

    [JsonProperty("awayTeamPenaltyScore")]
    public object AwayTeamPenaltyScore { get; set; }

    [JsonProperty("homeTeamPenaltyScore")]
    public object HomeTeamPenaltyScore { get; set; }

    [JsonProperty("aggregateAwayTeamScore")]
    public object AggregateAwayTeamScore { get; set; }

    [JsonProperty("aggregateHomeTeamScore")]
    public object AggregateHomeTeamScore { get; set; }

    [JsonProperty("awayTeam")]
    public object AwayTeam { get; set; }

    [JsonProperty("homeTeam")]
    public object HomeTeam { get; set; }

    [JsonProperty("resultType")]
    public int ResultType { get; set; }

    [JsonProperty("placeholderA")]
    public string PlaceholderA { get; set; }

    [JsonProperty("placeholderB")]
    public string PlaceholderB { get; set; }

    [JsonProperty("weather")]
    public object Weather { get; set; }

    [JsonProperty("officials")]
    public IList<object> Officials { get; set; }

    [JsonProperty("timeDefined")]
    public bool TimeDefined { get; set; }

    [JsonProperty("confederation")]
    public object Confederation { get; set; }

    [JsonProperty("matchNumber")]
    public int MatchNumber { get; set; }
}