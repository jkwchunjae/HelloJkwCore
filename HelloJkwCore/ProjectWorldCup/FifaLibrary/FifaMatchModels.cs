namespace ProjectWorldCup.FifaLibrary;

public class FifaMatchTeam
{
    [JsonProperty("Score")] public int? Score { get; set; }
    [JsonProperty("Side")] public object Side { get; set; }
    [JsonProperty("IdTeam")] public string IdTeam { get; set; }
    [JsonProperty("PictureUrl")] public string PictureUrl { get; set; }
    [JsonProperty("IdCountry")] public string IdCountry { get; set; }
    [JsonProperty("Tactics")] public object Tactics { get; set; }
    [JsonProperty("TeamType")] public int TeamType { get; set; }
    [JsonProperty("AgeType")] public int AgeType { get; set; }
    [JsonProperty("TeamName")] public IList<FifaIdName> TeamName { get; set; }
    [JsonProperty("Abbreviation")] public string Abbreviation { get; set; }
    [JsonProperty("ShortClubName")] public string ShortClubName { get; set; }
    [JsonProperty("FootballType")] public int FootballType { get; set; }
    [JsonProperty("Gender")] public int Gender { get; set; }
    [JsonProperty("IdAssociation")] public string IdAssociation { get; set; }
}


public class FifaMatchData
{
    [JsonProperty("IdCompetition")] public string IdCompetition { get; set; }
    [JsonProperty("IdSeason")] public string IdSeason { get; set; }
    [JsonProperty("IdStage")] public string IdStage { get; set; }
    [JsonProperty("IdGroup")] public string IdGroup { get; set; }
    [JsonProperty("Weather")] public object Weather { get; set; }
    [JsonProperty("Attendance")] public object Attendance { get; set; }
    [JsonProperty("IdMatch")] public string IdMatch { get; set; }
    [JsonProperty("MatchDay")] public object MatchDay { get; set; }
    [JsonProperty("StageName")] public IList<FifaIdName> StageName { get; set; }
    [JsonProperty("GroupName")] public IList<FifaIdName> GroupName { get; set; }
    [JsonProperty("CompetitionName")] public IList<FifaIdName> CompetitionName { get; set; }
    [JsonProperty("SeasonName")] public IList<FifaIdName> SeasonName { get; set; }
    [JsonProperty("SeasonShortName")] public IList<FifaIdName> SeasonShortName { get; set; }
    [JsonProperty("Date")] public DateTime Date { get; set; }
    [JsonProperty("LocalDate")] public DateTime LocalDate { get; set; }
    [JsonProperty("Home")] public FifaMatchTeam Home { get; set; }
    [JsonProperty("Away")] public FifaMatchTeam Away { get; set; }
    [JsonProperty("HomeTeamScore")] public int? HomeTeamScore { get; set; }
    [JsonProperty("AwayTeamScore")] public int? AwayTeamScore { get; set; }
    [JsonProperty("AggregateHomeTeamScore")] public int? AggregateHomeTeamScore { get; set; }
    [JsonProperty("AggregateAwayTeamScore")] public int? AggregateAwayTeamScore { get; set; }
    [JsonProperty("HomeTeamPenaltyScore")] public int? HomeTeamPenaltyScore { get; set; }
    [JsonProperty("AwayTeamPenaltyScore")] public int? AwayTeamPenaltyScore { get; set; }
    [JsonProperty("LastPeriodUpdate")] public object LastPeriodUpdate { get; set; }
    [JsonProperty("Leg")] public object Leg { get; set; }
    [JsonProperty("IsHomeMatch")] public object IsHomeMatch { get; set; }
    [JsonProperty("Stadium")] public object Stadium { get; set; }
    [JsonProperty("IsTicketSalesAllowed")] public object IsTicketSalesAllowed { get; set; }
    [JsonProperty("MatchTime")] public object MatchTime { get; set; }
    [JsonProperty("SecondHalfTime")] public object SecondHalfTime { get; set; }
    [JsonProperty("FirstHalfTime")] public object FirstHalfTime { get; set; }
    [JsonProperty("FirstHalfExtraTime")] public object FirstHalfExtraTime { get; set; }
    [JsonProperty("SecondHalfExtraTime")] public object SecondHalfExtraTime { get; set; }
    [JsonProperty("Winner")] public object Winner { get; set; }
    [JsonProperty("MatchReportUrl")] public object MatchReportUrl { get; set; }
    [JsonProperty("PlaceHolderA")] public string PlaceHolderA { get; set; }
    [JsonProperty("PlaceHolderB")] public string PlaceHolderB { get; set; }
    [JsonProperty("BallPossession")] public object BallPossession { get; set; }
    [JsonProperty("Officials")] public IList<object> Officials { get; set; }
    [JsonProperty("MatchStatus")] public int MatchStatus { get; set; }
    [JsonProperty("ResultType")] public int ResultType { get; set; }
    [JsonProperty("MatchNumber")] public int? MatchNumber { get; set; }
    [JsonProperty("TimeDefined")] public bool TimeDefined { get; set; }
    [JsonProperty("OfficialityStatus")] public int OfficialityStatus { get; set; }
    [JsonProperty("MatchLegInfo")] public object MatchLegInfo { get; set; }
    [JsonProperty("Properties")] public object Properties { get; set; }
    [JsonProperty("IsUpdateable")] public object IsUpdateable { get; set; }
}

