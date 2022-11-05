namespace ProjectWorldCup.FifaLibrary;

public class FifaStandingTeam
{
    [JsonProperty("IdTeam")] public string IdTeam { get; set; }
    [JsonProperty("IdConfederation")] public string IdConfederation { get; set; }
    [JsonProperty("Type")] public int Type { get; set; }
    [JsonProperty("AgeType")] public int AgeType { get; set; }
    [JsonProperty("FootballType")] public int FootballType { get; set; }
    [JsonProperty("Gender")] public int Gender { get; set; }
    [JsonProperty("Name")] public IList<FifaIdName> Name { get; set; }
    [JsonProperty("IdAssociation")] public string IdAssociation { get; set; }
    [JsonProperty("IdCity")] public object IdCity { get; set; }
    [JsonProperty("Headquarters")] public object Headquarters { get; set; }
    [JsonProperty("TrainingCentre")] public object TrainingCentre { get; set; }
    [JsonProperty("OfficialSite")] public object OfficialSite { get; set; }
    [JsonProperty("City")] public string City { get; set; }
    [JsonProperty("IdCountry")] public string IdCountry { get; set; }
    [JsonProperty("PostalCode")] public string PostalCode { get; set; }
    [JsonProperty("RegionName")] public object RegionName { get; set; }
    [JsonProperty("ShortClubName")] public string ShortClubName { get; set; }
    [JsonProperty("Abbreviation")] public string Abbreviation { get; set; }
    [JsonProperty("Street")] public string Street { get; set; }
    [JsonProperty("FoundationYear")] public object FoundationYear { get; set; }
    [JsonProperty("Stadium")] public object Stadium { get; set; }
    [JsonProperty("PictureUrl")] public string PictureUrl { get; set; }
    [JsonProperty("ThumbnailUrl")] public object ThumbnailUrl { get; set; }
    [JsonProperty("DisplayName")] public IList<FifaIdName> DisplayName { get; set; }
    [JsonProperty("Content")] public IList<object> Content { get; set; }
    [JsonProperty("Properties")] public object Properties { get; set; }
    [JsonProperty("IsUpdateable")] public object IsUpdateable { get; set; }
}

public class FifaStandingData
{
    [JsonProperty("MatchDay")] public int MatchDay { get; set; }
    [JsonProperty("IdCompetition")] public string IdCompetition { get; set; }
    [JsonProperty("IdSeason")] public string IdSeason { get; set; }
    [JsonProperty("IdStage")] public string IdStage { get; set; }
    [JsonProperty("IdGroup")] public string IdGroup { get; set; }
    [JsonProperty("IdTeam")] public string IdTeam { get; set; }
    [JsonProperty("Date")] public DateTime Date { get; set; }
    [JsonProperty("Group")] public IList<FifaIdName> Group { get; set; }
    [JsonProperty("Won")] public int Won { get; set; }
    [JsonProperty("Lost")] public int Lost { get; set; }
    [JsonProperty("Drawn")] public int Drawn { get; set; }
    [JsonProperty("Played")] public int Played { get; set; }
    [JsonProperty("HomeWon")] public int HomeWon { get; set; }
    [JsonProperty("HomeLost")] public int HomeLost { get; set; }
    [JsonProperty("HomeDrawn")] public int HomeDrawn { get; set; }
    [JsonProperty("HomePlayed")] public int HomePlayed { get; set; }
    [JsonProperty("AwayWon")] public int AwayWon { get; set; }
    [JsonProperty("AwayLost")] public int AwayLost { get; set; }
    [JsonProperty("AwayDrawn")] public int AwayDrawn { get; set; }
    [JsonProperty("AwayPlayed")] public int AwayPlayed { get; set; }
    [JsonProperty("Against")] public int Against { get; set; }
    [JsonProperty("For")] public int For { get; set; }
    [JsonProperty("HomeAgainst")] public int HomeAgainst { get; set; }
    [JsonProperty("HomeFor")] public int HomeFor { get; set; }
    [JsonProperty("AwayAgainst")] public int AwayAgainst { get; set; }
    [JsonProperty("AwayFor")] public int AwayFor { get; set; }
    [JsonProperty("Position")] public int Position { get; set; }
    [JsonProperty("HomePosition")] public int HomePosition { get; set; }
    [JsonProperty("AwayPosition")] public int AwayPosition { get; set; }
    [JsonProperty("Points")] public int Points { get; set; }
    [JsonProperty("HomePoints")] public int HomePoints { get; set; }
    [JsonProperty("AwayPoints")] public int AwayPoints { get; set; }
    [JsonProperty("PreviousPosition")] public object PreviousPosition { get; set; }
    [JsonProperty("GoalsDiference")] public int GoalsDiference { get; set; }
    [JsonProperty("Team")] public FifaStandingTeam Team { get; set; }
    [JsonProperty("StartDate")] public DateTime StartDate { get; set; }
    [JsonProperty("EndDate")] public DateTime EndDate { get; set; }
    [JsonProperty("FairPlayCoefficient")] public double FairPlayCoefficient { get; set; }
    [JsonProperty("WinByExtraTime")] public int WinByExtraTime { get; set; }
    [JsonProperty("WinByPenalty")] public int WinByPenalty { get; set; }
    [JsonProperty("MatchResults")] public IList<object> MatchResults { get; set; }
    [JsonProperty("Properties")] public object Properties { get; set; }
    [JsonProperty("IsUpdateable")] public object IsUpdateable { get; set; }
}