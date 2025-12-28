namespace ProjectWorldCup.FifaLibrary;

public class FifaStandingTeam
{
    [JsonPropertyName("IdTeam")] public string IdTeam { get; set; }
    [JsonPropertyName("IdConfederation")] public string IdConfederation { get; set; }
    [JsonPropertyName("Type")] public int Type { get; set; }
    [JsonPropertyName("AgeType")] public int AgeType { get; set; }
    [JsonPropertyName("FootballType")] public int FootballType { get; set; }
    [JsonPropertyName("Gender")] public int Gender { get; set; }
    [JsonPropertyName("Name")] public IList<FifaIdName> Name { get; set; }
    [JsonPropertyName("IdAssociation")] public string IdAssociation { get; set; }
    [JsonPropertyName("IdCity")] public object IdCity { get; set; }
    [JsonPropertyName("Headquarters")] public object Headquarters { get; set; }
    [JsonPropertyName("TrainingCentre")] public object TrainingCentre { get; set; }
    [JsonPropertyName("OfficialSite")] public object OfficialSite { get; set; }
    [JsonPropertyName("City")] public string City { get; set; }
    [JsonPropertyName("IdCountry")] public string IdCountry { get; set; }
    [JsonPropertyName("PostalCode")] public string PostalCode { get; set; }
    [JsonPropertyName("RegionName")] public object RegionName { get; set; }
    [JsonPropertyName("ShortClubName")] public string ShortClubName { get; set; }
    [JsonPropertyName("Abbreviation")] public string Abbreviation { get; set; }
    [JsonPropertyName("Street")] public string Street { get; set; }
    [JsonPropertyName("FoundationYear")] public object FoundationYear { get; set; }
    [JsonPropertyName("Stadium")] public object Stadium { get; set; }
    [JsonPropertyName("PictureUrl")] public string PictureUrl { get; set; }
    [JsonPropertyName("ThumbnailUrl")] public object ThumbnailUrl { get; set; }
    [JsonPropertyName("DisplayName")] public IList<FifaIdName> DisplayName { get; set; }
    [JsonPropertyName("Content")] public IList<object> Content { get; set; }
    [JsonPropertyName("Properties")] public object Properties { get; set; }
    [JsonPropertyName("IsUpdateable")] public object IsUpdateable { get; set; }
}

public class FifaStandingData
{
    [JsonPropertyName("MatchDay")] public int MatchDay { get; set; }
    [JsonPropertyName("IdCompetition")] public string IdCompetition { get; set; }
    [JsonPropertyName("IdSeason")] public string IdSeason { get; set; }
    [JsonPropertyName("IdStage")] public string IdStage { get; set; }
    [JsonPropertyName("IdGroup")] public string IdGroup { get; set; }
    [JsonPropertyName("IdTeam")] public string IdTeam { get; set; }
    [JsonPropertyName("Date")] public DateTime Date { get; set; }
    [JsonPropertyName("Group")] public IList<FifaIdName> Group { get; set; }
    [JsonPropertyName("Won")] public int Won { get; set; }
    [JsonPropertyName("Lost")] public int Lost { get; set; }
    [JsonPropertyName("Drawn")] public int Drawn { get; set; }
    [JsonPropertyName("Played")] public int Played { get; set; }
    [JsonPropertyName("HomeWon")] public int HomeWon { get; set; }
    [JsonPropertyName("HomeLost")] public int HomeLost { get; set; }
    [JsonPropertyName("HomeDrawn")] public int HomeDrawn { get; set; }
    [JsonPropertyName("HomePlayed")] public int HomePlayed { get; set; }
    [JsonPropertyName("AwayWon")] public int AwayWon { get; set; }
    [JsonPropertyName("AwayLost")] public int AwayLost { get; set; }
    [JsonPropertyName("AwayDrawn")] public int AwayDrawn { get; set; }
    [JsonPropertyName("AwayPlayed")] public int AwayPlayed { get; set; }
    [JsonPropertyName("Against")] public int Against { get; set; }
    [JsonPropertyName("For")] public int For { get; set; }
    [JsonPropertyName("HomeAgainst")] public int HomeAgainst { get; set; }
    [JsonPropertyName("HomeFor")] public int HomeFor { get; set; }
    [JsonPropertyName("AwayAgainst")] public int AwayAgainst { get; set; }
    [JsonPropertyName("AwayFor")] public int AwayFor { get; set; }
    [JsonPropertyName("Position")] public int Position { get; set; }
    [JsonPropertyName("HomePosition")] public int HomePosition { get; set; }
    [JsonPropertyName("AwayPosition")] public int AwayPosition { get; set; }
    [JsonPropertyName("Points")] public int Points { get; set; }
    [JsonPropertyName("HomePoints")] public int HomePoints { get; set; }
    [JsonPropertyName("AwayPoints")] public int AwayPoints { get; set; }
    [JsonPropertyName("PreviousPosition")] public object PreviousPosition { get; set; }
    [JsonPropertyName("GoalsDiference")] public int GoalsDiference { get; set; }
    [JsonPropertyName("Team")] public FifaStandingTeam Team { get; set; }
    [JsonPropertyName("StartDate")] public DateTime StartDate { get; set; }
    [JsonPropertyName("EndDate")] public DateTime EndDate { get; set; }
    [JsonPropertyName("FairPlayCoefficient")] public double FairPlayCoefficient { get; set; }
    [JsonPropertyName("WinByExtraTime")] public int WinByExtraTime { get; set; }
    [JsonPropertyName("WinByPenalty")] public int WinByPenalty { get; set; }
    [JsonPropertyName("MatchResults")] public IList<object> MatchResults { get; set; }
    [JsonPropertyName("Properties")] public object Properties { get; set; }
    [JsonPropertyName("IsUpdateable")] public object IsUpdateable { get; set; }
}