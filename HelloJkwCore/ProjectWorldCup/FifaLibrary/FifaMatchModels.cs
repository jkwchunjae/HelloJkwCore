namespace ProjectWorldCup.FifaLibrary;

public class FifaMatchTeam
{
    [JsonPropertyName("Score")] public int? Score { get; set; }
    [JsonPropertyName("Side")] public object Side { get; set; }
    [JsonPropertyName("IdTeam")] public string IdTeam { get; set; }
    [JsonPropertyName("PictureUrl")] public string PictureUrl { get; set; }
    [JsonPropertyName("IdCountry")] public string IdCountry { get; set; }
    [JsonPropertyName("Tactics")] public object Tactics { get; set; }
    [JsonPropertyName("TeamType")] public int TeamType { get; set; }
    [JsonPropertyName("AgeType")] public int AgeType { get; set; }
    [JsonPropertyName("TeamName")] public IList<FifaIdName> TeamName { get; set; }
    [JsonPropertyName("Abbreviation")] public string Abbreviation { get; set; }
    [JsonPropertyName("ShortClubName")] public string ShortClubName { get; set; }
    [JsonPropertyName("FootballType")] public int FootballType { get; set; }
    [JsonPropertyName("Gender")] public int Gender { get; set; }
    [JsonPropertyName("IdAssociation")] public string IdAssociation { get; set; }
}


public class FifaMatchData
{
    [JsonPropertyName("IdCompetition")] public string IdCompetition { get; set; }
    [JsonPropertyName("IdSeason")] public string IdSeason { get; set; }
    [JsonPropertyName("IdStage")] public string IdStage { get; set; }
    [JsonPropertyName("IdGroup")] public string IdGroup { get; set; }
    [JsonPropertyName("Weather")] public object Weather { get; set; }
    [JsonPropertyName("Attendance")] public object Attendance { get; set; }
    [JsonPropertyName("IdMatch")] public string IdMatch { get; set; }
    [JsonPropertyName("MatchDay")] public object MatchDay { get; set; }
    [JsonPropertyName("StageName")] public IList<FifaIdName> StageName { get; set; }
    [JsonPropertyName("GroupName")] public IList<FifaIdName> GroupName { get; set; }
    [JsonPropertyName("CompetitionName")] public IList<FifaIdName> CompetitionName { get; set; }
    [JsonPropertyName("SeasonName")] public IList<FifaIdName> SeasonName { get; set; }
    [JsonPropertyName("SeasonShortName")] public IList<FifaIdName> SeasonShortName { get; set; }
    [JsonPropertyName("Date")] public DateTime Date { get; set; }
    [JsonPropertyName("LocalDate")] public DateTime LocalDate { get; set; }
    [JsonPropertyName("Home")] public FifaMatchTeam Home { get; set; }
    [JsonPropertyName("Away")] public FifaMatchTeam Away { get; set; }
    [JsonPropertyName("HomeTeamScore")] public int? HomeTeamScore { get; set; }
    [JsonPropertyName("AwayTeamScore")] public int? AwayTeamScore { get; set; }
    [JsonPropertyName("AggregateHomeTeamScore")] public int? AggregateHomeTeamScore { get; set; }
    [JsonPropertyName("AggregateAwayTeamScore")] public int? AggregateAwayTeamScore { get; set; }
    [JsonPropertyName("HomeTeamPenaltyScore")] public int? HomeTeamPenaltyScore { get; set; }
    [JsonPropertyName("AwayTeamPenaltyScore")] public int? AwayTeamPenaltyScore { get; set; }
    [JsonPropertyName("LastPeriodUpdate")] public object LastPeriodUpdate { get; set; }
    [JsonPropertyName("Leg")] public object Leg { get; set; }
    [JsonPropertyName("IsHomeMatch")] public object IsHomeMatch { get; set; }
    [JsonPropertyName("Stadium")] public object Stadium { get; set; }
    [JsonPropertyName("IsTicketSalesAllowed")] public object IsTicketSalesAllowed { get; set; }
    [JsonPropertyName("MatchTime")] public object MatchTime { get; set; }
    [JsonPropertyName("SecondHalfTime")] public object SecondHalfTime { get; set; }
    [JsonPropertyName("FirstHalfTime")] public object FirstHalfTime { get; set; }
    [JsonPropertyName("FirstHalfExtraTime")] public object FirstHalfExtraTime { get; set; }
    [JsonPropertyName("SecondHalfExtraTime")] public object SecondHalfExtraTime { get; set; }
    [JsonPropertyName("Winner")] public string Winner { get; set; }
    [JsonPropertyName("MatchReportUrl")] public object MatchReportUrl { get; set; }
    [JsonPropertyName("PlaceHolderA")] public string PlaceHolderA { get; set; }
    [JsonPropertyName("PlaceHolderB")] public string PlaceHolderB { get; set; }
    [JsonPropertyName("BallPossession")] public object BallPossession { get; set; }
    [JsonPropertyName("Officials")] public IList<object> Officials { get; set; }
    [JsonPropertyName("MatchStatus")] public int MatchStatus { get; set; }
    [JsonPropertyName("ResultType")] public int ResultType { get; set; }
    [JsonPropertyName("MatchNumber")] public int? MatchNumber { get; set; }
    [JsonPropertyName("TimeDefined")] public bool TimeDefined { get; set; }
    [JsonPropertyName("OfficialityStatus")] public int OfficialityStatus { get; set; }
    [JsonPropertyName("MatchLegInfo")] public object MatchLegInfo { get; set; }
    [JsonPropertyName("Properties")] public object Properties { get; set; }
    [JsonPropertyName("IsUpdateable")] public object IsUpdateable { get; set; }
}

